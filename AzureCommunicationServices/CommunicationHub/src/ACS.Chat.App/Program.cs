// Quickstart: Add a bot to your chat app
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/chat/quickstart-botframework-integration

// Enable interoperability in your Teams tenant
// https://learn.microsoft.com/en-us/azure/communication-services/concepts/interop/calling-chat

using ACS.Chat;
using Azure.Communication;
using Azure.Communication.Chat;

// Bot ACS Id
const string botACSId = "";

// Your unique Communication Services endpoint
var acsEndpointUrl = CurrentCredentials.AcsCredential.AcsEndpointUrl;
// Your unique ACS access token
var userAccessTokenForChat = CurrentCredentials.TopicOwner.UserAccessToken;

// List chat threads.
var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
var chatClient = new ChatClient(new Uri(acsEndpointUrl), communicationTokenCredential);
var list = chatClient.GetChatThreads();
foreach (var item in list)
{
    Console.WriteLine(item.Id);
}

var input = Console.ReadLine();

while (true)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        input = Console.ReadLine();
        continue;
    }

    if (input == "new")
    {
        var chatManager = new AcsChatManager(acsEndpointUrl, userAccessTokenForChat);
        await chatManager.CreateChatThreadAsync("Microsoft Teams 2");
        chatManager.AddMicrosoftTeamsUserToChatThread("f5dafc5a-5906-4a2c-a185-d487c184c1f3", 
            "Adele Vance");
        chatManager.AddUserToChatThread(CurrentCredentials.MattermostUser.AcsUser.UserId,
            CurrentCredentials.MattermostUser.AcsUser.DisplayName);

        chatManager.AddUserToChatThread(CurrentCredentials.WhatsAppUser.AcsUser.UserId,
            CurrentCredentials.WhatsAppUser.AcsUser.DisplayName);

        chatManager.AddUserToChatThread(CurrentCredentials.TelegramUser.AcsUser.UserId,
            CurrentCredentials.TelegramUser.AcsUser.DisplayName);

        while (true)
        {
            var newMessage = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newMessage))
            {
                Console.WriteLine("Please provide a message content.");
                continue;
            }

            await chatManager.SendMessageToChatThreadAsync(newMessage);
        }
    }

    if (list.Any(chatThreadItem => chatThreadItem.Id.Equals(input)))
    {
        var chatManager =
            new AcsChatManager(acsEndpointUrl, userAccessTokenForChat);
        chatManager.ChangeChatThreadId(input);

        //chatManager.AddMicrosoftTeamsUserToChatThread("f5dafc5a-5906-4a2c-a185-d487c184c1f3", "Adele Vance");
        //chatManager.AddUserToChatThread(CurrentCredentials.MattermostUser.AcsUser.UserId, 
        //    CurrentCredentials.MattermostUser.AcsUser.DisplayName);

        //chatManager.AddUserToChatThread(CurrentCredentials.WhatsAppUser.AcsUser.UserId, 
        //    CurrentCredentials.WhatsAppUser.AcsUser.DisplayName);

        //chatManager.AddUserToChatThread(CurrentCredentials.TelegramUser.AcsUser.UserId,
        //    CurrentCredentials.TelegramUser.AcsUser.DisplayName);

        var participants = await chatManager.ListParticipantsAsync();
        foreach (var participant in participants)
        {
            Console.WriteLine($"{participant.User} {participant.DisplayName}");
        }

        await chatManager.ListMessagesAsync();
    }

    if (input == "exit")
    {
        break;
    }

    input = Console.ReadLine();
}