using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Debate
    {
        #region Properties
        public Guid DebateId { get; set; }
        public Guid RoundId { get; set; }
        public Guid VenueId { get; set; }
        #endregion

        #region Construction
        public Debate()
        {
            DebateId = Guid.NewGuid();
        }
        #endregion
    }
}
