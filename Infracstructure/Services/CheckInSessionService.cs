using Application.DTOs.AI;
using Application.DTOs.CheckIn;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Mappers;

namespace Infracstructure.Services;

public class CheckInSessionService : ICheckInSessionService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly IMatchingService _matchingService;

    public CheckInSessionService(IUnitOfWork unitOfWork, IAiService aiService, IMatchingService matchingService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _matchingService = matchingService;
    }

    public async Task CancelAsync(Guid sessionId, Guid userId, CancellationToken ct = default)
    {
        var session = await GetOwnedSessionAsync(sessionId, userId, ct);

        if (session.Status == CheckInStatus.Completed)
            throw new ConflictException("Completed session cannot be cancelled.");

        session.Status = CheckInStatus.Cancelled;
        session.CancelledAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangeAsync(ct);
    }

    /* Hàm này có tác dụng kiểm tra và lưu dữ liệu từ session đã thu thập được
     * Triển khai theo flow: Kiểm tra request và xác thực người dùng -> Lấy final summary (nếu edited summary : false -> lấy từ generatedsummary
     * -> Dựa vào summary gọi đến AI service để analyze thu về top emotion(scroce, label) -> Tạo đối tượng emotion entry lưu vào context để lát commit vào db
     * -> Tạo score add vào context -> kiểm tra xem analysis có trả về vector -> tạo text embedding -> cập nhật lại session -> savechange() -> return) 
     */
    public async Task<CheckInCompletedDto> ConfirmAsync(Guid sessionId, Guid userId, ConfirmCheckInRequest request, CancellationToken ct = default)
    {
        if (request is null)
        {
            throw new BadRequestException("request is required");
        }

        var session = await GetOwnedSessionAsync(sessionId, userId, ct);
        if (session.Status is not CheckInStatus.AwaitingConfirmation)
        {
            throw new ConflictException("this session is not awaiting confirmation");
        }

        if (!request.IsConfirmed)
        {
            throw new BadRequestException("session was not confirmed");
        }

        var finalSummary = string.IsNullOrWhiteSpace(request.EditedSummary) ? session.GeneratedSummary : request.EditedSummary.Trim();

        if (string.IsNullOrWhiteSpace(finalSummary)) throw new BadRequestException("confirmed summary is required");

        var analysis = await _aiService.AnalyzeAsync(finalSummary, ct);

        var emotionEntry = new EmotionEntry
        {
            UserId = userId,
            SourceType = EmotionSourceType.CheckIn,
            RawText = finalSummary,
            NormalizedText = finalSummary,
            TopEmotion = analysis.TopEmotion?.Label,
            TopEmotionScore = analysis.TopEmotion is null ? null : Convert.ToDecimal(analysis.TopEmotion.Score),
            LanguageCode = "vi"
        };

        await _unitOfWork.EmotionRepository.AddEmotionEntryAsync(emotionEntry, ct);

        var emotionScores = analysis.AllEmotions.Select(x => new EmotionScore
        {
            EmotionEntryId = emotionEntry.Id,
            EmotionLabel = x.Label,
            Score = Convert.ToDecimal(x.Score)
        }).ToList();

        if (emotionScores.Count > 0)
        {
            await _unitOfWork.EmotionRepository.AddEmotionScoresAsync(emotionScores, ct);
        }

        if (analysis.Vector.Count > 0)
        {
            var embedding = new TextEmbedding
            {
                EmotionEntryId = emotionEntry.Id,
                ModelName = "chat-summary-emdding",
                ModelVersion = "1.0",
                Embedding = new Pgvector.Vector(analysis.Vector.ToArray())
            };

            await _unitOfWork.EmotionRepository.AddEmbeddingAsync(embedding, ct);
        }

        session.EditedSummary = string.IsNullOrWhiteSpace(request.EditedSummary) ? null : request.EditedSummary.Trim();
        session.ConfirmedSummary = finalSummary;
        session.EmotionEntryId = emotionEntry.Id;
        session.Status = CheckInStatus.Completed;
        session.CurrentStep = CheckInStep.Completed;
        session.CompletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangeAsync(ct);

        var matchingResult = await _matchingService.CreateMatchingAsync(emotionEntry.Id, userId, ct);

        return new CheckInCompletedDto
        {
            SessionId = session.Id,
            EmotionEntryId = emotionEntry.Id,
            ConfirmedSummary = finalSummary,
            TopEmotion = emotionEntry.TopEmotion,
            TopEmotionScore = emotionEntry.TopEmotionScore,
            AllEmotions = emotionScores.Select(x => x.ToDto()).ToList(),
            Vector = analysis.Vector.ToList(),
            MatchingRequestId = matchingResult.MatchingRequestId,
            Candidates = matchingResult.Candidates
        };
    }

    public async Task<CheckInSessionDto?> GetActiveSessionAsync(Guid userId, CancellationToken ct = default)
    {
        var activeSession = await _unitOfWork.CheckInSessionRepository.GetActiveByUserIdAsync(userId, ct);
        return activeSession is null
            ? null
            : MapSessionForConversationHydration(activeSession);
    }

    public async Task<CheckInSessionDto?> GetBySessionId(Guid sessionId, CancellationToken ct = default)
    {
        if(sessionId == Guid.Empty)
        {
            throw new BadRequestException("Session Id is required here");
        }

        var session = await _unitOfWork.CheckInSessionRepository.GetByIdAsync(sessionId, ct);
        return session is null ? null : MapSessionForConversationHydration(session);
    }

    public async Task<CheckInStartResponseDto> StartAsync(Guid userId, StartCheckInRequest request, CancellationToken ct = default)
    {
        if (request == null) throw new BadRequestException("Start Request cannot be null");

        var activeSession = await _unitOfWork.CheckInSessionRepository.GetActiveByUserIdAsync(userId, ct);
        if (activeSession is not null)
        {
            return activeSession.ToStartResponseDto(GetQuestionForStep(activeSession.CurrentStep));
        }

        if (!Enum.TryParse<CheckInInputMode>(request.InputMode, true, out var inputMode))
        {
            throw new ConflictException("Check in input mode is invalid");
        }

        var session = new CheckInSession
        {
            UserId = userId,
            InputMode = inputMode,
            Status = CheckInStatus.Started,
            CurrentStep = CheckInStep.Step1Emotion
        };

        await _unitOfWork.CheckInSessionRepository.AddAsync(session, ct);
        await _unitOfWork.SaveChangeAsync(ct);

        return session.ToStartResponseDto(GetQuestionForStep(session.CurrentStep));
    }

    public async Task<CheckInStepResponseDto> SubmitAnswerAsync(Guid sessionId, Guid userId, SubmitCheckInAnswerRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new BadRequestException("Request is required");
        if (string.IsNullOrEmpty(request.Content)) throw new BadRequestException("Content is required");

        var session = await GetOwnedSessionAsync(sessionId, userId, ct);
        if (session.Status is CheckInStatus.Completed or CheckInStatus.Cancelled)
        {
            throw new ConflictException("This check in session is no longer editable");
        }

        var content = request.Content.Trim();

        switch (session.CurrentStep)
        {
            case CheckInStep.Step1Emotion:
                session.EmotionAnswer = content;
                session.CurrentStep = CheckInStep.Step2MainIssue;
                session.Status = CheckInStatus.InProgress;
                break;
            case CheckInStep.Step2MainIssue:
                session.IssueAnswer = content;
                session.CurrentStep = CheckInStep.Step3DeepDive;
                session.Status = CheckInStatus.InProgress;
                break;
            case CheckInStep.Step3DeepDive:
                session.DeepDiveAnswer = content;
                session.GeneratedSummary = await GenerateSummaryAsync(session, ct);
                session.CurrentStep = CheckInStep.AwaitingConfirmation;
                session.Status = CheckInStatus.AwaitingConfirmation;
                break;
            default:
                throw new ConflictException("This session is not accepting answer");
        }

        await _unitOfWork.SaveChangeAsync(ct);

        if (session.Status is CheckInStatus.AwaitingConfirmation)
        {
            return session.ToStepResponseDto();
        }

        return session.ToStepResponseDto(GetQuestionForStep(session.CurrentStep));
    }

    public async Task<List<CheckInSessionDto>> GetMySessions(Guid userId, CancellationToken ct = default)
    {
        var result = await _unitOfWork.CheckInSessionRepository.GetMySessions(userId, ct);
        return result
            .Select(MapSessionForConversationHydration)
            .ToList();
    }


    private CheckInSessionDto MapSessionForConversationHydration(CheckInSession session)
    {
        return session.ToDto(
            currentQuestion: GetCurrentQuestion(session),
            emotionQuestion: ShouldIncludeEmotionQuestion(session) ? GetQuestionForStep(CheckInStep.Step1Emotion) : null,
            issueQuestion: ShouldIncludeIssueQuestion(session) ? GetQuestionForStep(CheckInStep.Step2MainIssue) : null,
            deepDiveQuestion: ShouldIncludeDeepDiveQuestion(session) ? GetQuestionForStep(CheckInStep.Step3DeepDive) : null,
            reviewQuestion: ShouldIncludeReviewQuestion(session) ? GetReviewQuestion() : null);
    }

    private static string GetQuestionForStep(CheckInStep step)
    {
        return step switch
        {
            CheckInStep.Step1Emotion => "Lúc này bạn đang cảm thấy như thế nào, hoặc điều gì đang nổi bật nhất với bạn?",
            CheckInStep.Step2MainIssue => "Có điều gì, sự kiện nào hoặc trải nghiệm nào đang ảnh hưởng bạn nhiều nhất lúc này?",
            CheckInStep.Step3DeepDive => "Nếu chia sẻ thêm một chút, phần nào của trải nghiệm đó đang để lại dấu ấn rõ nhất với bạn?",
            _ => string.Empty
        };
    }

    private static string GetReviewQuestion()
    {
        return "Mình đã tóm tắt lại như trên. Bạn thấy nội dung này đã phản ánh đúng điều mình muốn chia sẻ chưa?";
    }

    private static string? GetCurrentQuestion(CheckInSession session)
    {
        if (session.Status == CheckInStatus.AwaitingConfirmation)
        {
            return GetReviewQuestion();
        }

        if (session.Status is CheckInStatus.Completed or CheckInStatus.Cancelled)
        {
            return null;
        }

        return GetQuestionForStep(session.CurrentStep);
    }

    private static bool ShouldIncludeEmotionQuestion(CheckInSession session)
    {
        return !string.IsNullOrWhiteSpace(session.EmotionAnswer)
            || session.CurrentStep != CheckInStep.Step1Emotion
            || session.Status != CheckInStatus.Started;
    }

    private static bool ShouldIncludeIssueQuestion(CheckInSession session)
    {
        return !string.IsNullOrWhiteSpace(session.IssueAnswer)
            || session.CurrentStep == CheckInStep.Step3DeepDive
            || session.Status is CheckInStatus.AwaitingConfirmation or CheckInStatus.Completed;
    }

    private static bool ShouldIncludeDeepDiveQuestion(CheckInSession session)
    {
        return !string.IsNullOrWhiteSpace(session.DeepDiveAnswer)
            || session.Status is CheckInStatus.AwaitingConfirmation or CheckInStatus.Completed;
    }

    private static bool ShouldIncludeReviewQuestion(CheckInSession session)
    {
        return session.Status is CheckInStatus.AwaitingConfirmation or CheckInStatus.Completed
            || !string.IsNullOrWhiteSpace(session.GeneratedSummary);
    }

    private async Task<CheckInSession> GetOwnedSessionAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.CheckInSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
            throw new NotFoundException("Check-in session not found.");

        if (session.UserId != userId)
            throw new ForbiddenException("You do not have access to this check-in session.");

        return session;
    }

    private async Task<string> GenerateSummaryAsync(
    CheckInSession session,
    CancellationToken cancellationToken)
    {
        var rawSummary = BuildRawSummary(session);

        try
        {
            return await ClarifySummaryAsync(session, rawSummary, cancellationToken);
        }
        catch (ExternalServiceException)
        {
            return rawSummary;
        }
    }

    private async Task<string> ClarifySummaryAsync(
        CheckInSession session,
        string fallbackSummary,
        CancellationToken cancellationToken)
    {
        var clarified = await _aiService.ClarifySummaryAsync(
            new ClarifySummaryRequestDto
            {
                EmotionAnswer = session.EmotionAnswer ?? string.Empty,
                IssueAnswer = session.IssueAnswer ?? string.Empty,
                DeepDiveAnswer = session.DeepDiveAnswer ?? string.Empty
            },
            cancellationToken);

        return string.IsNullOrWhiteSpace(clarified.ClarifiedSummary)
            ? fallbackSummary
            : clarified.ClarifiedSummary.Trim();
    }

    private async Task<string> RewriteSummaryAsync(
        string summary,
        CancellationToken cancellationToken)
    {
        var rewritten = await _aiService.RewriteSummaryAsync(
            new RewriteSummaryRequestDto
            {
                Text = summary
            },
            cancellationToken);

        return string.IsNullOrWhiteSpace(rewritten.RewrittenText)
            ? summary
            : rewritten.RewrittenText.Trim();
    }

    private static string BuildRawSummary(CheckInSession session)
    {
        return
            $"Người dùng hiện cảm thấy {session.EmotionAnswer?.Trim()}. " +
            $"Trải nghiệm hoặc bối cảnh nổi bật mà họ đang chia sẻ là {session.IssueAnswer?.Trim()}. " +
            $"Điều họ mô tả rõ hơn là {session.DeepDiveAnswer?.Trim()}.";
    }

    
}

    


