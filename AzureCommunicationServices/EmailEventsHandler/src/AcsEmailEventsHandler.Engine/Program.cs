using CloudNative.CloudEvents.SystemTextJson;
using System.Net;
using System.Text;
using AcsEmailEventsHandler;
using AcsEmailEventsHandler.Engine;

var builder = WebApplication.CreateSlimBuilder(args);

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

builder.Services.AddScoped(services =>
{
    var logger = services.GetRequiredService<ILogger<EmailEventsRepository>>();
    return new EmailEventsRepository(azureSqlConnectionString, logger);
});

var app = builder.Build();

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
    EmailEventsRepository r, ILogger <Program> logger) =>
{
    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    var body = await reader.ReadToEndAsync();
    logger.LogInformation("Received event.");

    // Deserialize the JSON string into a CloudEvent
    var formatter = new JsonEventFormatter();
    var cloudEvent = formatter.DecodeStructuredModeMessage(Encoding.UTF8.GetBytes(body).AsMemory(),
        null, null);

    r.Insert(new EmailEvent(DateTime.UtcNow, cloudEvent.Type, cloudEvent.Data.ToString()));

    logger.LogInformation($"Event Type: {cloudEvent.Type}");
    context.Response.StatusCode = (int)HttpStatusCode.OK;
});

app.Run();