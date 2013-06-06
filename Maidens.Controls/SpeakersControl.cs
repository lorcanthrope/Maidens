using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Maidens.Helpers;
using Maidens.Models;

namespace Maidens.Controls
{
    public partial class SpeakersControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;
        private bool _isChanged;
        private bool _initialActive;
        private string _initialName;
        private string _initialInstitution;
        private bool _initialSpecialNeeds;
        #endregion

        #region Construction
        public SpeakersControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _context = context;
            _tournament = tournament;

            LoadSpeakers();
            PopulateInstitutions();

            btn_CreateNewSpeaker.Click += (sender, e) => { CreateNewSpeaker(); };   
            CreateNewSpeaker();
        }
        #endregion

        #region CheckboxChanged
        private void CheckboxChanged()
        {
            if(!chk_SpecialNeeds.Checked.Equals(_initialSpecialNeeds))
            {
                _isChanged = true;
            }

            if(!chk_Active.Checked.Equals(_initialActive))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region ComboBoxChanged
        private void ComboBoxChanged()
        {
            if(!cmb_Institution.SelectedText.Equals(_initialInstitution))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region CreateNewSpeaker
        private void CreateNewSpeaker()
        {
            txt_Name.Text = "New Speaker";
            ActiveControl = txt_Name;
            tree_Speakers.SelectedNode = null;
            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { SaveNewSpeaker(); };
            _initialName = string.Empty;
            _initialInstitution = string.Empty;
            _initialSpecialNeeds = false;
            _isChanged = false;
        }
        #endregion

        #region DeleteSpeaker
        private void DeleteSpeaker(Guid speakerId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);
            NonQueryResult result = helper.DeleteSpeaker(speakerId);

            Speaker toBeDeleted = _context.Speakers.First(s => s.SpeakerId == speakerId);

            _context.Speakers = _context.Speakers.Where(s => s.SpeakerId != speakerId).ToList();
            
            ReloadSpeakers(toBeDeleted, ActionType.Delete);            
        }
        #endregion

        #region LoadSpeaker
        private void LoadSpeaker(Guid speakerId)
        {
            Speaker speaker = _context.Speakers.First(s => s.SpeakerId == speakerId);

            txt_Name.Text = speaker.Name;
            _initialName = speaker.Name;

            List<Institution> institutions = cmb_Institution.Items.Cast<Institution>().ToList();
            Institution institution = institutions.First(i => i.InstitutionId == speaker.InstitutionId); 
            string institutionName = institution.Name;
            _initialInstitution = institutionName;
            cmb_Institution.SelectedItem = institution;

            chk_SpecialNeeds.Checked = speaker.SpecialNeeds;
            _initialSpecialNeeds = speaker.SpecialNeeds;
            chk_Active.Checked = speaker.Active;
            _initialActive = speaker.Active;
            
            txt_Name.TextChanged += (sender, e) => { TextChanged(); };
            cmb_Institution.SelectedValueChanged += (sender, e) => { ComboBoxChanged(); };
            chk_SpecialNeeds.CheckedChanged += (sender, e) => { CheckboxChanged(); };
            chk_Active.CheckedChanged += (sender, e) => { CheckboxChanged(); };

            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { UpdateSpeaker(speakerId); };
            _isChanged = false;
            Refresh();
        }
        #endregion

        #region LoadSpeakers
        private void LoadSpeakers()
        {
            tree_Speakers.Nodes.Clear();

            if (_context.Speakers.Any())
            {
                foreach (Speaker s in _context.Speakers.OrderBy(s => s.Name))
                {
                    Guid speakerId = s.SpeakerId;
                    TreeNode node = new TreeNode()
                                        {
                                            Name = s.SpeakerId.ToString(),
                                            Text = s.Name
                                        };
                    node.ContextMenuStrip = new ContextMenuStrip();
                    ToolStripItem item = new ToolStripButton("Delete");
                    item.Click += (sender, e) => { DeleteSpeaker(speakerId); };
                    node.ContextMenuStrip.Items.Add(item);

                    if (!s.Active)
                    {
                        node.BackColor = Color.OrangeRed;
                    }

                    tree_Speakers.Nodes.Add(node);
                }
            }

            tree_Speakers.AfterSelect += (sender, e) => { LoadSpeaker(Guid.Parse(e.Node.Name)); };

            lbl_Count.Text = string.Format("{0} speakers active, {1} speakers inactive", _context.Speakers.Count(s => s.Active), _context.Speakers.Count(s => !s.Active));

            Refresh();
        }
        #endregion

        #region PopulateInstitutions
        private void PopulateInstitutions()
        {            
            cmb_Institution.Items.Clear();
            foreach(Institution i in _context.Institutions.OrderBy(i => i.Name))            
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

        #region ReloadSpeakers
        private void ReloadSpeakers(Speaker speaker, ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                     Guid speakerId = speaker.SpeakerId;
                     TreeNode node = new TreeNode()
                                    {
                                        Name = speaker.SpeakerId.ToString(),
                                        Text = speaker.Name
                                    };
                     node.ContextMenuStrip = new ContextMenuStrip();
                     ToolStripItem item = new ToolStripButton("Delete");
                     item.Click += (sender, e) => { DeleteSpeaker(speakerId); };
                     node.ContextMenuStrip.Items.Add(item);
                     int index = _context.Speakers.OrderBy(s => s.Name).ToList().IndexOf(speaker);
                     tree_Speakers.Nodes.Insert(index, node);     
                     break;
                case ActionType.Update:
                    tree_Speakers.Nodes[speaker.SpeakerId.ToString()].Text = speaker.Name;

                    if(speaker.Active)
                    {
                        tree_Speakers.Nodes[speaker.SpeakerId.ToString()].BackColor = Color.White;
                    }
                    else
                    {
                        tree_Speakers.Nodes[speaker.SpeakerId.ToString()].BackColor = Color.OrangeRed;
                    }

                    break;
                case ActionType.Delete:
                    tree_Speakers.Nodes.RemoveByKey(speaker.SpeakerId.ToString());
                    break;
            }

            lbl_Count.Text = string.Format("{0} speakers active, {1} speakers inactive", _context.Speakers.Count(s => s.Active), _context.Speakers.Count(s => !s.Active));

            Refresh();
        }
        #endregion
        
        #region SaveNewSpeaker
        private void SaveNewSpeaker()
        {
            string speakerName = txt_Name.Text;

            if(string.IsNullOrWhiteSpace(speakerName))
            {
                MessageBox.Show("Name cannot be empty!");
                return;
            }

            SqlHelper helper = new SqlHelper(_tournament.Database);

            Institution institution = (Institution) cmb_Institution.SelectedItem;

            bool specialNeeds = chk_SpecialNeeds.Checked;

            bool active = chk_Active.Checked;

            Speaker s = new Speaker()
                            {
                                Active = active,
                                InstitutionId = institution.InstitutionId,
                                Name = speakerName,
                                SpecialNeeds = specialNeeds
                            };

            NonQueryResult result = helper.InsertSpeaker(s);

            _context.Speakers.Add(s);

            ReloadSpeakers(s, ActionType.Create);
            
            CreateNewSpeaker();
        }
        #endregion

        #region TextChanged
        private void TextChanged()
        {
            if(!txt_Name.Text.Equals(_initialName))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region UpdateSpeaker
        private void UpdateSpeaker(Guid speakerId)
        {
            if(!_isChanged) return;

            string speakerName = txt_Name.Text;
            if(string.IsNullOrWhiteSpace(speakerName))
            {
                MessageBox.Show("Cannot enter empty speaker name!");
                return;
            }

            SqlHelper helper = new SqlHelper(_tournament.Database);

            Institution institution = (Institution) cmb_Institution.SelectedItem;
            bool specialNeeds = chk_SpecialNeeds.Checked;
            bool active = chk_Active.Checked;

            Speaker temp = new Speaker();
            temp.SpeakerId = speakerId;
            temp.Active = active;
            temp.Name = speakerName;
            temp.InstitutionId = institution.InstitutionId;
            temp.SpecialNeeds = specialNeeds;

            NonQueryResult result = helper.UpdateSpeaker(temp);

            _context.Speakers = _context.Speakers.Where(s => s.SpeakerId != speakerId).ToList();
            _context.Speakers.Add(temp);
            
            ReloadSpeakers(temp, ActionType.Update);
            
            _isChanged = false;
        }
        #endregion
    }
}
