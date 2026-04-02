namespace Infracstructure.Presence;

public class RedisPresenceOptions
{
    public const string SectionName = "RedisPresence";

    public string ConnectionString { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = "presence";
    public int LastActiveTtlMinutes { get; set; } = 30;
    public int ConnectionTtlHours { get; set; } = 12;
}
