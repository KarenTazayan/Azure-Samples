// Quickstart: Add a bot to your chat app
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/chat/quickstart-botframework-integration

using ACS.Chat;
using Azure.Communication;
using Azure.Communication.Chat;

string threadId = "";

// Your unique Communication Services endpoint
const string acsEndpointUrl = "https://xxx.communication.azure.com/";
// Your unique ACS access token
const string userAccessTokenForChat = "";

// Bot ACS Id
const string botACSId = "8:acs:xxx";

// List chat threads.
var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
var chatClient = new ChatClient(new Uri(acsEndpointUrl), communicationTokenCredential);
var list = chatClient.GetChatThreads();
foreach (var item in list)
{
    Console.WriteLine(item.Id);
}

var microsoftTeamsChatInteroperability = new MicrosoftTeamsChatInteroperability(acsEndpointUrl, userAccessTokenForChat);
microsoftTeamsChatInteroperability.ChangeChatThreadId("");
microsoftTeamsChatInteroperability.AddMicrosoftTeamsUserToChatThread("f5dafc5a-5906-4a2c-a185-d487c184c1f3", "Adele Vance");

var id = "";
microsoftTeamsChatInteroperability.AddUserToChatThread(id, "Alex Smith");

await microsoftTeamsChatInteroperability.ListParticipantsAsync();
await microsoftTeamsChatInteroperability.ListMessagesAsync();
return;

await microsoftTeamsChatInteroperability.CreateChatThreadAsync("Microsoft Teams 5");

// Adele Vance
microsoftTeamsChatInteroperability.AddMicrosoftTeamsUserToChatThread("f5dafc5a-5906-4a2c-a185-d487c184c1f3");
await microsoftTeamsChatInteroperability.ListParticipantsAsync();

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