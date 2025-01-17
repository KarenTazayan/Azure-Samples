namespace ACS.HandleAdvancedMessagingEvents;

public class ChatMessageInThread
{
    public string MessageBody { get; set; }
    public string MessageId { get; set; }
    public string Type { get; set; }
    public long Version { get; set; }
    public string SenderDisplayName { get; set; }
    public SenderCommunicationIdentifier SenderCommunicationIdentifier { get; set; }
    public DateTime ComposeTime { get; set; }
    public string ThreadId { get; set; }
    public string TransactionId { get; set; }
}