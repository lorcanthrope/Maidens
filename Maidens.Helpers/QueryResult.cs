using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Helpers
{
    public class QueryResult<T>
    {
        public T Result { get; set; }

        public string CommandText { get; set; }
        public bool Completed { get; set; }        
        public Exception Exception { get; set; }
    }
}
