namespace Domain.Entities;

public class Reflection : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public Guid? EmotionEntryId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MoodAfter { get; set; }
    public int? HelpfulScore { get; set; }

    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public EmotionEntry? EmotionEntry { get; set; }
}
