using Pgvector;

namespace Domain.Entities;

public class TextEmbedding : BaseEntity
{
    public Guid EmotionEntryId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string? ModelVersion { get; set; }
    public Vector Embedding { get; set; } = null!;

    public EmotionEntry EmotionEntry { get; set; } = null!;
}
