// Quickstart: Set up and manage access tokens for Teams users
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/manage-teams-identity?pivots=programming-language-csharp

using ACS.AccessTokensForTeamsUsers;
using ACS.Chat;

Console.WriteLine("Azure Communication Services - Teams Access Tokens Quickstart\n");

var accessTokensManager = new AccessTokensManager(CurrentCredentials.AcsConnectionString);
await RefreshAccessTokenList(accessTokensManager);

return;

async Task RefreshAccessTokenList(AccessTokensManager accessTokensManager)
{

    // WhatsApp User
    var whatsAppUserId = CurrentCredentials.WhatsAppUser.AcsUser.UserId;
    var whatsAppUserToken = await accessTokensManager.RefreshAnAccessToken(whatsAppUserId);
    Console.WriteLine($"WhatsApp User: {whatsAppUserId}");
    Console.WriteLine(whatsAppUserToken);
    Console.WriteLine("\n");

    // Telegram User
    var telegramUserId = CurrentCredentials.TelegramUser.AcsUser.UserId;
    var telegramUserToken = await accessTokensManager.RefreshAnAccessToken(telegramUserId);
    Console.WriteLine($"Telegram User: {telegramUserId}");
    Console.WriteLine(telegramUserToken);
    Console.WriteLine("\n");

    // Mattermost User
    var mattermostUserId = CurrentCredentials.MattermostUser.AcsUser.UserId;
    var mattermostUserToken = await accessTokensManager.RefreshAnAccessToken(mattermostUserId);
    Console.WriteLine($"Mattermost User: {mattermostUserId}");
    Console.WriteLine(mattermostUserToken);
    Console.WriteLine("\n");

    // Topic Owner
    var topicOwnerUserId = CurrentCredentials.TopicOwner.AcsUser.UserId;
    var topicOwnerUserToken = await accessTokensManager.RefreshAnAccessToken(topicOwnerUserId);
    Console.WriteLine($"Topic Owner: {topicOwnerUserId}");
    Console.WriteLine(topicOwnerUserToken);
    Console.WriteLine("\n");
}