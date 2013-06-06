using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Institution
    {
        #region Properties
        public Guid InstitutionId { get; set; }
        public string Name { get; set; }
        #endregion

        #region Construction
        public Institution()
        {
            InstitutionId = Guid.NewGuid();
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
