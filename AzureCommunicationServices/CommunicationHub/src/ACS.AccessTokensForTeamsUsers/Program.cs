// Quickstart: Set up and manage access tokens for Teams users
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/manage-teams-identity?pivots=programming-language-csharp

using ACS.AccessTokensForTeamsUsers;
using ACS.Chat;
using Azure.Communication.Identity;
using Microsoft.Identity.Client;

Console.WriteLine("Azure Communication Services - Teams Access Tokens Quickstart\n");

var accessTokensManager = new AccessTokensManager(CurrentCredentials.AcsConnectionString);

//var identity = await accessTokensManager.CreateUser();
//Console.WriteLine($"\nCreated an identity with ID: {identity.Id}");
//var token = await accessTokensManager.RefreshAnAccessToken(identity.Id);
//Console.WriteLine(token);
//return;

// WhatsApp User
const string whatsAppUserId = "8:acs:xxx_00000023-ab0c-29fe-0586-af3a0d003caa";
var whatsAppUserToken = await accessTokensManager.RefreshAnAccessToken(whatsAppUserId);
Console.WriteLine($"WhatsApp User: {whatsAppUserId}");
Console.WriteLine(whatsAppUserToken);
Console.WriteLine("\n");

// Telegram User
const string telegramUserId = "8:acs:xxx_00000023-ab22-8667-f5f4-ad3a0d001344";
var telegramUserToken = await accessTokensManager.RefreshAnAccessToken(telegramUserId);
Console.WriteLine($"Telegram User: {telegramUserId}");
Console.WriteLine(telegramUserToken);
Console.WriteLine("\n");

// Mattermost User
const string mattermostUserId = "8:acs:xxx_00000023-c5f1-9d98-65f0-ad3a0d0056ee";
var mattermostUserToken = await accessTokensManager.RefreshAnAccessToken(mattermostUserId);
Console.WriteLine($"Mattermost User: {mattermostUserId}");
Console.WriteLine(mattermostUserToken);
Console.WriteLine("\n");

// Topic Owner
const string topicOwnerUserId = "8:acs:xxx_00000023-aacc-151d-c187-af3a0d0029c9";
var topicOwnerUserToken = await accessTokensManager.RefreshAnAccessToken(topicOwnerUserId);
Console.WriteLine($"Topic Owner: {topicOwnerUserId}");
Console.WriteLine(topicOwnerUserToken);
Console.WriteLine("\n");

return;
List<string> scopes =
[
    "https://auth.msft.communication.azure.com/Teams.ManageCalls",
    "https://auth.msft.communication.azure.com/Teams.ManageChats"
];

// This code demonstrates how to fetch an AAD client ID and tenant ID 
// from an environment variable.
var appId = "";
var tenantId = "";
var authority = $"https://login.microsoftonline.com/{tenantId}";
var redirectUri = "http://localhost";

// Create an instance of PublicClientApplication
var aadClient = PublicClientApplicationBuilder
    .Create(appId)
    .WithAuthority(authority)
    .WithRedirectUri(redirectUri)
    .Build();

// Retrieve the AAD token and object ID of a Teams user
var result = await aadClient
    .AcquireTokenInteractive(scopes)
    .ExecuteAsync();
var teamsUserAadToken = result.AccessToken;
var userObjectId = result.UniqueId;

Console.WriteLine(teamsUserAadToken);

// This code demonstrates how to fetch your connection string
// from an environment variable.
// COMMUNICATION_SERVICES_CONNECTION_STRING
var connectionString = "endpoint=https://xxx.communication.azure.com/;accesskey=";
var client = new CommunicationIdentityClient(connectionString);

var options = new GetTokenForTeamsUserOptions(teamsUserAadToken, appId, userObjectId);
var accessToken = await client.GetTokenForTeamsUserAsync(options);
Console.WriteLine($"Token: {accessToken.Value.Token}");