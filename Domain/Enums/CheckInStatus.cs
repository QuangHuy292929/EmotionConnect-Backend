using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums;
public enum CheckInStatus
{
    Started = 1,
    InProgress = 2,
    AwaitingConfirmation = 3,
    Confirmed = 4,
    Completed = 5,
    Cancelled = 6
}
