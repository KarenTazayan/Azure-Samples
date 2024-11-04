using Azure.Communication;
using Azure.Communication.Chat;

namespace ACS.Chat;

internal class AzureBotServiceInteroperability
{
    private readonly string _botAcsId;
    private readonly ChatClient _chatClient;
    private string _chatThreadId = string.Empty;

    public AzureBotServiceInteroperability(string acsEndpointUrl, string userAccessTokenForChat, string botACSId)
    {
        _botAcsId = botACSId;
        var acsEndpoint = new Uri(acsEndpointUrl);
        var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
        _chatClient = new ChatClient(acsEndpoint, communicationTokenCredential);
    }

    public async Task<string> CreateChatThreadAsync(string topicTitle)
    {
        if (!string.IsNullOrWhiteSpace(_chatThreadId)) return _chatThreadId;

        var chatParticipant = new ChatParticipant(identifier: new CommunicationUserIdentifier(id: _botAcsId))
        {
            DisplayName = "My Azure Bot"
        };

        CreateChatThreadResult createChatThreadResult =
            await _chatClient.CreateChatThreadAsync(topicTitle, [chatParticipant]);
        _chatThreadId = createChatThreadResult.ChatThread.Id;

        return _chatThreadId;
    }
    public async Task<string> SendMessageToChatThreadAsync(string messageContent)
    {
        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        var sendChatMessageOptions = new SendChatMessageOptions()
        {
            Content = messageContent,
            MessageType = ChatMessageType.Text
        };

        SendChatMessageResult sendChatMessageResult = await chatThreadClient.SendMessageAsync(sendChatMessageOptions);
        return sendChatMessageResult.Id;
    }
}