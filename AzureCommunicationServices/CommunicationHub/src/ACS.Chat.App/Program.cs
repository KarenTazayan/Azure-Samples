// Quickstart: Add a bot to your chat app
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/chat/quickstart-botframework-integration

using ACS.Chat;
using Azure.Communication;
using Azure.Communication.Chat;

// Bot ACS Id
const string botACSId = "";

// Your unique Communication Services endpoint
var acsEndpointUrl = CurrentCredentials.AcsCredential.AcsEndpointUrl;
// Your unique ACS access token
var userAccessTokenForChat = CurrentCredentials.AcsCredential.UserAccessTokenForChat;

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
        var microsoftTeamsChatInteroperability = new MicrosoftTeamsChatInteroperability(acsEndpointUrl, userAccessTokenForChat);
        await microsoftTeamsChatInteroperability.CreateChatThreadAsync("Microsoft Teams 1");
        microsoftTeamsChatInteroperability.AddMicrosoftTeamsUserToChatThread("f5dafc5a-5906-4a2c-a185-d487c184c1f3", "Adele Vance");

        while (true)
        {
            var newMessage = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newMessage))
            {
                Console.WriteLine("Please provide a message content.");
                continue;
            }

            await microsoftTeamsChatInteroperability.SendMessageToChatThreadAsync(newMessage);
        }
    }

    if (list.Any(t => t.Id.Equals(input)))
    {
        var microsoftTeamsChatInteroperability =
            new MicrosoftTeamsChatInteroperability(acsEndpointUrl, userAccessTokenForChat);
        microsoftTeamsChatInteroperability.ChangeChatThreadId(input);

        //microsoftTeamsChatInteroperability.AddUserToChatThread("8:acs:383d9c6a-d810-468d-ae7d-ae9fc100ca4c_00000023-c5f1-9d98-65f0-ad3a0d0056ee", "Mattermost User");

        var participants = await microsoftTeamsChatInteroperability.ListParticipantsAsync();
        foreach (var participant in participants)
        {
            Console.WriteLine($"{participant.User} {participant.DisplayName}");
        }

        //await microsoftTeamsChatInteroperability.ListMessagesAsync();
    }

    if (input == "exit")
    {
        break;
    }

    input = Console.ReadLine();
}