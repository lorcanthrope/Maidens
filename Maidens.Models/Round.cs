using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Round
    {
        #region Properties
        public Guid RoundId { get; set; }
        public int RoundNumber { get; set; }
        public string Motion { get; set; }
        public bool IsRandom { get; set; }
        #endregion

        #region Construction
        public Round()
        {
            RoundId = Guid.NewGuid();
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("Round {0}", RoundNumber);
        }
        #endregion
    }
}
