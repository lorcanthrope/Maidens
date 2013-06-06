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
    public partial class ResultsControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;

        private Guid _currentlyLoadedDebate;
        private Guid _currentlyLoadedRound;
        #endregion

        #region Construction
        public ResultsControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _tournament = tournament;
            _context = context;
            PopulateRoundsComboBox();

            btn_LoadRound.Click += (sender, e) => { LoadRoundClick(); };

            dgv_Judges.Visible = false;
            dgv_Debate.Visible = false;
            dgv_Debate.RowTemplate.ReadOnly = false;
            btn_Save.Visible = false;
        }
        #endregion

        #region LoadDebate
        private void LoadDebate(Guid debateId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);

            List<SpeakerDraw> speakerDraws = helper.GetSpeakerDrawsByDebate(debateId).Result.OrderBy(s => s.Position).ToList();
            List<JudgeDraw> judgeDraws = helper.GetJudgeDrawsByDebate(debateId).Result.OrderBy(j => j.Number).ToList();
            
            dgv_Debate.Visible = true;
            dgv_Debate.Rows.Clear();
            dgv_Debate.Columns.Clear();

            int columnWidth = dgv_Debate.Width/4;

            DataGridViewColumn nameColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),                
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Name",
                Name = "Name",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Debate.Columns.Add(nameColumn);

            DataGridViewColumn positionColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),                
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Position",
                Name = "Position",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Debate.Columns.Add(positionColumn);

            DataGridViewColumn resultColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewComboBoxCell()
                                   {
                                       DataSource = Result.Unspecified.ToArray().Select(r => new { Display = r.ToString(), Value = r}).ToList(),
                                       DisplayMember = "Display",
                                       DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,        
                                       DropDownWidth = 100,
                                       MaxDropDownItems = 8,
                                       ReadOnly = false,
                                       ValueMember = "Value",
                                       ValueType = typeof(Result)
                                   },                                      
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Result",
                Name = "Result",        
                //ReadOnly = false,                
                Width = columnWidth
            };
            
            dgv_Debate.Columns.Add(resultColumn);

            DataGridViewColumn speakerPointsColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell()
                                   {
                                       //ReadOnly = false
                                   },                                
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Speaker Points",
                Name = "Speaker Points",      
                ReadOnly = false,
                Width = columnWidth
            };            
            dgv_Debate.Columns.Add(speakerPointsColumn);

            foreach (SpeakerDraw speakerDraw in speakerDraws)
            {
                Speaker s = _context.Speakers.First(sd => sd.SpeakerId.Equals(speakerDraw.SpeakerId));
                DataGridViewRow row = new DataGridViewRow()
                                          {
                                              ReadOnly = false
                                          };                
                int rowIndex = dgv_Debate.Rows.Add(row);
                dgv_Debate.Rows[rowIndex].ReadOnly = false;
                dgv_Debate.Rows[rowIndex].Tag = speakerDraw.DrawId;                
                dgv_Debate.Rows[rowIndex].Cells["Name"].Value = s.Name;
                dgv_Debate.Rows[rowIndex].Cells["Position"].Value = speakerDraw.Position.ToString();
                dgv_Debate.Rows[rowIndex].Cells["Result"].Value = speakerDraw.Result;                
                dgv_Debate.Rows[rowIndex].Cells["Speaker Points"].Value = speakerDraw.SpeakerPoints;                
            }

            dgv_Debate.EditingControlShowing += (sender, e) => { ValidateOnlyNumbericInput(sender, e); };
            dgv_Debate.CellValueChanged += (sender, e) => { ValidateSpeakerPoints(sender, e); };

            btn_Save.Visible = true;
            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { SaveDebate(sender, e); };

            _currentlyLoadedDebate = debateId;

            dgv_Judges.Visible = true;
            dgv_Judges.Rows.Clear();
            dgv_Judges.Columns.Clear();

            DataGridViewColumn judgeColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Judge",
                Name = "Judge",
                ReadOnly =  true,
                Width = dgv_Judges.Width/2
            };
            dgv_Judges.Columns.Add(judgeColumn);

            DataGridViewColumn judgeInstitutionColumn = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Institution",
                Name = "Institution",
                ReadOnly = true,
                Width = dgv_Judges.Width/2
            };
            dgv_Judges.Columns.Add(judgeInstitutionColumn);


            foreach (JudgeDraw judgeDraw in judgeDraws)
            {
                Judge j = _context.Judges.First(jd => jd.JudgeId.Equals(judgeDraw.JudgeId));
                DataGridViewRow row = new DataGridViewRow();
                int rowIndex = dgv_Judges.Rows.Add(row);
                dgv_Judges["Judge", rowIndex].Value = j.Name;
                dgv_Judges["Institution", rowIndex].Value = j.InstitutionId.HasValue ? _context.Institutions.First(i => i.InstitutionId.Equals(j.InstitutionId)).Name : "N/A";
            }

            Refresh();
        }
        #endregion

        #region LoadRoundClick
        private void LoadRoundClick()
        {
            Round currentlySelectedRound = (Round) cmb_Rounds.SelectedItem;

            if(currentlySelectedRound == null) return;

            if(_currentlyLoadedRound.Equals(currentlySelectedRound.RoundId))
            {
                return;
            }

            DialogResult result = MessageBox.Show(string.Format("Do you want to load {0}?", currentlySelectedRound), "Loading Round...", MessageBoxButtons.YesNo);
            if(result == DialogResult.No)
            {
                return;                
            }

            LoadRound(currentlySelectedRound.RoundId);
            _currentlyLoadedRound = currentlySelectedRound.RoundId;
        }
        #endregion

        #region LoadRound
        private void LoadRound(Guid roundId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);
            List<Debate> debates = helper.GetDebates(roundId).Result;

            tree_Rooms.Nodes.Clear();

            debates = debates.OrderBy(a => _context.Venues.First(v => v.VenueId.Equals(a.VenueId)).Name).ToList();

            foreach (Debate debate in debates)
            {
                TreeNode node = new TreeNode()
                {
                    Name = debate.DebateId.ToString(),
                    Text = _context.Venues.First(v => v.VenueId.Equals(debate.VenueId)).Name
                };

                bool roundIsComplete = helper.CountOfUnfinishedResults(debate.DebateId).Result == 0;
                
                if(roundIsComplete)
                {
                    node.BackColor = Color.GreenYellow;
                }
                else
                {
                    node.BackColor = Color.OrangeRed;
                }

                tree_Rooms.Nodes.Add(node);
            }

            tree_Rooms.AfterSelect += (sender, e) => { LoadDebate(Guid.Parse(e.Node.Name)); };
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

            if(cmb_Rounds.Items.Count > 0)
                cmb_Rounds.SelectedIndex = 0;
            Refresh();
        }
        #endregion

        #region SaveDebate
        private void SaveDebate(object sender, EventArgs e)
        {
            List<Result> results = new List<Result>();

            foreach (DataGridViewRow row in dgv_Debate.Rows)
            {
                results.Add((Result)row.Cells["Result"].Value);
            }

            if(results.Any(a => a == Result.Unspecified))
            {
                MessageBox.Show("Result unspecified - enter a value for each speaker");
                tree_Rooms.Nodes[_currentlyLoadedDebate.ToString()].BackColor = Color.OrangeRed;
                return;
            }


            if(results.Select(r => new KeyValuePair<Result, int>(r, results.Count(a => a.Equals(r)))).Any(s => s.Value > 1))
            {
                MessageBox.Show("Duplicate result is not permitted");
                tree_Rooms.Nodes[_currentlyLoadedDebate.ToString()].BackColor = Color.OrangeRed;
                return;
            }
            
            List<KeyValuePair<Result, int>> resultsWithSpeakerPoints = new List<KeyValuePair<Result, int>>();
            foreach (DataGridViewRow row in dgv_Debate.Rows)
            {
                resultsWithSpeakerPoints.Add(new KeyValuePair<Result, int>((Result)row.Cells["Result"].Value, (int)row.Cells["Speaker Points"].Value));               
            }

            int lastScore = 50;
            Result lastResult = Result.Unspecified;
            bool isfirstPass = true;

            foreach (KeyValuePair<Result, int> result in resultsWithSpeakerPoints.OrderByDescending(r => r.Key))
            {
                if(result.Value <= lastScore && !isfirstPass)
                {
                    MessageBox.Show(string.Format("{0}: {1} should be higher than {2}: {3}", result.Key, result.Value, lastResult, lastScore));
                    tree_Rooms.Nodes[_currentlyLoadedDebate.ToString()].BackColor = Color.OrangeRed;
                    return;
                }
                isfirstPass = false;
                lastScore = result.Value;
                lastResult = result.Key;
            }

            SqlHelper helper = new SqlHelper(_tournament.Database);

            foreach (DataGridViewRow row in dgv_Debate.Rows)
            {
                SpeakerDraw speakerDraw = helper.GetSpeakerDraw((Guid) row.Tag).Result;
                speakerDraw.Result = (Result) row.Cells["Result"].Value;
                speakerDraw.SpeakerPoints = (int)row.Cells["Speaker Points"].Value;
                helper.UpdateSpeakerDraw(speakerDraw);
            }

            tree_Rooms.Nodes[_currentlyLoadedDebate.ToString()].BackColor = Color.GreenYellow;
        }
        #endregion

        #region ValidateKeyPress
        private void ValidateKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        #endregion

        #region ValidateOnlyNumbericInput
        private void ValidateOnlyNumbericInput(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (!dgv_Debate.Columns[dgv_Debate.SelectedCells[0].ColumnIndex].Name.Equals("Speaker Points")) return;

            e.Control.KeyPress -= ValidateKeyPress;
            e.Control.KeyPress += (sender2, e2) =>{ ValidateKeyPress(sender2, e2);};           
        }
        #endregion

        #region ValidateSpeakerPoints
        private void ValidateSpeakerPoints(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgv_Debate.Columns[dgv_Debate.SelectedCells[0].ColumnIndex].Name.Equals("Speaker Points")) return;

            int value;

            object objValue = dgv_Debate[e.ColumnIndex, e.RowIndex].Value;

            if(objValue is string)
            {
                value = int.Parse(dgv_Debate[e.ColumnIndex, e.RowIndex].Value as string);
            }
            else
            {
                value = (int) dgv_Debate[e.ColumnIndex, e.RowIndex].Value;
            }
            
            
            if(value < 50)
            {
                dgv_Debate[e.ColumnIndex, e.RowIndex].Value = 50;
            }
            else if (value > 100)
            {
                dgv_Debate[e.ColumnIndex, e.RowIndex].Value = 100;
            }
            else
            {
                dgv_Debate[e.ColumnIndex, e.RowIndex].Value = value;
            }
        }
        #endregion
    }
}
