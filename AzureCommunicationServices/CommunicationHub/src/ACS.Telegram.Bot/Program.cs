using ACS.Chat;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var threadId = "";
// Your unique Communication Services endpoint
var acsEndpointUrl = "https://xxx.communication.azure.com/";
// Your unique ACS access token
var userAccessTokenForChat =
    "";

string botToken = "";  // Replace with your bot token
var botClient = new TelegramBotClient(botToken);

// Start receiving messages
var cancellationTokenSource = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = [] // Receive all update types
};

var microsoftTeamsChatInteroperability =
    new MicrosoftTeamsChatInteroperability(acsEndpointUrl, userAccessTokenForChat);
microsoftTeamsChatInteroperability.ChangeChatThreadId(threadId);

botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationTokenSource.Token);

Console.WriteLine("Bot is running. Press Enter to exit.");
Console.ReadLine();

// Stop receiving messages
cancellationTokenSource.Cancel();
return;

// Method to handle updates (messages from users)
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Ensure we handle only messages
    if (update is { Type: UpdateType.Message, Message.Text: not null })
    {
        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;
        var displayName = $"{update.Message.From?.FirstName} {update.Message.From?.LastName}";

        var participants = await microsoftTeamsChatInteroperability.ListParticipantsAsync();

        if (!participants.Any(p => displayName.Equals(p.DisplayName)))
        {
            microsoftTeamsChatInteroperability.AddUserToChatThread(displayName);
        }

        Console.WriteLine($"Received message: {messageText} from chat {chatId}");
        await microsoftTeamsChatInteroperability.SendMessageToChatThreadAsync(messageText);

        // Reply to the message
        await botClient.SendMessage(
            chatId: chatId,
            text: $"You said: {messageText}",
            cancellationToken: cancellationToken
        );
    }
}

// Method to handle errors
Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"An error occurred: {exception.Message}");
    return Task.CompletedTask;
}