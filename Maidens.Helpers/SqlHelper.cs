using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

using Maidens.Models;

namespace Maidens.Helpers
{
    public class SqlHelper
    {
        #region Fields
        private readonly string ConnectionString;
        private const string EnforceForeignKeys = "PRAGMA foreign_keys=ON;";
        #endregion
        
        #region Selectors
        private readonly Func<IDataReader, Debate> GetDebateSelector = reader => new Debate()
        {
            DebateId = Guid.Parse(reader.GetString(0)),
            RoundId = Guid.Parse(reader.GetString(1)),
            VenueId = Guid.Parse(reader.GetString(2))
        };

        private readonly Func<IDataReader, Institution> GetInstitutionSelector = reader => new Institution()
        {
            InstitutionId = Guid.Parse(reader.GetString(0)),
            Name = reader.GetString(1)
        };
        
        private readonly Func<IDataReader, JudgeDraw> GetJudgeDrawSelector = reader => new JudgeDraw()
        {
            DrawId = Guid.Parse(reader.GetString(0)),
            JudgeId = Guid.Parse(reader.GetString(1)),
            DebateId = Guid.Parse(reader.GetString(2)),
            Number = reader.GetInt32(3)
        };

        private readonly Func<IDataReader, Judge> GetJudgeSelector = reader => new Judge()
        {
            JudgeId = Guid.Parse(reader.GetString(0)),
            Name = reader.GetString(1),
            Level = (JudgeLevel)reader.GetInt32(2),
            InstitutionId = reader.IsDBNull(3) ? default(Guid?) : Guid.Parse(reader.GetString(3)),
            Active = bool.Parse(reader.GetString(4))
        };

        private readonly Func<IDataReader, Round> GetRoundSelector = reader => new Round()
        {
            RoundId = Guid.Parse(reader.GetString(0)),
            RoundNumber = reader.GetInt32(1),
            Motion = reader.GetString(2),
            IsRandom = bool.Parse(reader.GetString(3))
        };

        private readonly Func<IDataReader, SpeakerDraw> GetSpeakerDrawSelector = reader => new SpeakerDraw()
        {
            DrawId = Guid.Parse(reader.GetString(0)),
            SpeakerId = Guid.Parse(reader.GetString(1)),
            DebateId = Guid.Parse(reader.GetString(2)),
            Position = (Position)reader.GetInt32(3),
            Result = reader.IsDBNull(4) ? Result.Unspecified : (Result)reader.GetInt32(4),
            SpeakerPoints = reader.IsDBNull(5) ? 50 : reader.GetInt32(5)
        };

        private readonly Func<IDataReader, Speaker> GetSpeakerSelector = reader => new Speaker()
        {
            SpeakerId = Guid.Parse(reader.GetString(0)),
            Name = reader.GetString(1),
            InstitutionId = Guid.Parse(reader.GetString(2)),
            SpecialNeeds = bool.Parse(reader.GetString(3)),
            Active = bool.Parse(reader.GetString(4))
        };

        private readonly Func<IDataReader, Venue> GetVenueSelector = reader => new Venue()
        {
            VenueId = Guid.Parse(reader.GetString(0)),
            Name = reader.GetString(1),
            SpecialNeedsVenue = bool.Parse(reader.GetString(2)),
            Active = bool.Parse(reader.GetString(3))
        };

        #endregion
        
        #region Construction
        public SqlHelper(string location, bool createNew = false)
        {
            ConnectionString = string.Format("Data Source={0}", location);

            if (createNew)
            {
                SQLiteConnection.CreateFile(location);
                PopulateDatabase();
            }            
        }
        #endregion

        #region Initialize

        #region CreateDebatesTable
         /// <summary>
        /// Create the schema for the Debates table -  a simple lookup for debates
        /// </summary>        
        private bool CreateDebatesTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE debate (");
            query.AppendLine("debate_id TEXT(36) NOT NULL,");
            query.AppendLine("round_id TEXT(36) NOT NULL,");
            query.AppendLine("venue_id TEXT(36) NOT NULL,");
            query.AppendLine("FOREIGN KEY (round_id) REFERENCES round(round_id),");
            query.AppendLine("FOREIGN KEY (venue_id) REFERENCES venue(venue_id),");
            query.AppendLine("PRIMARY KEY (debate_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateInstitutionsTable
        /// <summary>
        /// Create the schema for the Institutions table - a simple lookup for institutions
        /// </summary>        
        private bool CreateInstitutionsTable()
        {           
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE institution (");
            query.AppendLine("institution_id TEXT(36) NOT NULL,");
            query.AppendLine("institution_name TEXT(100) NOT NULL,");
            query.AppendLine("PRIMARY KEY (institution_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateJudgesTable
        /// <summary>
        /// Creates the schema for the Judges table - a simple lookup for judges
        /// </summary>
        /// <returns></returns>
        private bool CreateJudgesTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE judge (");
            query.AppendLine("judge_id TEXT(36) NOT NULL,");
            query.AppendLine("judge_name TEXT(100) NOT NULL,");
            query.AppendLine("judge_level INTEGER NOT NULL,");
            query.AppendLine("institution_id TEXT(36) NULL,");
            query.AppendLine("active BOOLEAN NOT NULL, ");
            query.AppendLine("FOREIGN KEY (institution_id) REFERENCES institution(institution_id),");
            query.AppendLine("PRIMARY KEY (judge_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateJudgesDrawTable
        /// <summary>
        /// Create the schema for the JudgesDraw table -  a simple lookup for judge draws
        /// </summary>        
        private bool CreateJudgesDrawTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE judge_draw (");
            query.AppendLine("draw_id TEXT(36) NOT NULL,");
            query.AppendLine("judge_id TEXT(36) NOT NULL,");
            query.AppendLine("debate_id TEXT(36) NOT NULL,");
            query.AppendLine("number INTEGER NOT NULL,");
            query.AppendLine("FOREIGN KEY (judge_id) REFERENCES judge(judge_id),");
            query.AppendLine("FOREIGN KEY (debate_id) REFERENCES debate(debate_id),");
            query.AppendLine("PRIMARY KEY (draw_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateRoundsTable]
        /// <summary>
        /// Creates the schema for the Rounds table - a simple lookup for rounds
        /// </summary>        
        private bool CreateRoundsTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE round (");
            query.AppendLine("round_id TEXT(36) NOT NULL,");
            query.AppendLine("round_number INTEGER NOT NULL,");
            query.AppendLine("motion TEXT(500) NOT NULL,");
            query.AppendLine("is_random BOOLEAN NOT NULL,");
            query.AppendLine("PRIMARY KEY (round_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateSpeakersTable
        /// <summary>
        /// Create the schema for the Speakers table -  a simple lookup for speakers
        /// </summary>        
        private bool CreateSpeakersTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE speaker (");
            query.AppendLine("speaker_id TEXT(36) NOT NULL,");
            query.AppendLine("speaker_name TEXT(100) NOT NULL,");
            query.AppendLine("institution_id TEXT(36) NOT NULL,");
            query.AppendLine("special_needs BOOLEAN NOT NULL,");
            query.AppendLine("active BOOLEAN NOT NULL,");
            query.AppendLine("FOREIGN KEY (institution_id) REFERENCES institution(institution_id),");
            query.AppendLine("PRIMARY KEY (speaker_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateSpeakersDrawTable
        /// <summary>
        /// Create the schema for the SpeakersDraw table -  a simple lookup for speaker draws
        /// </summary>        
        private bool CreateSpeakerDrawTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE speaker_draw (");
            query.AppendLine("draw_id TEXT(36) NOT NULL,");
            query.AppendLine("speaker_id TEXT(36) NOT NULL,");
            query.AppendLine("debate_id TEXT(36) NOT NULL,");
            query.AppendLine("position INTEGER NOT NULL,");
            query.AppendLine("result INTEGER, ");
            query.AppendLine("speaker_points INTEGER, ");
            query.AppendLine("FOREIGN KEY (speaker_id) REFERENCES speaker(speaker_id),");
            query.AppendLine("FOREIGN KEY (debate_id) REFERENCES debate(debate_id),");
            query.AppendLine("PRIMARY KEY (draw_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region CreateVenuesTable
        /// <summary>
        /// Creates the schema for the Venues table - a simple lookup for venues
        /// </summary>
        private bool CreateVenuesTable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("CREATE TABLE venue (");
            query.AppendLine("venue_id TEXT(36) NOT NULL,");
            query.AppendLine("venue_name TEXT(100) NOT NULL,");
            query.AppendLine("special_needs_venue BOOLEAN NOT NULL,");
            query.AppendLine("active BOOLEAN NOT NULL,");
            query.AppendLine("PRIMARY KEY (venue_id))");
            NonQueryResult result = ExecuteNonQuery(query.ToString());
            return result.Exception == null;
        }
        #endregion

        #region PopulateDatabase
        private void PopulateDatabase()
        {
            CreateInstitutionsTable();
            CreateSpeakersTable();
            CreateJudgesTable();
            CreateVenuesTable();
            CreateRoundsTable();
            CreateDebatesTable();
            CreateSpeakerDrawTable();
            CreateJudgesDrawTable();
        }
        #endregion
        
        #endregion

        #region SQL

        #region ExecuteNonQuery
        private NonQueryResult ExecuteNonQuery(string commandText, bool enforceForeignKeys = false)
        {
            NonQueryResult result = new NonQueryResult()
                                        {
                                            CommandText = commandText,
                                            EnforceForeignKeys = enforceForeignKeys
                                        };
            try
            {
                using(SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;

                    if(enforceForeignKeys)
                    {
                        command.CommandText = EnforceForeignKeys;
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                    result.Completed = true;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;                
            }

            return result;
        }
        #endregion
        
        #region ExecuteReader
        private IEnumerable<T> ExecuteReader<T>(string commandText, Func<IDataReader, T> selector)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = commandText;                
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return selector(reader);
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Aggregate

        #region GetDataContext
        public QueryResult<DataContext> GetDataContext()
        {
            QueryResult<DataContext> result = new QueryResult<DataContext>();
            
            try
            {
                result.Result = new DataContext();
                result.Result.Speakers = GetSpeakers().Result;
                result.Result.Institutions = GetInstitutions().Result;
                result.Result.Judges = GetJudges().Result;
                result.Result.Venues = GetVenues().Result;
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #endregion

        #region Debates

        #region GetDebate
        public QueryResult<Debate> GetDebate(Guid debateId)
        {
            QueryResult<Debate> result = new QueryResult<Debate>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT debate_id, round_id, venue_id FROM debate ");
                query.AppendFormat("WHERE debate_id = '{0}'", debateId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Debate>(query.ToString(), GetDebateSelector).FirstOrDefault();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetDebates
        public QueryResult<List<Debate>> GetDebates()
        {
            QueryResult<List<Debate>> result = new QueryResult<List<Debate>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT debate_id, round_id, venue_id FROM debate");
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Debate>(query.ToString(), GetDebateSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public QueryResult<List<Debate>> GetDebates(Guid roundId)
        {
            QueryResult<List<Debate>> result = new QueryResult<List<Debate>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT debate_id, round_id, venue_id FROM debate ");
                query.AppendFormat("WHERE round_id = '{0}'", roundId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Debate>(query.ToString(), GetDebateSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        #endregion

        #region InsertDebate
        public NonQueryResult InsertDebate(Debate debate)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO debate (debate_id, round_id, venue_id) ");
            query.AppendFormat("VALUES ('{0}','{1}', '{2}')", debate.DebateId, debate.RoundId, debate.VenueId);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #region UpdateDebate
        public NonQueryResult UpdateDebate(Debate debate)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE debate ");
            query.AppendFormat("SET round_id = '{0}', ", debate.RoundId);
            query.AppendFormat("venue_id = '{0}' ", debate.VenueId);
            query.AppendFormat("WHERE debate_id = '{0}'", debate.DebateId);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #endregion

        #region Institutions

        #region DeleteInstitution
        public NonQueryResult DeleteInstitution(Guid institutionId)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM institution WHERE institution_id = '{0}'", institutionId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion
        
        #region GetInstitution
        public QueryResult<Institution> GetInstitution(Guid institutionId)
        {
            QueryResult<Institution> result = new QueryResult<Institution>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT institution_id, institution_name FROM institution ");
                query.AppendFormat("WHERE institution_id = '{0}'", institutionId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Institution>(query.ToString(), GetInstitutionSelector).FirstOrDefault();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetInstitutions
        public QueryResult<List<Institution>> GetInstitutions()
        {
            QueryResult<List<Institution>> result = new QueryResult<List<Institution>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT institution_id, institution_name FROM institution");
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Institution>(query.ToString(), GetInstitutionSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region InsertInstitution
        public NonQueryResult InsertInstitution(Institution institution)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO institution (institution_id, institution_name) ");
            query.AppendFormat("VALUES('{0}','{1}')", institution.InstitutionId, TextHelper.EscapeSingleQuotes(institution.Name));
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region UpdateInstitution
        public NonQueryResult UpdateInstitution(Institution institution)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE institution ");
            query.AppendLine("SET ");
            query.AppendFormat("institution_name = '{0}' ", institution.Name);
            query.AppendFormat("WHERE institution_id = '{0}'", institution.InstitutionId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #endregion

        #region JudgeDraw

        #region GetJudgeDraw
        public QueryResult<JudgeDraw> GetJudgeDraw(Guid drawId)
        {
            QueryResult<JudgeDraw> result = new QueryResult<JudgeDraw>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT draw_id, judge_id, debate_id, number from judge_draw ");
                query.AppendFormat("WHERE draw_id = '{0}'", drawId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<JudgeDraw>(query.ToString(), GetJudgeDrawSelector).First();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetJudgeDraws
        public QueryResult<List<JudgeDraw>> GetJudgeDrawsByDebate(Guid debateId)
        {
            QueryResult<List<JudgeDraw>> result = new QueryResult<List<JudgeDraw>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT draw_id, judge_id, debate_id, number from judge_draw ");
                query.AppendFormat("WHERE debate_id = '{0}'", debateId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<JudgeDraw>(query.ToString(), GetJudgeDrawSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }


        #endregion

        #region InsertJudgeDraw
        public NonQueryResult InsertJudgeDraw(JudgeDraw judgeDraw)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO judge_draw (draw_id, judge_id, debate_id, number) ");
            query.AppendFormat("VALUES('{0}', '{1}', '{2}', '{3}')", judgeDraw.DrawId, judgeDraw.JudgeId, judgeDraw.DebateId, judgeDraw.Number);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #region UpdateJudgeDraw
        public NonQueryResult UpdateJudgeDraw(JudgeDraw judgeDraw)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE judge_draw ");
            query.AppendFormat("SET judge_id = '{0}', ", judgeDraw.JudgeId);
            query.AppendFormat("debate_id = '{0}', ", judgeDraw.DebateId);
            query.AppendFormat("number = {0} ", judgeDraw.Number);
            query.AppendFormat("WHERE draw_id = '{0}'", judgeDraw.DrawId);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #endregion

        #region Judges

        #region DeleteJudge
        public NonQueryResult DeleteJudge(Guid judgeId)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM judge WHERE judge_id = '{0}'", judgeId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region GetJudge
        public QueryResult<Judge> GetJudge(Guid judgeId)
        {
            QueryResult<Judge> result = new QueryResult<Judge>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT judge_id, judge_name, judge_level, institution_id, active FROM judge ");
                query.AppendFormat("WHERE judge_id = '{0}'", judgeId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Judge>(query.ToString(), GetJudgeSelector).FirstOrDefault();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetJudges
        public QueryResult<List<Judge>> GetJudges()
        {
            QueryResult<List<Judge>> result = new QueryResult<List<Judge>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT judge_id, judge_name, judge_level, institution_id, active FROM judge");
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Judge>(query.ToString(), GetJudgeSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region InsertJudge
        public NonQueryResult InsertJudge(Judge judge)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("INSERT INTO judge (judge_id, judge_name, judge_level{0}, active) {1}", judge.InstitutionId.HasValue ? ", institution_id" : string.Empty, Environment.NewLine);
            query.AppendFormat("VALUES('{0}','{1}','{2}'{3}, '{4}' )", judge.JudgeId, TextHelper.EscapeSingleQuotes(judge.Name), (int)judge.Level, judge.InstitutionId.HasValue ? string.Format(", '{0}'", judge.InstitutionId.Value) : string.Empty, judge.Active);
            return ExecuteNonQuery(query.ToString(), true); 
        }
        #endregion

        #region UpdateJudge
        public NonQueryResult UpdateJudge(Judge judge)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE judge ");
            query.AppendLine("SET ");
            query.AppendFormat("judge_name = '{0}',", TextHelper.EscapeSingleQuotes(judge.Name));
            query.AppendFormat("judge_level = '{0}', ", (int)judge.Level);
            if(judge.InstitutionId.HasValue)
            {
                query.AppendFormat("institution_id = '{0}', ", judge.InstitutionId.Value);
            }
            query.AppendFormat("active = '{0}'", judge.Active);
            query.AppendFormat("WHERE judge_id = '{0}'", judge.JudgeId);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #endregion

        #region Rounds

        #region DeleteRound
        public NonQueryResult DeleteRound(Guid roundId)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM round WHERE roundId = '{0}'", roundId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region GetRound
        public QueryResult<Round> GetRound(Guid roundId)
        {
            QueryResult<Round> result = new QueryResult<Round>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT round_id, round_number, motion, is_random FROM round ");
                query.AppendFormat("WHERE round_id = '{0}'", roundId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Round>(query.ToString(), GetRoundSelector).FirstOrDefault();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetRounds
        public QueryResult<List<Round>> GetRounds()
        {
            QueryResult<List<Round>> result = new QueryResult<List<Round>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT round_id, round_number, motion, is_random FROM round");
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Round>(query.ToString(), GetRoundSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region InsertRound
        public NonQueryResult InsertRound(Round round)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO round (round_id, round_number, motion, is_random) ");
            query.AppendFormat("VALUES('{0}','{1}','{2}','{3}')", round.RoundId, round.RoundNumber, TextHelper.EscapeSingleQuotes(round.Motion), round.IsRandom);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region UpdateRound
        public NonQueryResult UpdateRound(Round round)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE round ");
            query.AppendLine("SET ");
            query.AppendFormat("round_number = {0}, ", round.RoundNumber);
            query.AppendFormat("motion = '{0}' ", TextHelper.EscapeSingleQuotes(round.Motion));
            query.AppendFormat("is_random = '{0}' ", round.IsRandom);
            query.AppendFormat("WHERE round_id = '{0}'", round.RoundId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #endregion

        #region SpeakerDraws

        #region CountOfUnfinishedResults
        public QueryResult<int> CountOfUnfinishedResults(Guid debateId)
        {
            QueryResult<int> result = new QueryResult<int>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT COUNT(*) FROM speaker_draw ");
                query.AppendFormat("WHERE debate_id = '{0}' AND (result = -1 OR result IS NULL)", debateId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<int>(query.ToString(), (reader) => reader.GetInt32(0)).First();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetSpeakerDraw
        public QueryResult<SpeakerDraw> GetSpeakerDraw(Guid drawId)
        {
            QueryResult<SpeakerDraw> result = new QueryResult<SpeakerDraw>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT draw_id, speaker_id, debate_id, position, result, speaker_points FROM speaker_draw ");
                query.AppendFormat("WHERE draw_id = '{0}'", drawId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<SpeakerDraw>(query.ToString(), GetSpeakerDrawSelector).First();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        #endregion

        #region GetSpeakerDraws
        public QueryResult<List<SpeakerDraw>> GetSpeakerDraws(Guid speakerId)
        {
            QueryResult<List<SpeakerDraw>> result = new QueryResult<List<SpeakerDraw>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT draw_id, speaker_id, debate_id, position, result, speaker_points FROM speaker_draw ");
                query.AppendFormat("WHERE speaker_id = '{0}'", speakerId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<SpeakerDraw>(query.ToString(), GetSpeakerDrawSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            
            return result;
        }

        public QueryResult<List<SpeakerDraw>> GetSpeakerDrawsByDebate(Guid debateId)
        {
            QueryResult<List<SpeakerDraw>> result = new QueryResult<List<SpeakerDraw>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT draw_id, speaker_id, debate_id, position, result, speaker_points FROM speaker_draw ");
                query.AppendFormat("WHERE debate_id = '{0}'", debateId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<SpeakerDraw>(query.ToString(), GetSpeakerDrawSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        } 

        public QueryResult<List<SpeakerDraw>> GetSpeakerDrawsUntilRoundNumber(Guid speakerId, int roundNumber)
        {
            QueryResult<List<SpeakerDraw>> result = new QueryResult<List<SpeakerDraw>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT draw_id, speaker_id, speaker_draw.debate_id, position, result, speaker_points FROM speaker_draw ");
                query.AppendLine("JOIN debate ");
                query.AppendLine("ON debate.debate_id = speaker_draw.debate_id ");
                query.AppendLine("JOIN round ");
                query.AppendLine("ON round.round_id = debate.round_id ");
                query.AppendFormat("WHERE speaker_id = '{0}' and round.round_number <= {1}", speakerId, roundNumber);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<SpeakerDraw>(query.ToString(), GetSpeakerDrawSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        #endregion

        #region InsertSpeakerDraw
        public NonQueryResult InsertSpeakerDraw(SpeakerDraw speakerDraw)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO speaker_draw (draw_id, speaker_id, debate_id, position) ");
            query.AppendFormat("VALUES('{0}', '{1}', '{2}', '{3}')", speakerDraw.DrawId, speakerDraw.SpeakerId, speakerDraw.DebateId, (int)speakerDraw.Position);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #region UpdateSpeakerDraw
        public NonQueryResult UpdateSpeakerDraw(SpeakerDraw speakerDraw)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE speaker_draw ");
            query.AppendFormat("SET speaker_id = '{0}', ", speakerDraw.SpeakerId);
            query.AppendFormat("debate_id = '{0}', ", speakerDraw.DebateId);
            query.AppendFormat("position = {0}, ", (int)speakerDraw.Position);
            query.AppendFormat("result = {0},  ", (int)speakerDraw.Result);
            query.AppendFormat("speaker_points = {0} ", speakerDraw.SpeakerPoints);
            query.AppendFormat("WHERE draw_id = '{0}'", speakerDraw.DrawId);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #endregion

        #region Speakers

        #region DeleteSpeaker
        public NonQueryResult DeleteSpeaker(Guid speakerId)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM speaker WHERE speaker_id = '{0}'", speakerId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region GetSpeaker
        public QueryResult<Speaker> GetSpeaker(Guid speakerId)
        {
            QueryResult<Speaker> result = new QueryResult<Speaker>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT speaker_id, speaker_name, institution_id, special_needs, active FROM speaker ");
                query.AppendFormat("WHERE speaker_id = '{0}'", speakerId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Speaker>(query.ToString(), GetSpeakerSelector).FirstOrDefault();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region GetSpeakers
        public QueryResult<List<Speaker>> GetSpeakers()
        {
            QueryResult<List<Speaker>> result = new QueryResult<List<Speaker>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT speaker_id, speaker_name, institution_id, special_needs, active FROM speaker");
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Speaker>(query.ToString(), GetSpeakerSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
        #endregion

        #region InsertSpeaker
        public NonQueryResult InsertSpeaker(Speaker speaker)
        {            
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO speaker (speaker_id, speaker_name, institution_id, special_needs, active) ");
            query.AppendFormat("VALUES('{0}','{1}','{2}','{3}','{4}')", speaker.SpeakerId, TextHelper.EscapeSingleQuotes(speaker.Name), speaker.InstitutionId, speaker.SpecialNeeds, speaker.Active);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #region UpdateSpeaker
        public NonQueryResult UpdateSpeaker(Speaker speaker)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE speaker ");
            query.AppendLine("SET ");
            query.AppendFormat("speaker_name = '{0}', ", TextHelper.EscapeSingleQuotes(speaker.Name));
            query.AppendFormat("institution_id = '{0}', ", speaker.InstitutionId);
            query.AppendFormat("special_needs = '{0}', ", speaker.SpecialNeeds);
            query.AppendFormat("active = '{0}' ", speaker.Active);
            query.AppendFormat("WHERE speaker_id = '{0}'", speaker.SpeakerId);
            return ExecuteNonQuery(query.ToString(), true);
        }
        #endregion

        #endregion

        #region Venues

        #region DeleteVenue
        public NonQueryResult DeleteVenue(Guid venueId)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM venue WHERE venue_id = '{0}'", venueId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region GetVenue
         public QueryResult<Venue> GetVenue(Guid venueId)
        {
            QueryResult<Venue> result = new QueryResult<Venue>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT venue_id, venue_name, special_needs_venue, active FROM venue ");
                query.AppendFormat("WHERE venue_id = '{0}'", venueId);
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Venue>(query.ToString(), GetVenueSelector).FirstOrDefault();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;                
            }

            return result;
        }
        #endregion

        #region GetVenues
        public QueryResult<List<Venue>> GetVenues()
        {
            QueryResult<List<Venue>> result = new QueryResult<List<Venue>>();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT venue_id, venue_name, special_needs_venue, active FROM venue");
                result.CommandText = query.ToString();
                result.Result = ExecuteReader<Venue>(query.ToString(), GetVenueSelector).ToList();
                result.Completed = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;                
            }

            return result;
        }
        #endregion

        #region InsertVenue
        public NonQueryResult InsertVenue(Venue venue)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("INSERT INTO venue (venue_id, venue_name, special_needs_venue, active) ");
            query.AppendFormat("VALUES('{0}','{1}','{2}','{3}')", venue.VenueId, TextHelper.EscapeSingleQuotes(venue.Name), venue.SpecialNeedsVenue, venue.Active);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #region UpdateVenue
        public NonQueryResult UpdateVenue(Venue venue)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE venue ");
            query.AppendLine("SET ");
            query.AppendFormat("venue_name = '{0}', ", TextHelper.EscapeSingleQuotes(venue.Name));
            query.AppendFormat("special_needs_venue = '{0}', ", venue.SpecialNeedsVenue);
            query.AppendFormat("active = '{0}' ", venue.Active);
            query.AppendFormat("WHERE venue_id = '{0}'", venue.VenueId);
            return ExecuteNonQuery(query.ToString());
        }
        #endregion

        #endregion
    }
}
