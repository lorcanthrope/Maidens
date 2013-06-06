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
    public partial class RoundsControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;
        
        private DataGridViewCell _currentlySelectedCell;
        private DataGridViewColumn _lastSelectedColumn;
        private int _lastSelectedColumnWidth;

        private Round _currentlySelectedRound;
        #endregion

        #region Construction
        public RoundsControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _context = context;
            _tournament = tournament;
            _context = context;

            LoadRounds();
            btn_LoadRound.Click += (sender, e) => { LoadRound(); };

            dgv_Round.Visible = false;
            btn_CreateNewRound.Click += (sender, e) => { CreateNewRound(); };

            btn_RerunBallots.Click += (sender, e) => { RerunBallots(); };
        }
        #endregion

        #region CellClicked
        private void CellClicked(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if(_lastSelectedColumn != null)
            {
                _lastSelectedColumn.Width = _lastSelectedColumnWidth;
                _lastSelectedColumn = null;
            }

            DataGridViewCell cell = dgv_Round.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if(cell.PreferredSize.Width > cell.Size.Width)
            {
                _lastSelectedColumn = dgv_Round.Columns[e.ColumnIndex];
                _lastSelectedColumnWidth = _lastSelectedColumn.Width;
                dgv_Round.Columns[e.ColumnIndex].Width = cell.PreferredSize.Width;
            }
        }
        #endregion

        #region CreateNewRound
        private void CreateNewRound()
        {
            if(txt_Motion.ReadOnly)
            {
                txt_Motion.ReadOnly = false;
                txt_Motion.Text = string.Empty;
                dgv_Round.Visible = false;

                MessageBox.Show("Enter the motion for debate in this round");
                return;
            }

            string motion = txt_Motion.Text;

            if(string.IsNullOrWhiteSpace(motion))
            {
                MessageBox.Show("Please enter a motion for debate in this round");
                return;
            }

            if(_context.Speakers.Count(s => s.Active) %6 != 0)
            {
                MessageBox.Show(string.Format("There are an incorrect number of speakers active to draw a round. You require either {0} or {1} and you currently have {2}",Math.Floor(((decimal)_context.Speakers.Count(s => s.Active)/6 - 1)*6),Math.Ceiling(((decimal)_context.Speakers.Count(s => s.Active)/6 + 1) * 6),_context.Speakers.Count(s => s.Active)));
                return;
            }

            if(_context.Speakers.Count/6 > _context.Venues.Count(v => v.Active))
            {
                MessageBox.Show(string.Format("There are not enough venues active to draw a round. You require {0} and you currently have {1}.", _context.Speakers.Count(s => s.Active) / 6, _context.Venues.Count(v => v.Active)));
                return;
            }

            bool isPowerPaired = chk_Powerpair.Checked;
            Round round = RoundHelper.DrawRound(_tournament, motion, isPowerPaired);
            LoadRounds();
            cmb_Rounds.SelectedIndex = cmb_Rounds.Items.Count - 1;
            LoadRound();
        }
        #endregion

        #region GetDraggedValue
        private void GetDraggedValue(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(!e.Button.HasFlag(MouseButtons.Left)) return;

            if(e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewCell cell = dgv_Round.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if(cell.Value != null)
            {
                _currentlySelectedCell = cell;
                Cursor = Cursors.Hand;
            }
            else
            {
                _currentlySelectedCell = null;
                Cursor = Cursors.Arrow;
            }
        }
        #endregion

        #region LoadRound
        private void LoadRound()
        {
            Round selectedRound = (Round)cmb_Rounds.SelectedItem;

            if(selectedRound == null) return;

            DialogResult result = MessageBox.Show(string.Format("Would you like to load {0}?", selectedRound), "Load Round", MessageBoxButtons.YesNo);
            if(result == DialogResult.No) return;

            dgv_Round.Visible = true;
            dgv_Round.Width = Width;
            dgv_Round.Rows.Clear();
            dgv_Round.Columns.Clear();

            int columnWidth = Width/12;
            
            DataGridViewColumn c = new DataGridViewColumn()
            {
                CellTemplate = new DataGridViewTextBoxCell(),
                Frozen = true,
                HeaderCell = new DataGridViewColumnHeaderCell(),
                HeaderText = "Venue",
                Name = "Venue",
                ReadOnly = true,
                Width = columnWidth
            };
            dgv_Round.Columns.Add(c);

            foreach (Position p in Enum.GetValues(typeof(Position)))
            {
                if(p == Position.Invalid) continue;
                dgv_Round.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Frozen = true,
                    HeaderCell = new DataGridViewColumnHeaderCell(),
                    HeaderText = p.ToString(),
                    Name = p.ToString(),
                    ReadOnly = true,
                    Width = columnWidth
                });
            } 
           
            for(int i = 1; i <= 5;i++)
            {
                dgv_Round.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    CellTemplate = new DataGridViewTextBoxCell(),
                    Frozen = true,
                    HeaderCell = new DataGridViewColumnHeaderCell(),
                    HeaderText = string.Format("Adj{0}", i),
                    Name = string.Format("Adj{0}",i),
                    ReadOnly = true,
                    Width = columnWidth
                });
            }

            
            SqlHelper helper = new SqlHelper(_tournament.Database);

            List<Debate> debates = helper.GetDebates(selectedRound.RoundId).Result;
            List<Round> validRounds = helper.GetRounds().Result.Where(r => r.RoundNumber < selectedRound.RoundNumber).ToList();
            List<Debate> validDebates = helper.GetDebates().Result.Where(d => validRounds.Select(r => r.RoundId).Contains(d.RoundId)).ToList();
            
            foreach (Debate debate in debates.OrderBy(a => _context.Venues.First(v => v.VenueId == a.VenueId).Name))
            {
                DataGridViewRow r = new DataGridViewRow()
                {
                    Tag = debate.DebateId
                };
                int rowIndex = dgv_Round.Rows.Add(r);
                Venue venue = _context.Venues.FirstOrDefault(v => v.VenueId == debate.VenueId);
                dgv_Round.Rows[rowIndex].Cells[0].Value = venue != null ? string.Format("{0}{1}", venue.SpecialNeedsVenue ? "[SN] " : string.Empty, venue.Name) : "Venue not found";

                List<SpeakerDraw> speakerDraws = helper.GetSpeakerDrawsByDebate(debate.DebateId).Result;
                foreach (SpeakerDraw speakerDraw in speakerDraws)
                {
                    Speaker speaker = _context.Speakers.First(s => s.SpeakerId.Equals(speakerDraw.SpeakerId));
                    int points = helper.GetSpeakerDraws(speakerDraw.SpeakerId).Result.Where(sd => validDebates.Select(d => d.DebateId).Contains(sd.DebateId)).Sum(sd => sd.Result.Points());
                    dgv_Round.Rows[rowIndex].Cells[speakerDraw.Position.ToString()].Value = string.Format("{0}{1} ({2})",speaker.SpecialNeeds ? "[SN] " : string.Empty, speaker.Name, points);
                    dgv_Round.Rows[rowIndex].Cells[speakerDraw.Position.ToString()].Tag = speakerDraw.DrawId;
                }

                List<JudgeDraw> judgeDraws = helper.GetJudgeDrawsByDebate(debate.DebateId).Result.OrderBy(jd => jd.Number).ToList();                
                int i = 1;
                foreach(JudgeDraw judgeDraw in judgeDraws)
                {
                    Judge j = _context.Judges.First(a => a.JudgeId.Equals(judgeDraw.JudgeId));
                    dgv_Round.Rows[rowIndex].Cells[string.Format("Adj{0}", i)].Value = string.Format("{0} ({1})",  j.Name, (int)j.Level);
                    dgv_Round.Rows[rowIndex].Cells[string.Format("Adj{0}", i++)].Tag = judgeDraw.DrawId;
                }
            }

            dgv_Round.CellClick += (sender, e) => { CellClicked(sender, e); };

            dgv_Round.CellMouseDown += (sender, e) => { GetDraggedValue(sender, e); };
            dgv_Round.CellMouseUp += (sender, e) => { MoveDraggedValue(sender, e); };

            txt_Motion.Text = selectedRound.Motion;
            txt_Motion.ReadOnly = true;

            _currentlySelectedRound = selectedRound;

            ShowClashes(selectedRound.RoundId);
        }
        #endregion

        #region LoadRounds
        private void LoadRounds()
        {
            cmb_Rounds.Items.Clear();

            SqlHelper helper = new SqlHelper(_tournament.Database);
            List<Round> rounds = helper.GetRounds().Result;

            foreach (Round round in rounds.OrderBy(r => r.RoundNumber))
            {                
                cmb_Rounds.Items.Add(round);
            }

            if(cmb_Rounds.Items.Count > 0)
                cmb_Rounds.SelectedIndex = 0;
        }
        #endregion

        #region MoveDraggedValue
        private void MoveDraggedValue(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(!e.Button.HasFlag(MouseButtons.Left)) return;

            if(_currentlySelectedCell == null) return;

            if(_currentlySelectedCell.ColumnIndex == e.ColumnIndex && _currentlySelectedCell.RowIndex == e.RowIndex) return;

            SqlHelper helper = new SqlHelper(_tournament.Database);

            bool isVenueDrag = _currentlySelectedCell.ColumnIndex == dgv_Round.Columns["Venue"].Index && dgv_Round.Rows[e.RowIndex].Tag != null;

            if(isVenueDrag)
            {
                Guid draggedDebateId = (Guid) dgv_Round.Rows[_currentlySelectedCell.RowIndex].Tag;
                Guid debateToSwitchItWithId = (Guid) dgv_Round.Rows[e.RowIndex].Tag;
                Debate draggedDebate = helper.GetDebate(draggedDebateId).Result;
                Guid draggedVenue = draggedDebate.VenueId;
                Debate debateToSwitchItWith = helper.GetDebate(debateToSwitchItWithId).Result;
                Guid switchedVenue = debateToSwitchItWith.VenueId;                
                draggedDebate.VenueId = switchedVenue;
                debateToSwitchItWith.VenueId = draggedVenue;
                helper.UpdateDebate(draggedDebate);
                helper.UpdateDebate(debateToSwitchItWith);
                SwitchRows(e.RowIndex, _currentlySelectedCell.RowIndex);                
            }

            bool isAdjudicatorDrag = dgv_Round.Columns[_currentlySelectedCell.ColumnIndex].Name.StartsWith("Adj") && dgv_Round.Columns[e.ColumnIndex].Name.StartsWith("Adj");

            if(isAdjudicatorDrag)
            {
                Guid draggedJudgeDrawId = dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Tag != null ? (Guid)dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Tag : Guid.Empty;
                if(draggedJudgeDrawId.Equals(Guid.Empty))
                {
                    _currentlySelectedCell = null;
                    Cursor = Cursors.Arrow;
                    return;
                }

                bool doesDraggedRowPossiblyNeedToBeRearranged = false;

                for(int i = int.Parse(dgv_Round.Columns[_currentlySelectedCell.ColumnIndex].Name.Replace("Adj",string.Empty)) + 1; i <=5;i++)
                {
                    DataGridViewCell nextDraggedCell = dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[string.Format("Adj{0}", i)];
                    if(nextDraggedCell.Value != null)
                    {
                        doesDraggedRowPossiblyNeedToBeRearranged = true;
                        break;
                    }
                }
                JudgeDraw draggedJudgeDraw = helper.GetJudgeDraw(draggedJudgeDrawId).Result;

                bool areWeSwitchingWithAnotherCell = true;
                JudgeDraw judgeDrawToSwitchItWith;
                int columnIndexToSwitchTo = e.ColumnIndex;

                int adjudicatorNumber = int.Parse(dgv_Round.Columns[e.ColumnIndex].Name.Replace("Adj",string.Empty));
                for (int i = 1; i < adjudicatorNumber;i++ )
                {
                    if (dgv_Round.Rows[e.RowIndex].Cells[string.Format("Adj{0}", i)].Value == null)
                    {
                        columnIndexToSwitchTo = dgv_Round.Columns[string.Format("Adj{0}", i)].Index;
                        break;
                    }
                }
                
                Guid judgeDrawToSwitchItWithId = dgv_Round.Rows[e.RowIndex].Cells[columnIndexToSwitchTo].Tag != null ? (Guid)dgv_Round.Rows[e.RowIndex].Cells[columnIndexToSwitchTo].Tag : Guid.Empty;
                if (judgeDrawToSwitchItWithId.Equals(Guid.Empty))
                {
                    judgeDrawToSwitchItWith = null;
                    areWeSwitchingWithAnotherCell = false;
                }
                else
                {
                    judgeDrawToSwitchItWith = helper.GetJudgeDraw(judgeDrawToSwitchItWithId).Result;
                }

                bool judgeInSameDebate;

                if(judgeDrawToSwitchItWith == null)
                {
                    if(_currentlySelectedCell.RowIndex == e.RowIndex)
                    {
                        _currentlySelectedCell = null;
                        Cursor = Cursors.Arrow;
                        return;
                    }
                    else
                    {
                        judgeInSameDebate = false;
                    }
                }
                else
                {
                    judgeInSameDebate = draggedJudgeDraw.DebateId.Equals(judgeDrawToSwitchItWith.DebateId);
                }

                int newJudgeNumber = 0;
                if(judgeDrawToSwitchItWith != null)
                {
                    newJudgeNumber = judgeDrawToSwitchItWith.Number;
                }
                else
                {
                    newJudgeNumber = int.Parse(dgv_Round.Columns[e.ColumnIndex].Name.Replace("Adj", string.Empty));
                }


                int draggedJudgeDrawNumber = draggedJudgeDraw.Number;
                int judgeDrawToSwitchItWithNumber = newJudgeNumber;
                draggedJudgeDraw.Number = judgeDrawToSwitchItWithNumber;
                if (areWeSwitchingWithAnotherCell)
                {
                    judgeDrawToSwitchItWith.Number = draggedJudgeDrawNumber;
                }

                if(!judgeInSameDebate)
                {
                    Guid draggedJudgeDrawDebateId = draggedJudgeDraw.DebateId;
                    Guid judgeDrawToSwitchItWithDebateId = (Guid)dgv_Round.Rows[e.RowIndex].Tag;
                    draggedJudgeDraw.DebateId = judgeDrawToSwitchItWithDebateId;
                    if(areWeSwitchingWithAnotherCell)
                    {                        
                        judgeDrawToSwitchItWith.DebateId = draggedJudgeDrawDebateId;
                    }
                }

                helper.UpdateJudgeDraw(draggedJudgeDraw);
                if(areWeSwitchingWithAnotherCell)
                {
                    helper.UpdateJudgeDraw(judgeDrawToSwitchItWith);
                }

                string draggedJudgeDrawValue = (string)dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Value;
                string judgeDrawToSwitchItWithValue = (string) dgv_Round.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Value = judgeDrawToSwitchItWithValue;
                Guid draggedTag = (Guid)dgv_Round[_currentlySelectedCell.ColumnIndex, _currentlySelectedCell.RowIndex].Tag;
                Guid switchTag = (Guid)dgv_Round[e.ColumnIndex, e.RowIndex].Tag;
                dgv_Round[_currentlySelectedCell.ColumnIndex, _currentlySelectedCell.RowIndex].Tag = switchTag;
                dgv_Round[e.ColumnIndex, e.RowIndex].Tag = draggedTag;


                if(doesDraggedRowPossiblyNeedToBeRearranged && !areWeSwitchingWithAnotherCell)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        DataGridViewCell thisCell = dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[string.Format("Adj{0}", i)];
                        DataGridViewCell nextCell = dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[string.Format("Adj{0}", i+1)];
                        if(thisCell.Value == null)
                        {
                            thisCell.Value = nextCell.Value;
                            thisCell.Tag = nextCell.Tag;
                            Guid nextId = nextCell.Tag != null ? (Guid)nextCell.Tag : Guid.Empty;
                            if(nextId != Guid.Empty)
                            {
                                JudgeDraw nextJudgeDraw = helper.GetJudgeDraw(nextId).Result;
                                nextJudgeDraw.Number = i;
                                helper.UpdateJudgeDraw(nextJudgeDraw);
                            }
                            nextCell.Value = null;
                            nextCell.Tag = null;
                        }

                    }
                }

                dgv_Round.Rows[e.RowIndex].Cells[columnIndexToSwitchTo].Value = draggedJudgeDrawValue; 
                Refresh();                   
            }

            Position position;
            bool isSpeakerDrag = Enum.TryParse(dgv_Round.Columns[_currentlySelectedCell.ColumnIndex].Name, out position) && Enum.TryParse(dgv_Round.Columns[_currentlySelectedCell.ColumnIndex].Name, out position);

            if(isSpeakerDrag)
            {
                Guid draggedSpeakerDrawId = (Guid)dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Tag;
                SpeakerDraw draggedSpeakerDraw = helper.GetSpeakerDraw(draggedSpeakerDrawId).Result;
                Guid speakerDrawToSwitchItWithId = (Guid) dgv_Round.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                SpeakerDraw speakerDrawToSwitchItWith = helper.GetSpeakerDraw(speakerDrawToSwitchItWithId).Result;

                bool speakerInSameDebate = draggedSpeakerDraw.DebateId.Equals(speakerDrawToSwitchItWith.DebateId);

                Position draggedSpeakerDrawPosition = draggedSpeakerDraw.Position;
                Position speakerDrawToSwitchItWithPosition = speakerDrawToSwitchItWith.Position;
                draggedSpeakerDraw.Position = speakerDrawToSwitchItWithPosition;
                speakerDrawToSwitchItWith.Position = draggedSpeakerDrawPosition;

                if(!speakerInSameDebate)
                {
                    Guid draggedSpeakerDrawDebateId = draggedSpeakerDraw.DebateId;
                    Guid speakerDrawToSwitchItWithDebateId = speakerDrawToSwitchItWith.DebateId;
                    draggedSpeakerDraw.DebateId = speakerDrawToSwitchItWithDebateId;
                    speakerDrawToSwitchItWith.DebateId = draggedSpeakerDrawDebateId;
                }

                helper.UpdateSpeakerDraw(draggedSpeakerDraw);
                helper.UpdateSpeakerDraw(speakerDrawToSwitchItWith);

                string draggedSpeakerDrawValue = (string)dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Value;
                string speakerDrawToSwitchItWithValue = (string)dgv_Round.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                dgv_Round.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Value = speakerDrawToSwitchItWithValue;
                dgv_Round.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = draggedSpeakerDrawValue;
            }
            
            ShowClashes(_currentlySelectedRound.RoundId);

            _currentlySelectedCell = null;
            Cursor = Cursors.Arrow;
        }
        #endregion

        #region ReloadRow
        private void ReloadRows(params int[] rowIndices)
        {
            
        }        
        #endregion

        #region RerunBallots
        private void RerunBallots()
        {
            if(_currentlySelectedRound == null) return;

            DialogResult result = MessageBox.Show("Do you want to rerun the ballots for Round " + _currentlySelectedRound.RoundNumber + "?", "Rerun Ballots", MessageBoxButtons.YesNo);
            if(result == DialogResult.Yes)
            {
                MicrosoftHelper.CreateBallots(_tournament, _context, _currentlySelectedRound.RoundId);
                MicrosoftHelper.CreateOverview(_tournament, _context, _currentlySelectedRound.RoundId);
                MicrosoftHelper.CreateRotation(_tournament, _context, _currentlySelectedRound.RoundNumber);
                HtmlHelper.CreateDisplay(_tournament, _context, _currentlySelectedRound.RoundId);
                MessageBox.Show("Ballots have been rerun!");
            }
        }
        #endregion

        #region ShowClashes
        private void ShowClashes(Guid roundId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);

            List<Guid> debateIds = new List<Guid>();

            //List<Guid> debateIds = helper.GetDebates(roundId).Result.Select(d => d.DebateId).ToList();

            foreach (DataGridViewRow row in dgv_Round.Rows)
            {
                debateIds.Add((Guid)row.Tag);
            }

            foreach (Guid debateId in debateIds)
            {
                DataGridViewRow row = dgv_Round.Rows.Cast<DataGridViewRow>().ToList().First(r => r.Tag.Equals(debateId));

                List<DataGridViewCell> cells = row.Cells.Cast<DataGridViewCell>().ToList();
                
                cells.ForEach(c => c.Style.BackColor = Color.White);

                List<SpeakerDraw> speakerDraws = helper.GetSpeakerDrawsByDebate(debateId).Result;

                List<Speaker> speakers = _context.Speakers.Where(s => speakerDraws.Select(sd => sd.SpeakerId).Contains(s.SpeakerId)).ToList();

                List<Guid> speakerInstitutionIds = speakers.Select(s => s.InstitutionId).Distinct().ToList();

                List<JudgeDraw> judgeDraws = helper.GetJudgeDrawsByDebate(debateId).Result;

                List<Judge> judges = _context.Judges.Where(j => judgeDraws.Select(jd => jd.JudgeId).Contains(j.JudgeId)).ToList();

                List<Guid> judgeInstitutionIds = judges.Where(j => j.InstitutionId.HasValue).Select(j => j.InstitutionId.Value).Distinct().ToList();

                foreach (Guid institutionId in judgeInstitutionIds.Intersect(speakerInstitutionIds))
                {
                    List<Judge> clashedJudges = judges.Where(j => j.InstitutionId.Equals(institutionId)).ToList();
                    List<JudgeDraw> clashedJudgeDraws = judgeDraws.Where(jd => clashedJudges.Select(j => j.JudgeId).Contains(jd.JudgeId)).ToList();
                    foreach (JudgeDraw clashedJudgeDraw in clashedJudgeDraws)
                    {
                        DataGridViewCell cell = cells.Where(c => c.Tag != null).First(c => clashedJudgeDraw.DrawId.Equals((Guid) c.Tag));
                        cell.Style.BackColor = Color.OrangeRed;
                    }
                }

                Refresh();
            }

        }
        #endregion

        #region SwitchRows
        private void SwitchRows(int rowA, int rowB)
        {
            int topRowIndex = Math.Max(rowA, rowB);
            int bottomRowIndex = Math.Min(rowA, rowB);

            DataGridViewRow topRow = dgv_Round.Rows[topRowIndex];
            DataGridViewRow bottomRow = dgv_Round.Rows[bottomRowIndex];

            foreach (DataGridViewCell cell in topRow.Cells)
            {                
                if(cell.ColumnIndex == 0) continue;

                string cellValue = (string)cell.Value;
                DataGridViewCell otherCell = bottomRow.Cells[cell.ColumnIndex];
                string otherValue = (string)otherCell.Value;
                cell.Value = otherValue;
                otherCell.Value = cellValue;
            }
            Refresh();
        }
        #endregion        
    }
}
