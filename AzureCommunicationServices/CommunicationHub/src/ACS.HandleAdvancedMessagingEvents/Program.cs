// Quickstart: Handle Advanced Messaging events
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/advanced-messaging/whatsapp/handle-advanced-messaging-events

using ACS.Chat;
using ACS.HandleAdvancedMessagingEvents;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Net;
using System.Text;
using System.Text.Json;

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
    }

    context.Response.StatusCode = (int)HttpStatusCode.OK;
});

app.Run();