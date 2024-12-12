namespace ACS.HandleAdvancedMessagingEvents;

public class SenderCommunicationIdentifier
{
    public string Kind { get; set; }
    public string RawId { get; set; }
    public MicrosoftTeamsUser MicrosoftTeamsUser { get; set; }
}