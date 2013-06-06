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
    public class HtmlTest
    {
        [TestMethod]
        public void DisplayTest()
        {
            Tournament tournament = Tournament.Parse(File.ReadAllText("E:\\Tournament\\tara.trn"));
            SqlHelper helper = new SqlHelper(tournament.Database);
            DataContext context = helper.GetDataContext().Result;
            Round round = helper.GetRounds().Result.First();
            HtmlHelper.CreateDisplay(tournament, context, round.RoundId);
        }
    }
}
