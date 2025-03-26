using AcsEmailEventsHandler;
using AcsEmailEventsHandler.WebApp.Common;
using AcsEmailEventsHandler.WebApp.Components;
using AcsEmailEventsHandler.WebApp.Home;
using Microsoft.SemanticKernel;
using MudBlazor;
using MudBlazor.Services;

Console.Title = nameof(AcsEmailEventsHandler.WebApp);

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Semantic Kernel
builder.Services.AddKernel().AddAzureOpenAIChatCompletion("gpt-4o-mini",
    "https://xxx.openai.azure.com/",
    "");

builder.Services.AddTransient<NlQueryParser>();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddScoped<INotificationService, NotificationService>();

var azureSqlConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(azureSqlConnectionString))
{
    throw new InvalidOperationException("Environment variable 'AZURE_SQL_CONNECTION_STRING' is missing.");
}

builder.Services.AddScoped(services =>
{
    var logger = services.GetRequiredService<ILogger<EmailEventsRepository>>();
    return new EmailEventsRepository(azureSqlConnectionString, logger);
});

// Home
builder.Services.AddScoped<HomePageViewModel>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();