namespace Application.DTOs.Emotion;

public class CreateEmotionEntryRequest
{
    public Guid? CommunityId { get; set; }
    public Guid? RoomId { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string RawText { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = "vi";
}
