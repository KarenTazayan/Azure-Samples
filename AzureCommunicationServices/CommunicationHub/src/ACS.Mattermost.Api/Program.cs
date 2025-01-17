using ACS.Chat;
using ACS.Mattermost.Api;
using System.Text;
using System.Text.Json;

Console.Title = "ACS.Mattermost.Api";

// Your unique Communication Services endpoint
var acsEndpointUrl = CurrentCredentials.AcsCredential.AcsEndpointUrl;
// Your unique ACS access token
// Mattermost User Access Token
var userAccessTokenForChat = CurrentCredentials.MattermostUser.UserAccessToken;
var acsChatThreadId = CurrentCredentials.AcsChatThreadId;

var chatManager = new AcsChatManager(acsEndpointUrl, userAccessTokenForChat);
chatManager.ChangeChatThreadId(acsChatThreadId);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/", async (HttpContext context, ILogger<Program> logger) =>
{
    // TODO: Add MATTERMOST_TOKEN validation.
    // https://developers.mattermost.com/integrate/webhooks/outgoing/

    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    var json = await reader.ReadToEndAsync();
    logger.LogInformation(json);

    var mattermostEvent = JsonSerializer.Deserialize<MattermostEvent>(json);

    Console.WriteLine($"From: {mattermostEvent?.UserName} Message: {mattermostEvent?.Text}");

    await chatManager.SendMessageToChatThreadAsync(mattermostEvent.Text);

    context.Response.StatusCode = 200;
    await context.Response.CompleteAsync();
});

app.Run();