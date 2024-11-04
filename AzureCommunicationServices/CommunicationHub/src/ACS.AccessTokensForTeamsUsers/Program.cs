// Quickstart: Set up and manage access tokens for Teams users
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/manage-teams-identity?pivots=programming-language-csharp

using Azure.Communication.Identity;
using Microsoft.Identity.Client;

Console.WriteLine("Azure Communication Services - Teams Access Tokens Quickstart");

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

List<string> scopes =
[
    "https://auth.msft.communication.azure.com/Teams.ManageCalls",
    "https://auth.msft.communication.azure.com/Teams.ManageChats"
];

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