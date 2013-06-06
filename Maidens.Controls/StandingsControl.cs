using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Maidens.Helpers;
using Maidens.Models;

namespace Maidens.Controls
{
    public partial class StandingsControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;
        private Round _currentlyLoadedRound;
        #endregion

        #region Construction
        public StandingsControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _context = context;
            _tournament = tournament;

            dgv_Standings.Visible = false;

            PopulateRoundsComboBox();
            btn_Load.Click += (sender, e) => { LoadRound(); };
        }
        #endregion

        #region LoadRound
        private void LoadRound()
        {
            Round round = (Round) cmb_Rounds.SelectedItem;

            if(round == null) return;

            if(round.Equals(_currentlyLoadedRound))
            {
                return;
            }

            SqlHelper helper = new SqlHelper(_tournament.Database);

            List<Speaker> speakers = new List<Speaker>();
            speakers.AddRange(_context.Speakers);
            speakers.AsParallel().ForAll(s => s.Draws = helper.GetSpeakerDrawsUntilRoundNumber(s.SpeakerId, round.RoundNumber).Result);

            dgv_Standings.Visible = true;
            dgv_Standings.Columns.Clear();
            dgv_Standings.Rows.Clear();

            int columnWidth = dgv_Standings.Width/5;

            DataGridViewColumn numberColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "#",
                Name = "#",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Standings.Columns.Add(numberColumn);

            DataGridViewColumn nameColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Name",
                Name = "Name",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Standings.Columns.Add(nameColumn);

            DataGridViewColumn institutionColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Institution",
                Name = "Institution",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Standings.Columns.Add(institutionColumn);

            DataGridViewColumn individualPointsColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Individual Points",
                Name = "Individual Points",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Standings.Columns.Add(individualPointsColumn);

            DataGridViewColumn speakerPointsColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Speaker Points",
                Name = "Speaker Points",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Standings.Columns.Add(speakerPointsColumn);

            int i = 1;
            foreach (Speaker speaker in speakers.OrderByDescending(a => a.Draws.Sum(sd => sd.Result.Points())).ThenByDescending(s => s.Draws.Sum(sd => sd.SpeakerPoints)))
            {
                DataGridViewRow row = new DataGridViewRow();
                int rowIndex = dgv_Standings.Rows.Add(row);
                dgv_Standings["#", rowIndex].Value = i++;
                dgv_Standings["Name", rowIndex].Value = speaker.Name;
                dgv_Standings["Institution", rowIndex].Value = _context.Institutions.First(s => s.InstitutionId.Equals(speaker.InstitutionId)).Name;
                dgv_Standings["Individual Points", rowIndex].Value = speaker.Draws.Sum(a => a.Result.Points());
                dgv_Standings["Speaker Points", rowIndex].Value = speaker.Draws.Where(d => d.Result != Result.Unspecified).Sum(a => a.SpeakerPoints);
            }

            _currentlyLoadedRound = round;
        }
        #endregion

        #region PopulateRoundsComboBox
        private void PopulateRoundsComboBox()
        {
            cmb_Rounds.Items.Clear();
            SqlHelper helper = new SqlHelper(_tournament.Database);
            List<Round> rounds = helper.GetRounds().Result;
            foreach (Round round in rounds)
            {
                cmb_Rounds.Items.Add(round);
            }
            cmb_Rounds.SelectedIndex = cmb_Rounds.Items.Count - 1;
            Refresh();
        }
        #endregion
    }
}
