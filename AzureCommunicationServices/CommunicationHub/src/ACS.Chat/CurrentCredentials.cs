namespace ACS.Chat;

public class CurrentCredentials
{
    public static readonly AcsCredential AcsCredential =
        new("https://xxx.communication.azure.com/",
            ""
        );

    public static readonly string AcsChatThreadId = "";

    public static readonly string TelegramBotToken = "";

    public static readonly string AcsConnectionString = $"endpoint={AcsCredential.AcsEndpointUrl};accesskey={AcsCredential.Key}";

    public static readonly string WhatsAppChannelRegistrationId = "";

    public static readonly string WhatsAppRecipient = "+xxx91";

    public static readonly AcsUserWithToken WhatsAppUser = new(
        new AcsUser("8:acs:xxx",
            "WhatsApp User", "WhatsApp User"),
        ""
        );

    public static readonly AcsUserWithToken TelegramUser = new (
        new AcsUser("8:acs:xxx",
            "Telegram User", "Telegram User"),
        ""
    );

    public static readonly AcsUserWithToken MattermostUser = new (
        new AcsUser("8:acs:xxx",
                "Mattermost User", "Mattermost User"),
        ""
    );

    public static readonly AcsUserWithToken TopicOwner = new (
        new AcsUser("8:acs:xxx",
            "Topic Owner", "Topic Owner"),
        ""
    );

    public static readonly List<AcsUser> AcsUsers =
    [
        WhatsAppUser.AcsUser, TelegramUser.AcsUser, MattermostUser.AcsUser
    ];
}