using ACS.Chat;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

Console.Title = "ACS.Telegram.Bot";

// Your unique Communication Services endpoint
var acsEndpointUrl = CurrentCredentials.AcsCredential.AcsEndpointUrl;

// Your unique ACS access token
// Telegram User Access Token
var userAccessTokenForChat = CurrentCredentials.TelegramUser.UserAccessToken;
var acsChatThreadId = CurrentCredentials.AcsChatThreadId;

var botToken = CurrentCredentials.TelegramBotToken;
var botClient = new TelegramBotClient(botToken);

// Start receiving messages
var cancellationTokenSource = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = [] // Receive all update types
};

var chatManager =
    new AcsChatManager(acsEndpointUrl, userAccessTokenForChat);
chatManager.ChangeChatThreadId(acsChatThreadId);

botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationTokenSource.Token);

Console.WriteLine("Bot is running. Press Enter to exit.");
Console.ReadLine();

// Stop receiving messages
cancellationTokenSource.Cancel();
return;

// Method to handle updates (messages from users)
async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
{
    // Ensure we handle only messages
    if (update is { Type: UpdateType.Message, Message.Text: not null })
    {
        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;
        var displayName = $"{update.Message.From?.FirstName} {update.Message.From?.LastName}";

        //var participants = await chatManager.ListParticipantsAsync();

        //if (!participants.Any(p => displayName.Equals(p.DisplayName)))
        //{
        //    chatManager.AddUserToChatThread(displayName);
        //}

        Console.WriteLine($"Received message: {messageText} from chat {chatId}");
        await chatManager.SendMessageToChatThreadAsync(messageText);

        // Reply to the message
        await telegramBotClient.SendMessage(
            chatId: chatId,
            text: $"You said: {messageText}",
            cancellationToken: cancellationToken
        );
    }
}

// Method to handle errors
Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"An error occurred: {exception.Message}");
    return Task.CompletedTask;
}