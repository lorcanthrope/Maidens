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
    public partial class JudgesControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;
        private bool _isChanged;
        private bool _initialActive;
        private string _initialName;
        private Guid? _initialInstitutionId;

        private DataGridViewCell _currentlySelectedCell;
        private DataGridViewColumn _lastSelectedColumn;
        private int _lastSelectedColumnWidth;
        #endregion

        #region Construction
        public JudgesControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _context = context;
            _tournament = tournament;                                   

            tabpages.Selected += (sender, e) => { TabpageSelected(sender, e); };
            TabpageSelected(null, new TabControlEventArgs(tabpage_JudgeEntry, 0, TabControlAction.Selected));
        }
        #endregion

        #region ActiveChanged
        private void ActiveChanged()
        {
            if(!chk_Active.Checked.Equals(_initialActive))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region CellClicked
        private void CellClicked(object sender, DataGridViewCellEventArgs e)
        {                      
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (_lastSelectedColumn != null)
            {
                _lastSelectedColumn.Width = _lastSelectedColumnWidth;
                _lastSelectedColumn = null;
            }

            DataGridViewCell cell = dgv_Ranked.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if(cell.PreferredSize.Width > cell.Size.Width)
            {                                
                _lastSelectedColumn = dgv_Ranked.Columns[e.ColumnIndex];
                _lastSelectedColumnWidth = _lastSelectedColumn.Width;
                dgv_Ranked.Columns[e.ColumnIndex].Width = cell.PreferredSize.Width;
            }
        }
        #endregion

        #region CreateNewJudge
        private void CreateNewJudge()
        {
            txt_Name.Text = "New Judge";
            ActiveControl = txt_Name;
            tree_Judges.SelectedNode = null;
            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { SaveNewJudge(); };
            _initialName = string.Empty;
            _initialInstitutionId = null;
            _isChanged = false;
        }
        #endregion

        #region DeleteJudge
        private void DeleteJudge(Guid judgeId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);

            NonQueryResult result = helper.DeleteJudge(judgeId);

            Judge toBeDeleted = _context.Judges.First(j => j.JudgeId == judgeId);

            _context.Judges = _context.Judges.Where(j => j.JudgeId != judgeId).ToList();

            ReloadJudges(toBeDeleted, ActionType.Delete);
        }
        #endregion

        #region GetDraggedValue
        private void GetDraggedValue(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(!e.Button.HasFlag(MouseButtons.Left)) return;

            if(e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewCell cell = dgv_Ranked.Rows[e.RowIndex].Cells[e.ColumnIndex];            
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

        #region HasInstitutionChanged
        private void HasInstitutionChanged()
        {
            cmb_Institution.Enabled = chk_HasInstitution.Checked;
        }
        #endregion

        #region InstitutionChanged
        private void InstitutionChanged()
        {
            if(_initialInstitutionId == null)
            {
                if(chk_HasInstitution.Checked)
                {
                    _isChanged = true;                                 
                }
            }
            else if(!chk_HasInstitution.Checked)
            {
                _isChanged = true;                
            }
            else
            {
                Institution i = (Institution) cmb_Institution.SelectedItem;
                if(!_initialInstitutionId.Value.Equals(i.InstitutionId))
                {
                    _isChanged = true;
                }
            }
        }
        #endregion

        #region LoadJudge
        private void LoadJudge(Guid judgeId)
        {
            Judge judge = _context.Judges.First(j => j.JudgeId == judgeId);

            txt_Name.Text = judge.Name;
            _initialName = judge.Name;

            bool hasInstitution = judge.InstitutionId.HasValue;
            bool active = judge.Active;
            if(hasInstitution)
            {
                chk_HasInstitution.Checked = true;
                Institution institution = _context.Institutions.First(i => i.InstitutionId == judge.InstitutionId);
                cmb_Institution.SelectedItem = institution;
            }
            else
            {
                chk_HasInstitution.Checked = false;
            }

            chk_Active.Checked = active;
            _initialActive = active;

            txt_Name.TextChanged += (sender, e) => { NameChanged(); };
            chk_HasInstitution.CheckedChanged += (sender, e) => { InstitutionChanged(); };
            cmb_Institution.SelectedValueChanged += (sender, e) => { InstitutionChanged(); };
            chk_Active.CheckedChanged += (sender, e) => { ActiveChanged(); };

            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { UpdateJudge(judgeId); };
            _isChanged = false;
            Refresh();
        }
        #endregion

        #region LoadJudges
        private void LoadJudges()
        {
            tree_Judges.Nodes.Clear();
            foreach (Judge j in _context.Judges.OrderBy(j => j.Name))
            {
                Guid judgeId = j.JudgeId;
                TreeNode node = new TreeNode()
                {
                    Name = j.JudgeId.ToString(),
                    Text = j.Name
                };
                node.ContextMenuStrip = new ContextMenuStrip();
                ToolStripItem item = new ToolStripButton("Delete");
                item.Click += (sender, e) => { DeleteJudge(judgeId); };
                node.ContextMenuStrip.Items.Add(item);
                if(!j.Active)
                {
                    node.BackColor = Color.OrangeRed;
                }
                tree_Judges.Nodes.Add(node);
            }

            tree_Judges.AfterSelect += (sender, e) => { LoadJudge(Guid.Parse(e.Node.Name)); };

            lbl_Count.Text = string.Format("{0} judges active, {1} judges inactive ", _context.Judges.Count(j => j.Active), _context.Judges.Count(j => !j.Active));

            Refresh();

        }
        #endregion

        #region LoadRankings
        private void LoadRankings()
        {
            dgv_Ranked.Width = Width;
            dgv_Ranked.Rows.Clear();
            dgv_Ranked.Columns.Clear();
            
            Array levelsArray = Enum.GetValues(typeof(JudgeLevel));
            IEnumerable<JudgeLevel> levels = levelsArray.Cast<JudgeLevel>().Where(j => j != JudgeLevel.Invalid).OrderByDescending(j => (int)j);

            int columnWidth = Width/levels.Count();
            
            foreach (JudgeLevel judgeLevel in levels)
            {
                DataGridViewColumn column = new DataGridViewColumn();  
                column.CellTemplate = new DataGridViewTextBoxCell();
                column.HeaderText = judgeLevel.ToString();
                column.HeaderCell = new DataGridViewColumnHeaderCell();
                column.Name = judgeLevel.ToString();
                column.Frozen = true;
                column.ReadOnly = true;
                column.Width = columnWidth;
                dgv_Ranked.Columns.Add(column);
            }
            
            DataGridViewRow r = new DataGridViewRow();            
            dgv_Ranked.Rows.Add(r);

            foreach (Judge judge in _context.Judges.OrderBy(j => j.Name))
            {
                bool entered = false;
                foreach(DataGridViewRow row in dgv_Ranked.Rows)
                {
                    if(row.Cells[judge.Level.ToString()].Value == null)
                    {
                        row.Cells[judge.Level.ToString()].Value = judge;
                        entered = true;
                        break;                            
                    }
                }

                if (!entered)
                {
                    DataGridViewRow row = new DataGridViewRow();                    
                    int rowIndex = dgv_Ranked.Rows.Add(row);
                    dgv_Ranked.Rows[rowIndex].Cells[judge.Level.ToString()].Value = judge; 
                    if(!judge.Active)
                    {
                        dgv_Ranked[judge.Level.ToString(), rowIndex].Style.BackColor = Color.OrangeRed;
                    }
                }
            }

            dgv_Ranked.CellClick += (sender, e) => { CellClicked(sender, e);};

            dgv_Ranked.CellMouseDown += (sender, e) => { GetDraggedValue(sender, e); };
            dgv_Ranked.CellMouseUp += (sender, e) => { MoveDraggedValue(sender, e); };

        }
        #endregion

        #region MoveDraggedValue
        private void MoveDraggedValue(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(!e.Button.HasFlag(MouseButtons.Left)) return;

            if(_currentlySelectedCell == null) return;
            
            if(e.ColumnIndex == -1 || e.RowIndex == -1) return;

            if(_currentlySelectedCell.ColumnIndex == e.ColumnIndex)
            {
                _currentlySelectedCell = null;
                Cursor = Cursors.Arrow;
                return;
            }


            bool entered = false;
            foreach (DataGridViewRow row in dgv_Ranked.Rows)
            {
                if(row.Cells[e.ColumnIndex].Value == null)
                {
                    row.Cells[e.ColumnIndex].Value = _currentlySelectedCell.Value;
                    entered = true;
                    break;                    
                }
            }

            if(!entered)
            {
                DataGridViewRow row = new DataGridViewRow();
                int rowIndex = dgv_Ranked.Rows.Add(row);
                dgv_Ranked.Rows[rowIndex].Cells[e.ColumnIndex].Value = _currentlySelectedCell.Value;
            }


            Judge judge = (Judge)_currentlySelectedCell.Value;
            judge.Level = (JudgeLevel) Enum.Parse(typeof (JudgeLevel), dgv_Ranked.Columns[e.ColumnIndex].Name);
            SqlHelper helper = new SqlHelper(_tournament.Database);
            helper.UpdateJudge(judge);
            _context.Judges = _context.Judges.Where(j => j.JudgeId != judge.JudgeId).ToList();
            _context.Judges.Add(judge);            

            dgv_Ranked.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Value = null;

            DataGridViewRow oldRow = dgv_Ranked.Rows[_currentlySelectedCell.RowIndex];
            bool hasValue = false;
            foreach (DataGridViewCell cell in oldRow.Cells)
            {
                if(cell.Value != null)
                {
                    hasValue = true;
                    break;
                }
            }

            if(_currentlySelectedCell.RowIndex == 0 && dgv_Ranked.Rows[_currentlySelectedCell.RowIndex].Cells[_currentlySelectedCell.ColumnIndex].Value == null)
            {
                for (int i = 0; i < dgv_Ranked.RowCount - 1; i++)
                {
                    dgv_Ranked.Rows[i].Cells[_currentlySelectedCell.ColumnIndex].Value = dgv_Ranked.Rows[i + 1].Cells[_currentlySelectedCell.ColumnIndex].Value;
                }
                dgv_Ranked.Rows[dgv_Ranked.RowCount - 1].Cells[_currentlySelectedCell.ColumnIndex].Value = null;
            }

            if(!hasValue)
            {
                dgv_Ranked.Rows.Remove(oldRow);
            }
            
            _currentlySelectedCell = null;
            Cursor = Cursors.Arrow;
        }
        #endregion

        #region PopulateInstitutions
        private void PopulateInstitutions()
        {
            cmb_Institution.Items.Clear();
            foreach (Institution i in _context.Institutions.OrderBy(i => i.Name))
            {
                cmb_Institution.Items.Add(i);
            }

            if (cmb_Institution.Items.Count > 0)
            {
                cmb_Institution.SelectedIndex = 0;
            }
            Refresh();
        }
        #endregion

        #region NameChanged
        private void NameChanged()
        {
            if(!string.Equals(_initialName, txt_Name.Text))
            {
                _isChanged = true;
            }            
        }
        #endregion

        #region ReloadJudges
        private void ReloadJudges(Judge judge, ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    Guid judgeId = judge.JudgeId;
                    TreeNode node = new TreeNode()
                    {
                        Name = judge.InstitutionId.ToString(),
                        Text = judge.Name
                    };
                    node.ContextMenuStrip = new ContextMenuStrip();
                    ToolStripItem item = new ToolStripButton("Delete");
                    item.Click += (sender, e) => { DeleteJudge(judgeId); };
                    node.ContextMenuStrip.Items.Add(item);
                    int index = _context.Judges.OrderBy(s => s.Name).ToList().IndexOf(judge);
                    tree_Judges.Nodes.Insert(index, node);
                    break;
                case ActionType.Update:
                    tree_Judges.Nodes[judge.JudgeId.ToString()].Text = judge.Name;
                    if(judge.Active)
                    {
                        tree_Judges.Nodes[judge.JudgeId.ToString()].BackColor = Color.White;
                    }
                    else
                    {
                        tree_Judges.Nodes[judge.JudgeId.ToString()].BackColor = Color.OrangeRed;
                    }
                    break;
                case ActionType.Delete:
                    tree_Judges.Nodes.RemoveByKey(judge.JudgeId.ToString());
                    break;
            }

            lbl_Count.Text = string.Format("{0} judges active, {1} judges active", _context.Judges.Count(j => j.Active), _context.Judges.Count(j => !j.Active));

            Refresh();
        }
        #endregion

        #region SaveNewJudge
        private void SaveNewJudge()
        {
            string judgeName = txt_Name.Text;

            if(string.IsNullOrWhiteSpace(judgeName))
            {
                MessageBox.Show("Name cannot be empty!");
                return;
            }

            Guid? institutionId = default(Guid?);
            bool hasInstitution = chk_HasInstitution.Checked;
            if(hasInstitution)
            {
                Institution institution = (Institution) cmb_Institution.SelectedItem;
                institutionId = institution.InstitutionId;
            }

            Judge j = new Judge()
            {
                InstitutionId = institutionId,
                Name = judgeName
            };

            SqlHelper helper = new SqlHelper(_tournament.Database);

            NonQueryResult result = helper.InsertJudge(j);

            _context.Judges.Add(j);

            ReloadJudges(j, ActionType.Create);

            CreateNewJudge();
        }
        #endregion

        #region TabpageSelected
        private void TabpageSelected(object sender, TabControlEventArgs e)
        {
            if(!e.Action.HasFlag(TabControlAction.Selected)) return;

            if(e.TabPageIndex == tabpage_JudgeEntry.TabIndex)
            {
                LoadJudges();
                PopulateInstitutions();
                btn_CreateNewJudge.Click += (sender2, e2) => { CreateNewJudge(); };
                chk_HasInstitution.CheckedChanged += (sender3, e3) => { HasInstitutionChanged(); };
                cmb_Institution.Enabled = chk_HasInstitution.Checked;
                CreateNewJudge();
            }
            else
            {
                LoadRankings();
            }
        }
        #endregion

        #region UpdateJudge
        private void UpdateJudge(Guid judgeId)
        {
            if(!_isChanged) return;

            string judgeName = txt_Name.Text;
            if(string.IsNullOrWhiteSpace(judgeName))
            {
                MessageBox.Show("Cannot enter empty judge name!");
                return;
            }

            Guid? institutionId = default(Guid?);

            bool hasInstitution = chk_HasInstitution.Checked;
            if(hasInstitution)
            {
                Institution i = (Institution) cmb_Institution.SelectedItem;
                institutionId = i.InstitutionId;
            }

            bool active = chk_Active.Checked;

            Judge judge = _context.Judges.First(j => j.JudgeId == judgeId);
            judge.Name = judgeName;
            judge.InstitutionId = institutionId;
            judge.Active = active;

            SqlHelper helper = new SqlHelper(_tournament.Database);

            NonQueryResult result = helper.UpdateJudge(judge);

            _context.Judges = _context.Judges.Where(j => j.JudgeId != judgeId).ToList();
            _context.Judges.Add(judge);

            ReloadJudges(judge, ActionType.Update);

            _isChanged = false;
        }
        #endregion
    }
}
