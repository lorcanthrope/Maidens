using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maidens.Models
{
    public class SpeakerDraw
    {
        public Guid DrawId { get; set; }
        public Guid SpeakerId { get; set; }
        public Guid DebateId { get; set; }
        public Position Position { get; set; }
        public Result Result { get; set; }

        private int _speakerPoints { get; set; }

        public int SpeakerPoints
        {
            get
            {
                if (_speakerPoints < 50) return 50;
                if (_speakerPoints > 100) return 100;
                return _speakerPoints;
            }
            set
            {
                if (value < 50) _speakerPoints = 50;
                if (value > 100) _speakerPoints = 100;
                _speakerPoints = value;
            }
        }

        public SpeakerDraw()
        {
            DrawId = Guid.NewGuid();
        }
    }
}
