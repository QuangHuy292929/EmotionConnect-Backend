namespace Application.DTOs.Reflection;

public class CreateReflectionRequest
{
    public Guid RoomId { get; set; }
    public Guid? EmotionEntryId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MoodAfter { get; set; }
    public int? HelpfulScore { get; set; }
}
