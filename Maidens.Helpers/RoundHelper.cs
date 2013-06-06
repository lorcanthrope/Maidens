using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Maidens.Models;

namespace Maidens.Helpers
{
    public static class RoundHelper
    {
        #region DrawRound
        public static Round DrawRound(Tournament tournament, string motion, bool isPowerPaired = false)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);
            DataContext context = helper.GetDataContext().Result;
            context.Speakers.AsParallel().ForAll(a => a.Draws = helper.GetSpeakerDraws(a.SpeakerId).Result);

            Round round;

            if(isPowerPaired)
            {
                round = DrawPowerpairedRound(tournament, context, motion);    
            }
            else
            {
                round = DrawNonPowerpairedRound(tournament, context, motion);
            }
            tournament.RoundNumber++;
            tournament.Update();

            MicrosoftHelper.CreateBallots(tournament, context, round.RoundId);
            MicrosoftHelper.CreateOverview(tournament, context, round.RoundId);
            MicrosoftHelper.CreateRotation(tournament, context, round.RoundNumber);
            HtmlHelper.CreateDisplay(tournament, context, round.RoundId);

            return round;
        }
        #endregion

        #region DrawPowerpairedRound
        private static Round DrawPowerpairedRound(Tournament tournament, DataContext context, string motion)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            Round round = new Round()
                              {
                                  Motion = motion,
                                  IsRandom = false,
                                  RoundNumber = tournament.RoundNumber
                              };

            NonQueryResult insert = helper.InsertRound(round);

            List<Debate> debates = new List<Debate>();

            List<Speaker> speakers = new List<Speaker>();
            speakers.AddRange(context.Speakers.Where(a => a.Active));
            speakers.AsParallel().ForAll(a => a.Draws = helper.GetSpeakerDraws(a.SpeakerId).Result);
            speakers = speakers.OrderByDescending(a => a.Draws.Sum(d => d.Result.Points())).ThenByDescending(s => s.Draws.Sum(d => d.SpeakerPoints)).ToList();

            int numberOfRooms = speakers.Count/6;
            int numberOfJudgesPerRoom = context.Judges.Count/numberOfRooms;
            
            List<Judge> judges = new List<Judge>();
            judges.AddRange(context.Judges.Where(j => j.Active));
            judges = judges.OrderByDescending(j => (int)j.Level).ToList();

            List<Venue> venues = new List<Venue>();
            venues.AddRange(context.Venues);


            for(int i = 1;i <= numberOfRooms;i++)
            {
                Venue v = venues.First();
                venues.Remove(v);
                Debate debate = new Debate()
                                    {
                                        RoundId = round.RoundId,
                                        VenueId = v.VenueId
                                    };
                
                debates.Add(debate);

                insert = helper.InsertDebate(debate);

                List<Speaker> toInsert = speakers.Take(6).ToList();

                SetSpeakersResult result = SetSpeakers(toInsert, debate.DebateId);

                foreach (SpeakerDraw speakerDraw in result.SpeakerDraws)
                {
                    helper.InsertSpeakerDraw(speakerDraw);
                }

                foreach (Speaker speaker in toInsert)
                {
                    speakers.Remove(speaker);
                }

                Judge judge = judges.First();
                JudgeDraw judgeDraw = new JudgeDraw()
                {
                    DebateId = debate.DebateId,
                    Number = 1,
                    JudgeId = judge.JudgeId
                };
                judges.Remove(judge);
                insert = helper.InsertJudgeDraw(judgeDraw);
            }

            foreach (Debate debate in debates)
            {
                for (int i = 2; i <= numberOfJudgesPerRoom; i++)
                {
                    Judge j = judges.First();
                    JudgeDraw judgeDraw = new JudgeDraw()
                    {
                        DebateId = debate.DebateId,
                        Number = i,
                        JudgeId = j.JudgeId
                    };
                    judges.Remove(j);
                    insert = helper.InsertJudgeDraw(judgeDraw);
                }
            }

            while (judges.Any())
            {
                Debate debate = debates.OrderBy(a => a.DebateId).First();
                debates.Remove(debate);
                Judge j = judges.First();
                JudgeDraw judgeDraw = new JudgeDraw()
                {
                    DebateId = debate.DebateId,
                    Number = numberOfJudgesPerRoom + 1,
                    JudgeId = j.JudgeId
                };
                judges.Remove(j);
                insert = helper.InsertJudgeDraw(judgeDraw);
            }

            return round;
        }
        #endregion

        #region DrawNonPowerpairedRound
        private static Round DrawNonPowerpairedRound(Tournament tournament, DataContext context, string motion)
        {
            SqlHelper helper = new SqlHelper(tournament.Database);

            Round round = new Round()
                              {
                                  Motion = motion,
                                  IsRandom = true,
                                  RoundNumber = tournament.RoundNumber
                              };

            NonQueryResult insert = helper.InsertRound(round);

            Random r = new Random();

            List<Debate> debates = new List<Debate>();

            List<Speaker> initialSpeakers = context.Speakers.Where(s => s.Active).OrderBy(a => Guid.NewGuid()).ToList();
            Queue<Speaker> speakers = new Queue<Speaker>(context.Speakers.Count(s => s.Active));

            int count = context.Speakers.Count(s => s.Active);
            while(initialSpeakers.Any())
            {
                Speaker s = initialSpeakers.First();
                speakers.Enqueue(s);
                initialSpeakers.Remove(s);                
            }
            
            int numberOfRooms = speakers.Count/6;
            int numberOfJudgesPerRoom = context.Judges.Count(j => j.Active)/numberOfRooms;
            
            List<Judge> judges = new List<Judge>();
            judges.AddRange(context.Judges.Where(j => j.Active));
            judges = judges.OrderByDescending(j => (int) j.Level).ToList();

            List<Venue> venues = new List<Venue>();
            venues.AddRange(context.Venues.Where(v => v.Active));

            for(int i = 1; i <= numberOfRooms;i++)
            {
                Venue v = venues.First();
                venues.Remove(v);

                Debate debate = new Debate()
                                    {
                                        RoundId = round.RoundId,
                                        VenueId = v.VenueId
                                    };
                debates.Add(debate);

                insert = helper.InsertDebate(debate);

                foreach(Position p in Enum.GetValues(typeof (Position)))
                {
                    if(p == Position.Invalid) continue;

                    Speaker s = speakers.Dequeue();

                    SpeakerDraw speakerDraw = new SpeakerDraw()
                                           {
                                               DebateId = debate.DebateId,
                                               Position = p,
                                               SpeakerId = s.SpeakerId
                                           };

                    insert = helper.InsertSpeakerDraw(speakerDraw);
                }
            }

            foreach (Debate debate in debates.OrderBy(a => Guid.NewGuid()))
            {
                Judge j = judges.First();
                JudgeDraw judgeDraw = new JudgeDraw()
                {
                    DebateId = debate.DebateId,
                    Number = 1,
                    JudgeId = j.JudgeId
                };
                judges.Remove(j);
                insert = helper.InsertJudgeDraw(judgeDraw);
            }

            foreach (Debate debate in debates.OrderBy(a => Guid.NewGuid()))
            {
                for(int i = 2; i <= numberOfJudgesPerRoom;i++)
                {
                    Judge j = judges.First();
                    JudgeDraw judgeDraw = new JudgeDraw()
                    {
                        DebateId = debate.DebateId,
                        Number = i,
                        JudgeId = j.JudgeId
                    };
                    judges.Remove(j);
                    insert = helper.InsertJudgeDraw(judgeDraw);
                }
            }
            
            while(judges.Any())
            {
                Debate debate = debates.OrderBy(a => a.DebateId).First();
                debates.Remove(debate);
                Judge j = judges.First();
                JudgeDraw judgeDraw = new JudgeDraw()
                {
                    DebateId = debate.DebateId,
                    Number = numberOfJudgesPerRoom+1,
                    JudgeId = j.JudgeId
                };
                judges.Remove(j);
                insert = helper.InsertJudgeDraw(judgeDraw);
            }

            return round;
        }
        #endregion

        #region SetSpeakers
        private static SetSpeakersResult SetSpeakers(List<Speaker> speakers, Guid debateId)
        {
            Random r = new Random();

            Dictionary<Position, Speaker> sorting = new Dictionary<Position, Speaker>();
            foreach(Position p in Enum.GetValues(typeof(Position)))
            {
                if(p == Position.Invalid) continue;

                sorting.Add(p, null);
            }

            List<Position> positions = sorting.Select(a => a.Key).ToList();
            
            List<Speaker> temp = new List<Speaker>();
            temp.AddRange(speakers);

            Dictionary<Position, List<Speaker>> canHavePosition = new Dictionary<Position, List<Speaker>>();
            
            foreach (Position position in positions)
            {
                List<Speaker> hasntHad = temp.Where(s => !s.Draws.Select(d => d.Position).Contains(position)).ToList();
                canHavePosition.Add(position, hasntHad);
            }
            
            Dictionary<Position, List<Speaker>> temp2 = new Dictionary<Position, List<Speaker>>();

            int tries = 0;

            Dictionary<int, Dictionary<Position, Speaker>> mapping = new Dictionary<int, Dictionary<Position, Speaker>>();


            bool complete = false;

            while(tries <= 8  && !complete)
            {
                mapping[tries] = new Dictionary<Position, Speaker>();
                
                if(tries % 2 == 0 && tries != 0)
                {
                    foreach (KeyValuePair<Position, List<Speaker>> keyValuePair in canHavePosition)
                    {
                        List<Speaker> x = speakers.Where(c => !canHavePosition[keyValuePair.Key].Contains(c)).ToList();
                        if(!x.Any()) continue;
                        canHavePosition[keyValuePair.Key].Add(x.ToArray()[r.Next(0, x.Count - 1)]);
                    }
                }

                temp2.Clear();
                foreach (KeyValuePair<Position, List<Speaker>> keyValuePair in canHavePosition)
                {
                    List<Speaker> s = new List<Speaker>();
                    s.AddRange(keyValuePair.Value);
                    temp2.Add(keyValuePair.Key, s);
                }
                
                foreach (Position position in positions.Where(p => temp2[p].Count() == 1))
                {                        
                    mapping[tries][position] = sorting[position];
                    temp2.Remove(position);
                }                

                List<Position> alreadyFilled = new List<Position>();

                foreach (KeyValuePair<Position, List<Speaker>> t in temp2)
                {
                    if(!t.Value.Any()) continue;

                    if (alreadyFilled.Contains(t.Key)) continue;                    

                    Speaker s = t.Value.ToArray()[r.Next(0, t.Value.Count - 1)];
                    mapping[tries][t.Key] = s;
                    foreach (KeyValuePair<Position, List<Speaker>> keyValuePair in temp2.Where(a => a.Key > t.Key))
                    {
                        keyValuePair.Value.Remove(s);
                    }

                    foreach (KeyValuePair<Position, List<Speaker>> keyValuePair in temp2.Where(a => a.Value.Count == 1 && !mapping[tries].ContainsKey(a.Key)))
                    {
                        if(mapping[tries].Any(m => m.Value != null && m.Value.Equals(keyValuePair.Value.First())))
                        {
                            continue;
                        }

                        mapping[tries][keyValuePair.Key] = keyValuePair.Value.First();
                        foreach (KeyValuePair<Position, List<Speaker>> valuePair in temp2.Where(a => a.Key != keyValuePair.Key))
                        {
                            valuePair.Value.Remove(keyValuePair.Value.First());
                        }
                        alreadyFilled.Add(keyValuePair.Key);
                    }
                }

                if(positions.All(p => mapping[tries].ContainsKey(p)) && !mapping[tries].Any(a => a.Value == null))
                {
                    complete = true;
                    foreach (KeyValuePair<Position, Speaker> m in mapping[tries])
                    {
                        sorting[m.Key] = m.Value;
                    }
                }

                tries++;
            }                      
            
            List<SpeakerDraw> speakerDraws = sorting.Select(a => new SpeakerDraw()
            {
                DebateId = debateId,
                Position = a.Key,
                SpeakerId = a.Value.SpeakerId
            }).ToList();

            return new SetSpeakersResult()
                       {
                           Complete = true,
                           SpeakerDraws = speakerDraws
                       };
        }
        #endregion
    }
}
