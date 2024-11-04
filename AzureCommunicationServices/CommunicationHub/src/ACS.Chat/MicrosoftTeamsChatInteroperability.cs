using Azure.Communication;
using Azure.Communication.Chat;

namespace ACS.Chat;

internal class MicrosoftTeamsChatInteroperability
{
    private readonly ChatClient _chatClient;
    private string _chatThreadId = string.Empty;

    public MicrosoftTeamsChatInteroperability(string acsEndpointUrl, string userAccessTokenForChat)
    {
        var acsEndpoint = new Uri(acsEndpointUrl);
        var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
        _chatClient = new ChatClient(acsEndpoint, communicationTokenCredential);
    }

    public void ChangeChatThreadId(string newChatThreadId)
    {
        _chatThreadId = newChatThreadId;
    }

    public async Task<string> CreateChatThreadAsync(string topicTitle)
    {
        if(!string.IsNullOrWhiteSpace(_chatThreadId)) return _chatThreadId;

        var id = "8:acs:xxx";

        var chatParticipant = new ChatParticipant(identifier: new CommunicationUserIdentifier(id))
        {
            DisplayName = "John Smith"
        };

        CreateChatThreadResult createChatThreadResult = 
            await _chatClient.CreateChatThreadAsync(topicTitle, [chatParticipant]);
        _chatThreadId = createChatThreadResult.ChatThread.Id;
            
        return _chatThreadId;
    }

    public void AddUserToChatThread(string participantId, string displayName = "")
    {
        var teamUser = new CommunicationUserIdentifier(participantId);
        var teamUserChatParticipant = new ChatParticipant(teamUser);

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            teamUserChatParticipant.DisplayName = displayName;
        }

        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        chatThreadClient.AddParticipant(teamUserChatParticipant);
    }

    /// <summary>
    /// Add Microsoft Teams user to chat thread.
    /// </summary>
    /// <param name="participantId">GUID of user in Microsoft Entra ID Tenant.</param>
    /// <param name="displayName">Display name.</param>
    public void AddMicrosoftTeamsUserToChatThread(string participantId, string displayName = "")
    {
        var teamUser = new MicrosoftTeamsUserIdentifier(participantId);
        var teamUserChatParticipant = new ChatParticipant(teamUser);
            
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            teamUserChatParticipant.DisplayName = displayName;
        }

        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        chatThreadClient.AddParticipant(teamUserChatParticipant);
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

    public async Task ListParticipantsAsync()
    {
        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        var list = chatThreadClient.GetParticipantsAsync();
        await foreach (var item in list)
        {
            Console.WriteLine(item.User);
            Console.WriteLine(item.DisplayName);
        }
    }

    public async Task ListMessagesAsync()
    {
        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        var list = chatThreadClient.GetMessagesAsync();
        await foreach (var item in list)
        {
            Console.WriteLine(item.Id);
            Console.WriteLine(item.Sender);
            Console.WriteLine(item.Content.Message);
        }
    }
}