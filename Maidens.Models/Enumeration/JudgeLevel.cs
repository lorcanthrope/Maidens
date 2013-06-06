using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public enum JudgeLevel
    {
        Invalid = -1,
        Inactive = 0,
        Trainee = 1,
        IrrPanelistB = 2,
        IrrPanelistA = 3,
        IrrChair = 4,
        RlvPanelistB = 5,
        RlvPanelistA = 6,
        RlvChairB = 7,
        RlvChairA = 8,
        TopChair = 9
    }
}
