using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Speaker
    {
        #region Properties
        public Guid SpeakerId { get; set; }
        public string Name { get; set; }
        public Guid InstitutionId { get; set; }
        public bool SpecialNeeds { get; set; }
        public List<SpeakerDraw> Draws { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Construction
        public Speaker()
        {
            SpeakerId = Guid.NewGuid();
            Active = true;
        }
        #endregion       

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is Speaker)) return false;

            Speaker s = (Speaker) obj;

            return (s.SpeakerId == SpeakerId)
                   && (s.Name == Name)
                   && (s.InstitutionId == InstitutionId)
                   && (s.SpecialNeeds == SpecialNeeds);
        }

        public override int GetHashCode()
        {
            return SpeakerId.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

    }
}
