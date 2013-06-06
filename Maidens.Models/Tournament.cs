using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class Tournament
    {
        #region Fields
        private const string _database = "database";
        private const string _name = "name";
        private const string _location = "location";
        private const string _logo = "logo";
        private const string _roundNumber = "round_number";
        private const string _numberOfRounds = "number_of_rounds";
        #endregion

        #region Properties
        public string Database { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int RoundNumber { get; set; }
        public int NumberOfRounds { get; set; }
        #endregion  
    
        #region Methods
        public static Tournament Parse(string line)
        {
            string[] array = line.Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            Tournament tournament = new Tournament();
            
            foreach (string s in array)
            {
                string[] segments = s.Split('|');
                if(segments.Length < 2) continue;

                switch (segments.First())
                {
                    case _database:
                        tournament.Database = segments[1];
                        break;
                    case _name:
                        tournament.Name = segments[1];
                        break;
                    case _location:
                        tournament.Location = segments[1];
                        break;
                    case _logo:
                        tournament.Logo = segments[1];
                        break;
                    case _roundNumber:
                        tournament.RoundNumber = int.Parse(segments[1]);
                        break;
                    case _numberOfRounds:
                        tournament.NumberOfRounds = int.Parse(segments[1]);
                        break;                        
                }
            }
            
            return tournament;
        }

        public void Update()
        {
            File.WriteAllText(Location, ToString());
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}|{1}{2}", _database, Database, Environment.NewLine);
            sb.AppendFormat("{0}|{1}{2}", _location, Location, Environment.NewLine);
            sb.AppendFormat("{0}|{1}{2}", _logo, Logo ?? string.Empty, Environment.NewLine);
            sb.AppendFormat("{0}|{1}{2}", _name, Name, Environment.NewLine);
            sb.AppendFormat("{0}|{1}{2}", _roundNumber, RoundNumber, Environment.NewLine);
            sb.AppendFormat("{0}|{1}{2}", _numberOfRounds, NumberOfRounds, Environment.NewLine);
            return sb.ToString();
        }
        #endregion
    }
}
