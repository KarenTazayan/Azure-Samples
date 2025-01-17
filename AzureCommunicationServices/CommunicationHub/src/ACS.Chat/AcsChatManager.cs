using Azure.Communication;
using Azure.Communication.Chat;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ACS.Chat;

public class AcsChatManager
{
    private readonly ChatClient _chatClient;
    private string _chatThreadId = string.Empty;
    private readonly string _accessToken;

    public AcsChatManager(string acsEndpointUrl, string userAccessTokenForChat)
    {
        var acsEndpoint = new Uri(acsEndpointUrl);
        var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
        _chatClient = new ChatClient(acsEndpoint, communicationTokenCredential);
        _accessToken = userAccessTokenForChat;
    }

    private static string ExtractResourceIdFromToken(string token)
    {
        // Initialize the JsonWebTokenHandler
        var handler = new JsonWebTokenHandler();

        // Read the token as a JsonWebToken
        var jwtToken = handler.ReadJsonWebToken(token);

        // Retrieve the "resourceId" claim
        var resourceId = jwtToken.GetClaim("resourceId")?.Value;

        if (string.IsNullOrWhiteSpace(resourceId))
        {
            throw new InvalidOperationException("No value for claim resourceId.");
        }

        return resourceId;
    }

    private static string ExtractSkypeIdFromToken(string token)
    {
        // Initialize the JsonWebTokenHandler
        var handler = new JsonWebTokenHandler();

        // Read the token as a JsonWebToken
        var jwtToken = handler.ReadJsonWebToken(token);

        // Retrieve the "skypeid" claim
        var skypeId = jwtToken.GetClaim("skypeid")?.Value;

        return skypeId ?? "skypeid not found";
    }

    public void ChangeChatThreadId(string newChatThreadId)
    {
        _chatThreadId = newChatThreadId;
    }

    public async Task<string> CreateChatThreadAsync(string topicTitle)
    {
        if(!string.IsNullOrWhiteSpace(_chatThreadId)) return _chatThreadId;

        var id = $"8:{ExtractSkypeIdFromToken(_accessToken)}";

        var chatParticipant = new ChatParticipant(identifier: new CommunicationUserIdentifier(id))
        {
            DisplayName = "Topic Owner"
        };

        CreateChatThreadResult createChatThreadResult = 
            await _chatClient.CreateChatThreadAsync(topicTitle, [chatParticipant]);
        _chatThreadId = createChatThreadResult.ChatThread.Id;
            
        return _chatThreadId;
    }

    public void AddUserToChatThread(string displayName)
    {
        var participantId = $"8:acs:{ExtractResourceIdFromToken(_accessToken)}_{Guid.NewGuid()}";
        var user = new CommunicationUserIdentifier(participantId);
        var chatParticipant = new ChatParticipant(user);

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            chatParticipant.DisplayName = displayName;
        }

        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        chatThreadClient.AddParticipant(chatParticipant);
    }

    public void AddUserToChatThread(string participantId, string displayName)
    {
        var user = new CommunicationUserIdentifier(participantId);
        var chatParticipant = new ChatParticipant(user);

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            chatParticipant.DisplayName = displayName;
        }

        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        chatThreadClient.AddParticipant(chatParticipant);
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

    public async Task<List<(string User, string? DisplayName)>> ListParticipantsAsync()
    {
        var chatThreadClient = _chatClient.GetChatThreadClient(_chatThreadId);
        var participants = new List<(string User, string? DisplayName)>();
        var list = chatThreadClient.GetParticipantsAsync();

        await foreach (var item in list)
        {
            if (item != null)
            {
                participants.Add((item.User.ToString(), item.DisplayName)!);
            }
        }

        return participants;
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