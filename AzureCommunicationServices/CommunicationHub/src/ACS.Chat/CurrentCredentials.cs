namespace ACS.Chat;

public class CurrentCredentials
{
    public static readonly AcsCredential AcsCredential =
        new("https://xxx.communication.azure.com/",
            ""
        );

    public static readonly string AcsChatThreadId = "xxx";

    public static readonly string TelegramBotToken = "xxx";

    public static readonly string AcsConnectionString = $"endpoint={AcsCredential.AcsEndpointUrl};accesskey={AcsCredential.Key}";

    public static readonly string WhatsAppChannelRegistrationId = "xxx";

    public static readonly string WhatsAppRecipient = "+xxx91";

    public static readonly AcsUserWithToken WhatsAppUser = new(
        new AcsUser("8:acs:xxx_00000025-0e12-b194-49a1-473a0d00c9f9",
            "WhatsApp User", "WhatsApp User"),
        ""
        );

    public static readonly AcsUserWithToken TelegramUser = new(
        new AcsUser("8:acs:xxx_00000025-0e14-edd8-6ba8-473a0d000992",
            "Telegram User", "Telegram User"),
        ""
    );

    public static readonly AcsUserWithToken MattermostUser = new(
        new AcsUser("8:acs:xxx_00000025-0e15-1f34-6ba8-473a0d0009a2",
                "Mattermost User", "Mattermost User"),
        ""
    );

    public static readonly AcsUserWithToken TopicOwner = new(
        new AcsUser("8:acs:xxx_00000025-0e16-59d9-6ba8-473a0d000a3c",
            "Topic Owner", "Topic Owner"),
        ""
    );

    public static readonly List<AcsUser> AcsUsers =
    [
        WhatsAppUser.AcsUser, TelegramUser.AcsUser, MattermostUser.AcsUser
    ];
}