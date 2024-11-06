namespace ACS.HandleAdvancedMessagingEvents;

public class AdvancedMessage
{
    public string Content { get; set; }
    public string ChannelType { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public DateTime ReceivedTimestamp { get; set; }
}