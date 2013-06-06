using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Helpers
{
    public static class TextHelper
    {
        #region EscapeSingleQuotes
        public static string EscapeSingleQuotes(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return input.Replace("'", "''");
        }
        #endregion
    }
}
