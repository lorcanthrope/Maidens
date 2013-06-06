using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class JudgeDraw
    {
        #region Properties
        public Guid DrawId { get; set; }
        public Guid JudgeId { get; set; }
        public Guid DebateId { get; set; }
        public int Number{ get; set; }
        #endregion

        #region Construction
        public JudgeDraw()
        {
            DrawId = Guid.NewGuid();
        }
        #endregion
    }
}
