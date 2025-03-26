using AcsEmailEventsHandler;
using AcsEmailEventsHandler.Engine;
using System.Net;

Console.Title = nameof(AcsEmailEventsHandler.Engine);

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    //options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var azureSqlConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(azureSqlConnectionString))
{
    throw new InvalidOperationException("Environment variable 'AZURE_SQL_CONNECTION_STRING' is missing.");
}

var sqlDatabaseInitializer = new SqlDatabaseInitializer(azureSqlConnectionString);
sqlDatabaseInitializer.Init();

builder.Services.AddScoped<CloudEventHandler>();
builder.Services.AddScoped(services =>
{
    var logger = services.GetRequiredService<ILogger<EmailEventsRepository>>();
    return new EmailEventsRepository(azureSqlConnectionString, logger);
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Endpoint validation with CloudEvents v1.0
// https://learn.microsoft.com/en-us/azure/event-grid/end-point-validation-cloud-events-schema
app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.Headers.Add("Allow", "GET, POST");
        context.Response.Headers.Add("WebHook-Request-Origin", "*");

        context.Response.StatusCode = (int)HttpStatusCode.OK;

        string? webHookRequestCallback = context.Request.Headers!["WebHook-Request-Callback"];

        if (webHookRequestCallback is null)
        {
            throw new InvalidOperationException("WebHook-Request-Callback header is missing.");
        }

        /* Http GET is expected on the validation url that is included in
        the 'WebHook-Request-Callback' request header(within 10 minutes). */
        _ = Task.Run(() =>
        {
            Task.Delay(7000).Wait();
            using HttpClient client = new();
            var result = client.GetAsync(webHookRequestCallback).Result;
            Console.WriteLine($"GET request to {webHookRequestCallback} returned {result.StatusCode}.");
        });

        return;
    }

    await next.Invoke();
});

app.MapGet("/", () => "Working...");

app.Map("/handler/event-grid", async (HttpContext context,
    CloudEventHandler cloudEventHandler, ILogger<Program> logger) =>
{
    await using var memoryStream = new MemoryStream();
    await context.Request.Body.CopyToAsync(memoryStream);
    var body = memoryStream.ToArray().AsMemory();

    logger.LogInformation("Received event.");

    var result = await cloudEventHandler.Handle(body);

    if (result.IsFailure)
    {
        logger.LogInformation(result.Error);
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return;
    }

    logger.LogInformation("Event handled successfully.");
    context.Response.StatusCode = (int)HttpStatusCode.OK;
});

app.Run();