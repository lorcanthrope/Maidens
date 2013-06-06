using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Helpers
{
    public class NonQueryResult
    {
        public string CommandText { get; set; }
        public bool Completed { get; set; }
        public bool EnforceForeignKeys { get; set; }
        public Exception Exception { get; set; }
    }
}
