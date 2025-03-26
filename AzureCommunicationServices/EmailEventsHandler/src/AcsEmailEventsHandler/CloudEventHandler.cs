using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace AcsEmailEventsHandler;

public class CloudEventHandler(ILogger<CloudEventHandler> logger, EmailEventsRepository repository)
{
    public async Task<Result> Handle(Memory<byte> body)
    {
        // Deserialize the JSON string into a CloudEvent
        var formatter = new JsonEventFormatter();
        var cloudEvent = formatter.DecodeStructuredModeMessage(body, null, null);

        if (!cloudEvent.IsValid)
        {
            logger.LogWarning("Invalid cloud event format.");
            return Result.Failure("Invalid cloud event format.");
        }

        switch (cloudEvent.Type)
        {
            case "Microsoft.Communication.EmailDeliveryReportReceived":
            case "Microsoft.Communication.EmailEngagementTrackingReportReceived":
                return await HandleEmailEvent(cloudEvent);

            default:
                logger.LogWarning("Cloud event type not supported.");
                return Result.Failure("Cloud event type not supported.");
        }
    }

    private async Task<Result> HandleEmailEvent(CloudEvent cloudEvent)
    {
        logger.LogInformation($"[{DateTime.Now.ToLongTimeString()}] Handling email event: {cloudEvent.Type}");
        var emailEvent = new EmailEvent(DateTime.UtcNow, cloudEvent.Type!, cloudEvent.Data!.ToString()!);

        try
        {
            await repository.Insert(emailEvent);
        }
        catch (Exception e)
        {
            const string message = "Failed to insert email event";
            logger.LogError(e, message);
            Result.Failure(message);
        }

        return Result.Success();
    }
}