namespace ACS.Chat;

public class AcsUser(string userId, string displayName, string type)
{
    public string UserId { get; private set; } = userId;

    public string DisplayName { get; private set; } = displayName;

    public string Type { get; private set; } = type;
}