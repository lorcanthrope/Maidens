using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Maidens.Helpers;
using Maidens.Models;

namespace Maidens.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SqlTest
    {
        #region Constants
        private const string TournamentLocation = "E:\\Tournament\\Tournament_yhkvrgvk.db;";
        #endregion

        #region CreateRandomName
        private string CreateRandomName()
		{
            Random r = new Random();
		    StringBuilder randomName = new StringBuilder();
            for(int i = 0;i < r.Next(6,10); i++)
            {
                randomName.Append(char.ConvertFromUtf32(r.Next(97, 122)));
            }
		    return randomName.ToString();
		}
	    #endregion

        #region InitializeDatabase
        [TestMethod]
        public void InitializeDatabase()
        {
            string location = string.Concat("E:\\Tournament\\Tournament_", CreateRandomName(), ".db");
            SqlHelper helper = new SqlHelper(location, true);
            
            Institution i1 = new Institution()
                                {
                                    InstitutionId = Guid.NewGuid(),
                                    Name = "University of Limerick"
                                };
            helper.InsertInstitution(i1);
            
            Institution i2 = new Institution()
                                 {
                                     InstitutionId = Guid.NewGuid(),
                                     Name = "University College Cork"
                                 };
            helper.InsertInstitution(i2);

            Speaker s1 = new Speaker()
                            {
                                InstitutionId = i1.InstitutionId,
                                Name = "Maurice Cotter",
                                SpeakerId = Guid.NewGuid()
                            };
            helper.InsertSpeaker(s1);

            Speaker s2 = new Speaker()
                             {
                                 InstitutionId = i2.InstitutionId,
                                 Name = "Aaron Vickery",
                                 SpeakerId = Guid.NewGuid()
                             };
            helper.InsertSpeaker(s2);

            Venue v1 = new Venue()
                           {
                               Name = "Jean Monnet Theatre",
                               VenueId = Guid.NewGuid()
                           };
            helper.InsertVenue(v1);

            Venue v2 = new Venue()
                           {
                               Name = "Jonathan Swift Theatre",
                               VenueId = Guid.NewGuid(),
                               SpecialNeedsVenue = true
                           };
            helper.InsertVenue(v2);

            Judge j1 = new Judge()
                           {
                               JudgeId = Guid.NewGuid(),
                               Name = "Lorcan O'Neill",
                               Level = JudgeLevel.IrrChair,
                               InstitutionId = i1.InstitutionId
                           };
            helper.InsertJudge(j1);

            Judge j2 = new Judge()
                           {
                               JudgeId = Guid.NewGuid(),
                               Level = JudgeLevel.TopChair,
                               Name = "Stephen Egan"
                           };
            helper.InsertJudge(j2);

        }
        #endregion

        #region Institution
        
        #region CreateInstitution
        private Institution CreateInstitution()
        {
            return new Institution()
                       {
                           InstitutionId = Guid.NewGuid(),
                           Name = CreateRandomName()
                       };
        }
        #endregion

        #region GetInstitutions
        [TestMethod]
        public void GetInstitutions()
        {
            SqlHelper helper = new SqlHelper(TournamentLocation);
            QueryResult<List<Institution>> result = helper.GetInstitutions();
        }
        #endregion

        #region InsertInstitution
        [TestMethod]
        public void InsertInstitution()
        {
            Institution i = CreateInstitution();
            SqlHelper helper = new SqlHelper(TournamentLocation);
            helper.InsertInstitution(i);
        }
        #endregion
        
        #endregion

        #region Judge

        #region CreateJudge
        private Judge CreateJudge()
        {
            return new Judge()
                       {
                           JudgeId = Guid.NewGuid(),
                           Name = "Jimmy " + CreateRandomName(),
                           Level = JudgeLevel.IrrChair
                       };
        }
        #endregion

        #region GetJudge
        [TestMethod]
        public void GetJudge()
        {
            Guid judgeId = Guid.Parse("ac578446-cf45-49ac-b90b-8275b8bc26d1");
            SqlHelper helper = new SqlHelper(TournamentLocation);
            QueryResult<Judge> result = helper.GetJudge(judgeId);            
        }
        #endregion

        #region GetJudges
        [TestMethod]
        public void GetJudges()
        {
            SqlHelper helper = new SqlHelper(TournamentLocation);
            QueryResult<List<Judge>> result = helper.GetJudges();
        }
        #endregion

        #region InsertJudge
        [TestMethod]
        public void InsertJudge()
        {
            Judge j = CreateJudge();
            SqlHelper helper = new SqlHelper(TournamentLocation);
            helper.InsertJudge(j);
        }
        #endregion

        #endregion

        #region Speaker

        #region CreateSpeaker
        private Speaker CreateSpeaker()
        {
            return new Speaker()
                       {
                           Name = CreateRandomName(),
                           InstitutionId = Guid.Parse("3ff78f20-b7c8-4f8a-8ecc-4b113b9bc4a1"),
                           SpeakerId = Guid.NewGuid()
                       };
        }
        #endregion

        #region GetSpeaker
        [TestMethod]
        public void GetSpeaker()
        {
            Guid speakerId = Guid.Parse("8a4b4abd-3f2e-4f1e-bbb6-56308b4ca531");
            SqlHelper helper = new SqlHelper(TournamentLocation);
            QueryResult<Speaker> result = helper.GetSpeaker(speakerId);
        }
        #endregion

        #region GetSpeakers
        [TestMethod]
        public void GetSpeakers()
        {
            SqlHelper helper = new SqlHelper(TournamentLocation);
            QueryResult<List<Speaker>> result = helper.GetSpeakers();
        } 
        #endregion

        #region InsertSpeaker
        [TestMethod]
        public void InsertSpeaker()
        {
            Speaker s = CreateSpeaker();
            SqlHelper helper = new SqlHelper(TournamentLocation);
            helper.InsertSpeaker(s);
        }
        #endregion

        #region UpdateSpeaker
        [TestMethod]
        public void UpdateSpeaker()
        {
            SqlHelper helper = new SqlHelper(TournamentLocation);
            Institution i = helper.GetInstitutions().Result.First();
            
            Speaker speaker = CreateSpeaker();
            speaker.InstitutionId = i.InstitutionId;
            helper.InsertSpeaker(speaker);
            
            Speaker speakerRetrieved = helper.GetSpeaker(speaker.SpeakerId).Result;
            Assert.AreEqual(speaker, speakerRetrieved);

            speaker.Name = speaker.Name + "Updated";
            helper.UpdateSpeaker(speaker);

            Speaker newSpeakerRetrieved = helper.GetSpeaker(speaker.SpeakerId).Result;
            Assert.AreNotEqual(speakerRetrieved, newSpeakerRetrieved);
        }
        #endregion

        #endregion

        #region Venue

        #region CreateVenue
        private Venue CreateVenue()
        {
            return new Venue()
                       {
                           Name = CreateRandomName(),
                           VenueId = Guid.NewGuid()
                       };
        }
        #endregion

        #region GetVenues
        [TestMethod]
        public void GetVenues()
        {
            SqlHelper helper = new SqlHelper(TournamentLocation);
            QueryResult<List<Venue>> result = helper.GetVenues();
        }
        #endregion

        #region InsertVenue
        [TestMethod]
        public void InsertVenue()
        {
            Venue v = CreateVenue();
            SqlHelper helper = new SqlHelper(TournamentLocation);
            helper.InsertVenue(v);
        }
        #endregion


        #endregion
    }
}
