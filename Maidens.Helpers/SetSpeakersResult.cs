using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Maidens.Models;

namespace Maidens.Helpers
{
    public class SetSpeakersResult
    {
        public List<SpeakerDraw> SpeakerDraws { get; set; }
        public bool Complete { get; set; }
    }
}
