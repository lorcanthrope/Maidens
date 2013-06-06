using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Judge
    {
        #region Properties
        public Guid JudgeId { get; set; }
        public string Name { get; set; }
        public JudgeLevel Level { get; set; }
        public Guid? InstitutionId { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Construction
        public Judge()
        {
            Active = true;
            JudgeId = Guid.NewGuid();
            Level = JudgeLevel.RlvPanelistB;
        }
        #endregion

        #region Overrides

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
