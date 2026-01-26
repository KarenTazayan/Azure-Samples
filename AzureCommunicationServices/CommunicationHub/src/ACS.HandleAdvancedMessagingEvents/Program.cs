// Quickstart: Handle Advanced Messaging events
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/advanced-messaging/whatsapp/handle-advanced-messaging-events

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ACS.Chat;
using ACS.HandleAdvancedMessagingEvents;
using Azure;
using Azure.Communication.Messages;
using CloudNative.CloudEvents.SystemTextJson;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

Console.Title = "ACS.HandleAdvancedMessagingEvents";

var builder = WebApplication.CreateBuilder(args);
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

// Your unique Communication Services endpoint
var acsEndpointUrl = CurrentCredentials.AcsCredential.AcsEndpointUrl;
// Your unique ACS access token
// WhatsApp User Access Token
var userAccessTokenForChat = CurrentCredentials.WhatsAppUser.UserAccessToken;
var acsChatThreadId = CurrentCredentials.AcsChatThreadId;

var microsoftTeamsChatInteroperability =
    new AcsChatManager(acsEndpointUrl, userAccessTokenForChat);
microsoftTeamsChatInteroperability.ChangeChatThreadId(acsChatThreadId);

app.Map("/handler/event-grid", async (HttpContext context, ILogger<Program> logger) =>
{
    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    var body = await reader.ReadToEndAsync();
    logger.LogInformation("Received event.");

    // Deserialize the JSON string into a CloudEvent
    var formatter = new JsonEventFormatter();
    var cloudEvent = formatter.DecodeStructuredModeMessage(Encoding.UTF8.GetBytes(body).AsMemory(),
        null, null);

    logger.LogInformation($"Event Type: {cloudEvent.Type}");

    if (cloudEvent.Type == "Microsoft.Communication.AdvancedMessageReceived")
    {
        var json = cloudEvent.Data.ToString().Replace("{{", "{").Replace("}}", "}");
        var advancedMessage = JsonSerializer.Deserialize<AdvancedMessage>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        Console.WriteLine($"Received message: {advancedMessage.Content} from chat {advancedMessage.From}");
        await microsoftTeamsChatInteroperability.SendMessageToChatThreadAsync(advancedMessage.Content);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        return;
    }

    if (cloudEvent.Type == "Microsoft.Communication.ChatMessageReceivedInThread")
    {
        var json = cloudEvent.Data.ToString(); //.Replace("{{", "{").Replace("}}", "}");
        var advancedMessage = JsonSerializer.Deserialize<ChatMessageInThread>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var messageText = advancedMessage.MessageBody.Replace("<p>", "").Replace("</p>", "");

        if (CurrentCredentials.AcsUsers.Single(u => u.Type == "WhatsApp User").UserId !=
            advancedMessage.SenderCommunicationIdentifier.RawId)
        {
            // "ACS.WhatsApp.Business";

            var connectionString = CurrentCredentials.AcsConnectionString;

            // Instantiate the client
            var notificationMessagesClient = new NotificationMessagesClient(connectionString);

            // Your channel registration ID GUID
            var channelRegistrationId = new Guid(CurrentCredentials.WhatsAppChannelRegistrationId);
            var recipientList = new List<string> { CurrentCredentials.WhatsAppRecipient };

            // Send a text message
            var textContent = new TextNotificationContent(channelRegistrationId, recipientList, messageText);
            Response<SendMessageResult> sendTextMessageResult =
                await notificationMessagesClient.SendAsync(textContent);
        }

        if (CurrentCredentials.AcsUsers.Single(u => u.Type == "Telegram User").UserId !=
            advancedMessage.SenderCommunicationIdentifier.RawId)
        {
            var botToken = CurrentCredentials.TelegramBotToken;
            var botClient = new TelegramBotClient(botToken);

            var response= await botClient.SendMessage(chatId: "7849784244", text: messageText);
        }

        //if (CurrentCredentials.AcsUsers.Single(u => u.Type == "Mattermost User").UserId !=
        //    advancedMessage.SenderCommunicationIdentifier.RawId)
        //{
        //    var mattermostBaseUrl = "https://xxx.westeurope.cloudapp.azure.com";
        //    var accessToken = "";
        //    var channelId = "";

        //    await MattermostApi.SendMessageToMattermost(mattermostBaseUrl, accessToken, channelId, messageText);
        //}

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        return;
    }

    context.Response.StatusCode = (int)HttpStatusCode.OK;
});

app.MapGet("/", () => "Working...");

app.Run();