// Quickstart: Handle Advanced Messaging events
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/advanced-messaging/whatsapp/handle-advanced-messaging-events

using ACS.Chat;
using ACS.HandleAdvancedMessagingEvents;
using Azure;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Communication.Messages;
using Telegram.Bot;

Console.Title = "ACS.HandleAdvancedMessagingEvents";

// Your unique Communication Services endpoint
var acsEndpointUrl = CurrentCredentials.AcsCredential.AcsEndpointUrl;
// Your unique ACS access token
// WhatsApp User Access Token
var userAccessTokenForChat = "";
var acsChatThreadId = CurrentCredentials.AcsChatThreadId;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Event Subscription Endpoint validation
app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.Headers.Add("Allow", "GET, POST");
        context.Response.Headers.Add("WebHook-Request-Origin", "*");
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        return;
    }

    await next.Invoke();
});

app.MapGet("/", () => "Working...");

var microsoftTeamsChatInteroperability =
    new MicrosoftTeamsChatInteroperability(acsEndpointUrl, userAccessTokenForChat);
microsoftTeamsChatInteroperability.ChangeChatThreadId(acsChatThreadId);

app.Map("/api/eventgrid", async (HttpContext context, ILogger<Program> logger) =>
{
    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    var body = await reader.ReadToEndAsync();
    logger.LogInformation("Received event.");

    // Deserialize the event grid event
    var events = JsonSerializer.Deserialize<List<EventGridEvent>>(body);
    foreach (var eventGridEvent in events)
    {
        logger.LogInformation($"Event Type: {eventGridEvent.EventType}");

        if (eventGridEvent.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
        {
            var validationEventData = eventGridEvent.Data.ToObjectFromJson<SubscriptionValidationEventData>();
            var responseData = new SubscriptionValidationResponse
            {
                ValidationResponse = validationEventData.ValidationCode
            };

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsJsonAsync(responseData);
            return;
        }

        // Handle other event types here

        if (eventGridEvent.EventType == "Microsoft.Communication.AdvancedMessageReceived")
        {
            var json = eventGridEvent.Data.ToString().Replace("{{", "{").Replace("}}", "}");
            var advancedMessage = JsonSerializer.Deserialize<AdvancedMessage>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            var participants = await microsoftTeamsChatInteroperability.ListParticipantsAsync();

            if (!participants.Any(p => advancedMessage.From.Equals(p.DisplayName)))
            {
                microsoftTeamsChatInteroperability.AddUserToChatThread(advancedMessage.From);
            }

            Console.WriteLine($"Received message: {advancedMessage.Content} from chat {advancedMessage.From}");
            await microsoftTeamsChatInteroperability.SendMessageToChatThreadAsync(advancedMessage.Content);

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return;
        }

        if (eventGridEvent.EventType == "Microsoft.Communication.ChatMessageReceivedInThread")
        {
            var json = eventGridEvent.Data.ToString();//.Replace("{{", "{").Replace("}}", "}");
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
                Response<SendMessageResult> sendTextMessageResult = await notificationMessagesClient.SendAsync(textContent);
            }

            if (CurrentCredentials.AcsUsers.Single(u => u.Type == "Telegram User").UserId !=
                advancedMessage.SenderCommunicationIdentifier.RawId)
            {
                var botToken = CurrentCredentials.TelegramBotToken;
                var botClient = new TelegramBotClient(botToken);

                await botClient.SendMessage(
                    chatId: "7849784244",
                    text: messageText);
            }

            if (CurrentCredentials.AcsUsers.Single(u => u.Type == "Mattermost User").UserId !=
                advancedMessage.SenderCommunicationIdentifier.RawId)
            {
                string mattermostBaseUrl = "https://mattermost-sample.westeurope.cloudapp.azure.com";
                string accessToken = "jqxpkmjrdinfur5k4fgnw9byyh";
                string channelId = "p9zp4t3bsjn8pe3st8sof7w31e";

                await MattermostApi.SendMessageToMattermost(mattermostBaseUrl, accessToken, channelId, messageText);
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return;
        }
    }

    context.Response.StatusCode = (int)HttpStatusCode.OK;
});

app.Run();