using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Excel = Microsoft.Office.Interop.Excel;
using Powerpoint = Microsoft.Office.Interop.PowerPoint;
using Word = Microsoft.Office.Interop.Word;

using Maidens.Models;

namespace Maidens.Helpers
{
    public static class MicrosoftHelper
    {
        #region CreateBallots
        public static void CreateBallots(Tournament tournament, DataContext context, Guid roundId)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            Round round = helper.GetRound(roundId).Result;

            string motion = round.Motion;

            List<Debate> debates = helper.GetDebates(roundId).Result;

            string directory = Path.Combine(Path.GetDirectoryName(tournament.Location), tournament.Name, string.Format("Round {0}", round.RoundNumber));

            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string ballotsDirectory = Path.Combine(directory, "ballots");

            if(!Directory.Exists(ballotsDirectory))
            {
                Directory.CreateDirectory(ballotsDirectory);
            }

            foreach (Debate debate in debates)
            {
                List<SpeakerDraw> speakerDraws = helper.GetSpeakerDrawsByDebate(debate.DebateId).Result;
                List<JudgeDraw> judgeDraws = helper.GetJudgeDrawsByDebate(debate.DebateId).Result;

                Venue venue = context.Venues.First(v => v.VenueId.Equals(debate.VenueId));

                Word._Application wordApplication = new Word.Application(){Visible = false};
                wordApplication.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
                object filename = Path.Combine(Path.GetDirectoryName(tournament.Location), tournament.Name, "lib", "ballot.docx");
                Word._Document wordDocument = wordApplication.Documents.Open(ref filename);
                wordDocument.Activate();
                Word.Range content = wordDocument.Range();

                Dictionary<string, string> findReplaces = new Dictionary<string, string>();
                findReplaces.Add("#t", tournament.Name);
                findReplaces.Add("#v", venue.Name);
                string chairJudge = judgeDraws.OrderBy(j => j.Number).Select(jd => context.Judges.First(j => j.JudgeId.Equals(jd.JudgeId))).First().Name;
                findReplaces.Add("#chair", chairJudge);

                findReplaces.Add("#j", string.Join(", ", judgeDraws.OrderBy(j => j.Number).Select(a => context.Judges.First(j => j.JudgeId.Equals(a.JudgeId)).Name)));
                findReplaces.Add("#r", round.RoundNumber.ToString());
                findReplaces.Add("#m", motion);
                
                SpeakerDraw og = speakerDraws.First(sd => sd.Position.Equals(Position.OpeningGovernment));
                Speaker ogSpeaker = context.Speakers.First(sd => og.SpeakerId.Equals(sd.SpeakerId));
                findReplaces.Add("#og", context.Speakers.First(s => s.SpeakerId.Equals(og.SpeakerId)).Name);
                findReplaces.Add("#iog", context.Institutions.First(i => i.InstitutionId.Equals(ogSpeaker.InstitutionId)).Name);

                SpeakerDraw oo = speakerDraws.First(sd => sd.Position.Equals(Position.OpeningOpposition));
                Speaker ooSpeaker = context.Speakers.First(sd => oo.SpeakerId.Equals(sd.SpeakerId));
                findReplaces.Add("#oo", context.Speakers.First(s => s.SpeakerId.Equals(oo.SpeakerId)).Name);
                findReplaces.Add("#ioo", context.Institutions.First(i => i.InstitutionId.Equals(ooSpeaker.InstitutionId)).Name);

                SpeakerDraw sg = speakerDraws.First(sd => sd.Position.Equals(Position.SecondGovernment));
                Speaker sgSpeaker = context.Speakers.First(sd => sg.SpeakerId.Equals(sg.SpeakerId));
                findReplaces.Add("#sg", context.Speakers.First(s => s.SpeakerId.Equals(sg.SpeakerId)).Name);
                findReplaces.Add("#isg", context.Institutions.First(i => i.InstitutionId.Equals(sgSpeaker.InstitutionId)).Name);
                
                SpeakerDraw so = speakerDraws.First(sd => sd.Position.Equals(Position.SecondOpposition));
                Speaker soSpeaker = context.Speakers.First(sd => so.SpeakerId.Equals(sd.SpeakerId));
                findReplaces.Add("#so", context.Speakers.First(s => s.SpeakerId.Equals(so.SpeakerId)).Name);
                findReplaces.Add("#iso", context.Institutions.First(i => i.InstitutionId.Equals(soSpeaker.InstitutionId)).Name);

                SpeakerDraw cg = speakerDraws.First(sd => sd.Position.Equals(Position.ClosingGovernment));
                Speaker cgSpeaker = context.Speakers.First(sd => cg.SpeakerId.Equals(sd.SpeakerId));
                findReplaces.Add("#cg", context.Speakers.First(s => s.SpeakerId.Equals(cg.SpeakerId)).Name);
                findReplaces.Add("#icg", context.Institutions.First(i => i.InstitutionId.Equals(cgSpeaker.InstitutionId)).Name);

                SpeakerDraw co = speakerDraws.First(sd => sd.Position.Equals(Position.ClosingOpposition));
                Speaker coSpeaker = context.Speakers.First(sd => co.SpeakerId.Equals(sd.SpeakerId));
                findReplaces.Add("#co", context.Speakers.First(s => s.SpeakerId.Equals(co.SpeakerId)).Name);
                findReplaces.Add("#ico", context.Institutions.First(i => i.InstitutionId.Equals(coSpeaker.InstitutionId)).Name);

                foreach (KeyValuePair<string, string> keyValuePair in findReplaces)
                {
                    content.Find.Execute(FindText: keyValuePair.Key, Replace: Word.WdReplace.wdReplaceAll, ReplaceWith: keyValuePair.Value);
                }
                
                object ballotFilename = Path.Combine(ballotsDirectory, string.Format("{0}.doc", venue.Name));

                wordDocument.SaveAs(ref ballotFilename);

                object save = Word.WdSaveOptions.wdDoNotSaveChanges;
                
                wordDocument.Close(ref save);
                wordApplication.Quit();
            }
        }
        #endregion

        #region CreateOverview
        public static void CreateOverview(Tournament tournament, DataContext context, Guid roundId)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            Round round = helper.GetRound(roundId).Result;

            List<Debate> debates = helper.GetDebates(roundId).Result;

            Excel.Application excelApplication = new Excel.Application() {Visible = false};

            Excel.Workbook excelWorkbook = excelApplication.Workbooks.Add();

            Excel.Sheets excelSheets = excelWorkbook.Worksheets;

            Excel.Worksheet sheet = (Excel.Worksheet) excelSheets.get_Item("Sheet1");

            debates = debates.OrderBy(d => context.Venues.First(v => d.VenueId.Equals(v.VenueId)).Name).ToList();

            int column = 1;
            int letter = 'A';

            Excel.Range range = sheet.get_Range("A1");

            range.Value2 = "Venue";

            foreach (Position p in Enum.GetValues(typeof(Position)))
            {
                if(p.Equals(Position.Invalid)) continue;

                range = sheet.get_Range(string.Format("{0}{1}", (char)++letter, column));
                range.Value2 = p.ToString();
            }

            range = sheet.get_Range(string.Format("{0}{1}", (char)++letter, column));
            range.Value2 = "Adjudicators";

            column++;

            foreach (Debate debate in debates)
            {
                letter = 'A';

                Venue venue = context.Venues.First(v => v.VenueId.Equals(debate.VenueId));

                range.Value2 = string.Format("{0}{1}", venue.SpecialNeedsVenue ? "[SN]" : string.Empty, venue.Name);

                range = sheet.get_Range(string.Format("{0}{1}", (char) ++letter, column));

                List<SpeakerDraw> speakerDraws = helper.GetSpeakerDrawsByDebate(debate.DebateId).Result;

                foreach (SpeakerDraw speakerDraw in speakerDraws.OrderBy(sd => sd.Position))
                {
                    range = sheet.get_Range(string.Format("{0}{1}", (char)++letter, column));
                    range.Value2 = context.Speakers.First(s => speakerDraw.SpeakerId.Equals(s.SpeakerId)).Name;
                }

                List<JudgeDraw> judgeDraws = helper.GetJudgeDrawsByDebate(debate.DebateId).Result.OrderBy(j => j.Number).ToList();

                List<Guid> judgeIds = judgeDraws.Select(j => j.JudgeId).ToList();

                List<Judge> judges = context.Judges.Where(j => judgeIds.Contains(j.JudgeId)).ToList();

                List<string> names = judges.Select(j => j.Name).ToList();

                range = sheet.get_Range(string.Format("{0}{1}", (char)++letter, column));
                range.Value2 = string.Join(", ", names);

                column++;
            }

            string directory = Path.Combine(Path.GetDirectoryName(tournament.Location), string.Format("Round {0}", round.RoundNumber));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = Path.Combine(directory, string.Format("Round {0} - overview.xlsx", round.RoundNumber));

            string endCell = string.Format("{0}{1}", (char)(letter), column - 1);

            range = sheet.get_Range("A1", endCell);
            range.Columns.AutoFit();

            excelApplication.DisplayAlerts = false;
            excelWorkbook.SaveAs(filename);
            excelWorkbook.Close();
            excelApplication.DisplayAlerts = true;
            excelApplication.Quit();

        }
        #endregion

        #region CreateRotation
        public static void CreateRotation(Tournament tournament, DataContext context, int roundNumber)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            List<Round> rounds = helper.GetRounds().Result.Where(r => r.RoundNumber <= roundNumber).ToList();

            List<Tuple<int, List<Debate>>> debates = new List<Tuple<int, List<Debate>>>();

            foreach (Round round in rounds)
            {
                debates.Add(Tuple.Create(round.RoundNumber, helper.GetDebates(round.RoundId).Result.ToList()));
            }

            List<SpeakerDraw> draws = new List<SpeakerDraw>();

            foreach (Tuple<int, List<Debate>> debate in debates)
            {
                foreach (Debate d in debate.Item2)
                {
                    draws.AddRange(helper.GetSpeakerDrawsByDebate(d.DebateId).Result);
                }
            }

            List<Speaker> speakers = new List<Speaker>();
            speakers.AddRange(context.Speakers);
            speakers.AsParallel().ForAll(s => s.Draws = draws.Where(d => d.SpeakerId.Equals(s.SpeakerId)).ToList());
            speakers = speakers.OrderBy(s => s.Name).ToList();

            Excel.Application excelApplication = new Excel.Application() { Visible = false };

            Excel.Workbook excelWorkbook = excelApplication.Workbooks.Add();

            Excel.Sheets excelSheets = excelWorkbook.Worksheets;

            Excel.Worksheet sheet = (Excel.Worksheet)excelSheets.get_Item("Sheet1");

            Excel.Range range = sheet.get_Range("A1");
            range.Value2 = "Speaker";

            int column = 1;
            int letter = 'B';

            for (int i = 1; i <= roundNumber; i++)
            {
                range = sheet.get_Range(string.Format("{0}{1}", (char) letter++, column));
                range.Value2 = string.Format("Round {0}", roundNumber);
            }

            column++;           

            foreach (Speaker speaker in speakers)
            {
                letter = 'A';
                range = sheet.get_Range(string.Format("{0}{1}", (char) letter++, column));
                range.Value2 = speaker.Name;

                foreach (Round round in rounds)
                {
                    List<Debate> theseDebates = debates[round.RoundNumber - 1].Item2;
                    SpeakerDraw speakerDraw = speaker.Draws.First(sd => theseDebates.Select(d => d.DebateId).Contains(sd.DebateId));
                    range = sheet.get_Range(string.Format("{0}{1}", (char)letter++, column));
                    range.Value2 = speakerDraw.Position.ToString();
                }

                column++;
            }

            string directory = Path.Combine(Path.GetDirectoryName(tournament.Location), string.Format("Round {0}", roundNumber));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = Path.Combine(directory, string.Format("Round {0} - rotation.xlsx", roundNumber));

            string endCell = string.Format("{0}{1}", (char)(letter - 1), column - 1);

            range = sheet.get_Range("A1", endCell);
            range.Columns.AutoFit();

            excelApplication.DisplayAlerts = false;
            excelWorkbook.SaveAs(filename);
            excelWorkbook.Close();
            excelApplication.DisplayAlerts = true;
            excelApplication.Quit();
        }
        #endregion

        #region WriteTabSoFar
        public static void WriteTabSoFar(Tournament tournament, DataContext context, int roundNumber)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            List<Round> rounds = helper.GetRounds().Result.Where(r => r.RoundNumber <= roundNumber).ToList();

            List<Tuple<int, List<Debate>>> debates = new List<Tuple<int, List<Debate>>>();

            foreach (Round round in rounds)
            {
                debates.Add(Tuple.Create(round.RoundNumber, helper.GetDebates(round.RoundId).Result.ToList()));
            }

            List<SpeakerDraw> draws = new List<SpeakerDraw>();

            foreach (Tuple<int, List<Debate>> debate in debates)
            {
                foreach (Debate d in debate.Item2)
                {
                    draws.AddRange(helper.GetSpeakerDrawsByDebate(d.DebateId).Result);
                }
            }
            
            List<Speaker> speakers = new List<Speaker>();
            speakers.AddRange(context.Speakers);
            speakers.AsParallel().ForAll(s => s.Draws = draws.Where(d => d.SpeakerId.Equals(s.SpeakerId)).ToList());

            speakers = speakers.OrderByDescending(s => s.Draws.Sum(sd => sd.Result.Points())).ThenByDescending(s => s.Draws.Sum(sd => sd.SpeakerPoints)).ToList();

            Excel.Application excelApplication = new Excel.Application(){Visible = false};

            Excel.Workbook excelWorkbook = excelApplication.Workbooks.Add();

            Excel.Sheets excelSheets = excelWorkbook.Worksheets;

            Excel.Worksheet sheet = (Excel.Worksheet) excelSheets.get_Item("Sheet1");

            Excel.Range cell = sheet.get_Range("A2");
            cell.Value2 = "Speaker";
            cell = sheet.get_Range("B2");
            cell.Value2 = "Total Points";
            cell = sheet.get_Range("C2");
            cell.Value2 = "Total Speaker Points";


            int columns = 1;

            int columnLetter = 'D';

            for(int i = 1; i <= roundNumber;i++)
            {
                cell = sheet.get_Range(string.Format("{0}1", (char)columnLetter));
                cell.Value2 = string.Concat("Round ", i);
                cell = sheet.get_Range(string.Format("{0}2", (char) columnLetter++));
                cell.Value2 = "Result";
                cell = sheet.get_Range(string.Format("{0}2", (char)columnLetter++));
                cell.Value2 = "Speaker Points";
                columns += 3;
            }

            int letter = 'A';
            int row = 3;

            foreach (Speaker speaker in speakers)
            {
                letter = 'A';
                cell = sheet.get_Range(string.Format("{0}{1}", (char) letter++, row));
                cell.Value2 = speaker.Name;
                cell = sheet.get_Range(string.Format("{0}{1}", (char)letter++, row));
                cell.Value2 = speaker.Draws.Sum(x => x.Result.Points());
                cell = sheet.get_Range(string.Format("{0}{1}", (char)letter++, row));
                cell.Value2 = speaker.Draws.Sum(x => x.SpeakerPoints);

                foreach (Round round in rounds)
                {
                    List<Debate> theseDebates = debates[round.RoundNumber - 1].Item2;
                    SpeakerDraw speakerDraw = speaker.Draws.First(sd => theseDebates.Select(d => d.DebateId).Contains(sd.DebateId));
                    cell = sheet.get_Range(string.Format("{0}{1}", (char)letter++, row));
                    cell.Value2 = speakerDraw.Result.Points();
                    cell = sheet.get_Range(string.Format("{0}{1}", (char)letter++, row));
                    cell.Value2 = speakerDraw.SpeakerPoints;                    
                }
                row++;
            }
            
            string directory = Path.Combine(Path.GetDirectoryName(tournament.Location), string.Format("Round {0}", roundNumber));
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = Path.Combine(directory, string.Format("Round {0} - tab.xlsx", roundNumber));

            string endCell = string.Format("{0}{1}", (char) (letter - 1), row - 1);

            cell = sheet.get_Range("A1", endCell);
            cell.Columns.AutoFit();

            excelApplication.DisplayAlerts = false;
            excelWorkbook.SaveAs(filename); 
            excelWorkbook.Close();
            excelApplication.DisplayAlerts = true;
            excelApplication.Quit();
        }
        #endregion
    }
}
