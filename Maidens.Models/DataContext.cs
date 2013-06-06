using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
	public class DataContext
	{
        public List<Speaker> Speakers { get; set; }
        public List<Institution> Institutions { get; set; }
        public List<Judge> Judges { get; set; }
        public List<Venue> Venues { get; set; }        
	}
}
