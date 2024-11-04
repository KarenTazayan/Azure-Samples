// Quickstart: Handle Advanced Messaging events
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/advanced-messaging/whatsapp/handle-advanced-messaging-events

using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    if (context.Request.Method == HttpMethods.Options)
//    {
//        context.Response.Headers.Add("Allow", "GET, POST");
//        context.Response.Headers.Add("WebHook-Request-Origin", "*");
//        context.Response.StatusCode = (int)HttpStatusCode.OK;
//        return;
//    }
//    await next.Invoke();
//});

app.Map("/api/eventgrid", async (HttpContext context, ILogger<Program> logger) =>
{
    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    var body = await reader.ReadToEndAsync();
    logger.LogInformation($"Received event: {body}");

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
    }

    context.Response.StatusCode = (int)HttpStatusCode.OK;
});

app.Run();