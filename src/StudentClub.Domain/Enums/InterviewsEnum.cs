using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Domain.Enums
{
    public enum InterviewStatus
    {
        Registered = 0,
        CheckedIn = 1,
        Interviewing = 2,
        Done = 3,
        NoShow = 4,
        Cancelled = 5
    }

    public enum InterviewResult
    {
        Pending = 0,
        Pass = 1,
        Fail = 2
    }

    public enum ApplicationType
    {
        Online = 0,
        WalkIn = 1
    }
}
