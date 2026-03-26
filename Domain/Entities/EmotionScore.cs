namespace Domain.Entities;

public class EmotionScore : BaseEntity
{
    public Guid EmotionEntryId { get; set; }
    public string EmotionLabel { get; set; } = string.Empty;
    public decimal Score { get; set; }

    public EmotionEntry EmotionEntry { get; set; } = null!;
}
