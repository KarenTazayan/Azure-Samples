namespace AcsEmailEventsHandler;

public record EmailEvent(DateTime EventDateTime, string EventType, string EventPayload);