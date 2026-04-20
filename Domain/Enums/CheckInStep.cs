using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums;

public enum CheckInStep
{
    Step1Emotion = 1,
    Step2MainIssue = 2,
    Step3DeepDive = 3,
    SumaryGenerated = 4,
    AwaitingConfirmation = 5,
    Completed = 6
}
