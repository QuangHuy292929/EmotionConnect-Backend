namespace Application.DTOs.AI;

public class AnalyzeResponseDto
{
    public string Text { get; set; } = string.Empty;
    public EmotionPredictionDto? TopEmotion { get; set; }
    public List<EmotionPredictionDto> AllEmotions { get; set; } = new();
    public IReadOnlyList<float> Vector { get; set; } = Array.Empty<float>();
}
