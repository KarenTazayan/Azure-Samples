using System.Web;

namespace ACS.Mattermost.Api;

public class CommandPayloadParser
{
    public static CommandPayload Parse(string payload)
    {
        var queryParams = HttpUtility.ParseQueryString(payload);
            
        return new CommandPayload
        {
            ChannelId = queryParams["channel_id"],
            ChannelName = queryParams["channel_name"],
            Command = queryParams["command"],
            ResponseUrl = queryParams["response_url"],
            TeamDomain = queryParams["team_domain"],
            TeamId = queryParams["team_id"],
            Text = queryParams["text"],
            Token = queryParams["token"],
            TriggerId = queryParams["trigger_id"],
            UserId = queryParams["user_id"],
            UserName = queryParams["user_name"]
        };
    }
}