using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Maidens.Models;
using Maidens.Helpers;
using System.IO;

namespace Maidens.Test
{
    [TestClass]
    public class MicrosoftTest
    {
        [TestMethod]
        public void TabTest()
        {
            Tournament tournament = Tournament.Parse(File.ReadAllText("E:\\Tournament\\tara.trn"));
            SqlHelper helper = new SqlHelper(tournament.Database);
            DataContext context = helper.GetDataContext().Result;
            MicrosoftHelper.WriteTabSoFar(tournament, context, 1);
        }

        [TestMethod]
        public void BallotTest()
        {
            Tournament tournament = Tournament.Parse(File.ReadAllText("E:\\Tournament\\tara.trn"));
            SqlHelper helper = new SqlHelper(tournament.Database);
            DataContext context = helper.GetDataContext().Result;
            Round round = helper.GetRounds().Result.First();
            
            MicrosoftHelper.CreateBallots(tournament, context, round.RoundId);
        }
    }
}