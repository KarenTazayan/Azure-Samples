using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using WebAppForBot;
using WebAppForBot.Components;

// Provision and publish a bot
// https://learn.microsoft.com/en-us/azure/bot-service/provision-and-publish-a-bot

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WebLogService>();

builder.Services.AddMvc();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

// Create the Bot Adapter with error handling enabled.
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.UseStaticFiles();
app.UseWebSockets();
app.UseRouting();
app.UseAntiforgery();

app.MapDefaultControllerRoute();

app.MapGet("/", () => "Hello World!");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();