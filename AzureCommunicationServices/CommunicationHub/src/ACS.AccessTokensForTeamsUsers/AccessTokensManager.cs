using Azure.Communication;
using Azure.Communication.Identity;

namespace ACS.AccessTokensForTeamsUsers;

internal class AccessTokensManager
{
    private readonly CommunicationIdentityClient _client;

    internal AccessTokensManager(string acsConnectionString)
    {
        _client = new CommunicationIdentityClient(acsConnectionString);
    }

    internal async Task<string> RefreshAnAccessToken(string identityId)
    {
        var identityToRefresh = new CommunicationUserIdentifier(identityId);
        var tokenResponse = await _client.GetTokenAsync(identityToRefresh, 
            scopes: [CommunicationTokenScope.VoIP, CommunicationTokenScope.Chat]);

        return tokenResponse.Value.Token;
    }

    internal async Task<CommunicationUserIdentifier> CreateUser()
    {
        var identityResponse = await _client.CreateUserAsync();
        var identity = identityResponse.Value;

        return identity;
    }
}