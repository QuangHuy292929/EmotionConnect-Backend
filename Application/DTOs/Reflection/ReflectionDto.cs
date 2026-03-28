namespace Application.DTOs.Reflection;

public class ReflectionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public Guid? EmotionEntryId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MoodAfter { get; set; }
    public int? HelpfulScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
