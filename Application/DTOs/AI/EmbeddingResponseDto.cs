namespace Application.DTOs.AI;

public class EmbeddingResponseDto
{
    public string Text { get; set; } = string.Empty;
    public IReadOnlyList<float> Vector { get; set; } = Array.Empty<float>();
}
