using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum OutBoxStatus
    {
        Pending = 1,
        Processing = 2,
        Processed = 3,
        Failed = 4,
    }
}
