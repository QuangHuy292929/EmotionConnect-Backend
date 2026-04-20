using Application.DTOs.CheckIn;
using Domain.Entities;
using System.Diagnostics.SymbolStore;

namespace Infracstructure.Mappers;

public static class CheckInSessionMapper
{
    public static CheckInSessionDto ToDto(this CheckInSession session)
    {
        return new CheckInSessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            EmotionEntryId = session.EmotionEntryId,
            Status = session.Status.ToString(),
            CurrentStep = session.CurrentStep.ToString(),
            InputMode = session.InputMode.ToString(),

            EmotionAnswer = session.EmotionAnswer,
            IssueAnswer = session.IssueAnswer,
            DeepDiveAnswer = session.DeepDiveAnswer,

            GeneratedSummary = session.GeneratedSummary,
            EditedSummmary = session.EditedSummary,
            ConfirmedSummary = session.ConfirmedSummary,

            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            CompletedAt = session.CompletedAt,
            CancelledAt = session.CancelledAt
        };
    }

    public static CheckInStartResponseDto ToStartResponseDto(this CheckInSession session, string firstQuestion)
    {
        return new CheckInStartResponseDto
        {
            SessionId = session.Id,
            CurrentStep = session.CurrentStep.ToString(),
            InputMode = session.InputMode.ToString(),
            Status = session.Status.ToString(),
            FirstQuestion = firstQuestion
        };
    }

    public static CheckInStepResponseDto ToStepResponseDto(this CheckInSession session, string? nextQuestion = null)
    {
        return new CheckInStepResponseDto
        {
            SessionId = session.Id,
            CurrentStep = session.CurrentStep.ToString(),
            Status = session.Status.ToString(),
            NextQuestion = nextQuestion,
            IsCompleted = session.Status == Domain.Enums.CheckInStatus.Completed,
            IsAwaitingConfirmation = session.Status == Domain.Enums.CheckInStatus.AwaitingConfirmation,
            Summary = session.GeneratedSummary
        };
    }

    public static List<CheckInSessionDto> ToListDto(this IEnumerable<CheckInSession> sessions)
    {
        return sessions.Select(ToDto).ToList();
    }
}
