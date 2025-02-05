using AcsEmailEventsHandler;
using AcsEmailEventsHandler.WebApp.Components;
using AcsEmailEventsHandler.WebApp.Home;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var azureSqlConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(azureSqlConnectionString))
{
    throw new InvalidOperationException("Environment variable 'AZURE_SQL_CONNECTION_STRING' is missing.");
}

builder.Services.AddScoped(_ => new EmailEventsRepository(azureSqlConnectionString));

// Home
builder.Services.AddScoped<HomePageViewModel>();

var app = builder.Build();

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
