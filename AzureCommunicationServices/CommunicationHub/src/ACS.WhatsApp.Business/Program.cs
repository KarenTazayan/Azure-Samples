// Quickstart: Send WhatsApp messages using Advanced Messages
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/advanced-messaging/whatsapp/get-started?tabs=visual-studio%2Cconnection-string&pivots=programming-language-csharp

using ACS.Chat;
using Azure;
using Azure.Communication.Messages;

Console.Title = "ACS.Telegram.Bot";

var connectionString = CurrentCredentials.AcsConnectionString;

// Instantiate the client
var notificationMessagesClient = new NotificationMessagesClient(connectionString);

// Your channel registration ID GUID
var channelRegistrationId = new Guid(CurrentCredentials.WhatsAppChannelRegistrationId);
var recipientList = new List<string> { CurrentCredentials.WhatsAppRecipient };

// Send a text message
var messageText = "Hello from ACS!";
var textContent = new TextNotificationContent(channelRegistrationId, recipientList, messageText);
Response<SendMessageResult> sendTextMessageResult = await notificationMessagesClient.SendAsync(textContent);

PrintResult(sendTextMessageResult);
Console.WriteLine("Text message sent to my phoneNumber.\nPress any key to continue.\n");
Console.ReadKey();

return;

static void PrintResult(Response<SendMessageResult> result)
{
    Console.WriteLine($"Response: {result.GetRawResponse().Status} " +
                      $"({result.GetRawResponse().ReasonPhrase})");
    Console.WriteLine($"Date: " +
                      $"{result.GetRawResponse().Headers.First(header => header.Name == "Date").Value}");
    Console.WriteLine($"ClientRequestId: {result.GetRawResponse().ClientRequestId}");
    Console.WriteLine($"MS-CV: " +
                      $"{result.GetRawResponse().Headers.First(header => header.Name == "MS-CV").Value}");
    foreach (var receipts in result.Value.Receipts)
    {
        Console.WriteLine($"MessageId: {receipts.MessageId}");
    }
    Console.WriteLine($"\n");
}