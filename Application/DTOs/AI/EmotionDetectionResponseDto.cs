namespace Application.DTOs.AI;

public class EmotionDetectionResponseDto
{
    public string Text { get; set; } = string.Empty;
    public List<EmotionPredictionDto> Emotions { get; set; } = new();
}
