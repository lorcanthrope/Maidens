using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Maidens.Models;

namespace Maidens.Helpers
{
    public static class HtmlHelper
    {
        #region CreateDisplay
        public static void CreateDisplay(Tournament tournament, DataContext context, Guid roundId)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            Round round = helper.GetRound(roundId).Result;

            List<Debate> debates = helper.GetDebates(roundId).Result;
            
            List<SpeakerDraw> speakerDraws = new List<SpeakerDraw>();
            List<JudgeDraw> judgeDraws = new List<JudgeDraw>();

            foreach (Debate debate in debates)
            {
                speakerDraws.AddRange(helper.GetSpeakerDrawsByDebate(debate.DebateId).Result);
                judgeDraws.AddRange(helper.GetJudgeDrawsByDebate(debate.DebateId).Result);
            }

            List<Speaker> speakers = new List<Speaker>();
            speakers.AddRange(context.Speakers);
            speakers = speakers.OrderBy(s => s.Name).ToList();

            List<Judge> judges = new List<Judge>();
            judges.AddRange(context.Judges);

            StringBuilder display = new StringBuilder();
            display.AppendLine("<html>");
            display.AppendLine("<head>");
            display.AppendFormat("<title>{0} - Round {1}</title>", tournament.Name, round.RoundNumber);
            display.AppendFormat("<link rel=\"stylesheet\" href=\"../lib/display.css\" type=\"text/css\"/>");
            display.AppendLine();
            display.AppendFormat("<script src=\"../lib/display.js\" type=\"text/javascript\"></script>");
            display.AppendLine();
            display.AppendLine("</head>");
            display.AppendLine("<body>");
            display.AppendFormat("<div class=\"header\">{0} - Round {1}</div>", tournament.Name, round.RoundNumber);
            display.AppendLine();
            display.AppendLine("<div class=\"blocker\"></div>");
            display.AppendLine("<table border=\"1\" cellpadding=\"20\">");
            display.AppendLine("<th class=\"table-header\">Speaker</th><th class=\"table-header\">Institution</th><th class=\"table-header\">Venue</th><th class=\"table-header\">Opening Government</th><th class=\"table-header\">Opening Opposition</th><th class=\"table-header\">Second Government</th><th class=\"table-header\">Second Opposition</th><th class=\"table-header\">Closing Government</th><th class=\"table-header\">Closing Opposition</th><th class=\"table-header\">Judges</th>");
            
            foreach(Speaker speaker in speakers)
            {
                SpeakerDraw speakerDraw = speakerDraws.First(sd => sd.SpeakerId.Equals(speaker.SpeakerId));                
                Debate debate = debates.First(d => d.DebateId.Equals(speakerDraw.DebateId));
                Venue venue = context.Venues.First(v => v.VenueId.Equals(debate.VenueId));
                display.AppendLine("<tr class=\"display-row-entry\">");

                display.AppendFormat("<td class=\"display-cell-entry\">{0}</td>", speaker.Name);
                display.AppendFormat("<td class=\"display-cell-entry\">{0}</td>", context.Institutions.First(i => i.InstitutionId.Equals(speaker.InstitutionId)).Name);
                display.AppendFormat("<td class=\"display-cell-entry\">{0}</td>", venue.Name);
                foreach (SpeakerDraw draw in speakerDraws.Where(sd => sd.DebateId.Equals(speakerDraw.DebateId)).OrderBy(o => o.Position))
                {
                    display.AppendFormat("<td class=\"display-cell-entry{0}\">{1}</td>", draw.DrawId.Equals(speakerDraw.DrawId) ? " selected" : string.Empty,  speakers.First(s => s.SpeakerId.Equals(draw.SpeakerId)).Name);
                }

                string[] judgeNames = judgeDraws.Where(jd => jd.DebateId.Equals(debate.DebateId)).OrderBy(j => j.Number).Select(jd => judges.First(j => jd.JudgeId.Equals(j.JudgeId)).Name).ToArray();
                display.AppendFormat("<td class=\"display-cell-entry\">{0}</td>", string.Join(",<br/>", judgeNames));
                
                display.AppendLine("</tr>");
            }

            display.AppendLine("</table>");

            display.AppendLine("</body>");
            display.AppendLine("</html>");

            string directory = Path.Combine(Path.GetDirectoryName(tournament.Location),tournament.Name, string.Format("Round {0}", round.RoundNumber));

            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = Path.Combine(directory, "draw.html");
            File.WriteAllText(filename, display.ToString());

        }
        #endregion
    }
}
