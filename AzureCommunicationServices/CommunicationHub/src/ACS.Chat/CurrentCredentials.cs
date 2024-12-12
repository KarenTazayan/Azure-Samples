namespace ACS.Chat;

public class CurrentCredentials
{
    public static readonly AcsCredential AcsCredential =
        new("https://xxx.communication.azure.com/",
            ""
        );

    public static readonly string AcsChatThreadId =
        "";

    public static readonly string TelegramBotToken = "";

    public static readonly string AcsConnectionString = $"endpoint={AcsCredential.AcsEndpointUrl};accesskey=xxx";

    public static readonly string WhatsAppChannelRegistrationId = "";

    public static readonly string WhatsAppRecipient = "+xxx91";

    public static readonly List<AcsUser> AcsUsers =
    [
        new("8:acs:xxx_00000023-ab0c-29fe-0586-af3a0d003caa",
            "WhatsApp User", "WhatsApp User"),

        new("8:acs:xxx_00000023-ab22-8667-f5f4-ad3a0d001344",
            "Telegram User", "Telegram User"),

        new("8:acs:xxx_00000023-c5f1-9d98-65f0-ad3a0d0056ee",
            "Mattermost User", "Mattermost User")
    ];
}