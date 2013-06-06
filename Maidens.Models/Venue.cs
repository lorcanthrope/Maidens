using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Venue
    {
        #region Properties
        public Guid VenueId { get; set; }
        public string Name { get; set; }
        public bool SpecialNeedsVenue { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Construction
        public Venue()
        {
            Active = true;
            VenueId = Guid.NewGuid();
        }
        #endregion
    }
}
