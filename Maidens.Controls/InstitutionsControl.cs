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
    public partial class InstitutionsControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;
        private bool _isChanged;
        private string _initialValue;
        #endregion

        #region Construction
        public InstitutionsControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _context = context;
            _tournament = tournament;

            LoadInstitutions();

            btn_CreateNewInstitution.Click += (sender, e) => { CreateNewInstitution(); };
            CreateNewInstitution();
        }
        #endregion

        #region CreateNewInstitution
        private void CreateNewInstitution()
        {
            txt_Name.Text = "New Institution";
            ActiveControl = txt_Name;
            tree_Institutions.SelectedNode = null;    
            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { SaveNewInstitution(); };
            _initialValue = string.Empty;
            _isChanged = false;
        }
        #endregion

        #region DeleteInstitution
        private void DeleteInstitution(Guid institutionId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);
            
            NonQueryResult result = helper.DeleteInstitution(institutionId);

            Institution toBeDeleted = _context.Institutions.First(i => i.InstitutionId == institutionId);
            
            _context.Institutions = _context.Institutions.Where(i => i.InstitutionId != institutionId).ToList();

            ReloadInstitutions(toBeDeleted, ActionType.Delete);                     
        }
        #endregion

        #region LoadInstitution
        private void LoadInstitution(Guid institutionId)
        {
            Institution institution = _context.Institutions.First(i => i.InstitutionId == institutionId);
             
            txt_Name.Text = institution.Name;
            _initialValue = institution.Name;

            txt_Name.TextChanged += (sender, e) => { TextChanged(); };

            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { UpdateInstitution(institution.InstitutionId); };
            _isChanged = false;
            Refresh();
        }
        #endregion
        
        #region LoadInstitutions
        private void LoadInstitutions()
        {            
            tree_Institutions.Nodes.Clear();
            foreach (Institution i in _context.Institutions.OrderBy(i => i.Name))
            {
                Guid institutionId = i.InstitutionId;
                TreeNode node = new TreeNode()
                                    {
                                        Name = i.InstitutionId.ToString(),
                                        Text = i.Name
                                    };
                node.ContextMenuStrip = new ContextMenuStrip();
                ToolStripItem item = new ToolStripButton("Delete");
                item.Click += (sender, e) => { DeleteInstitution(institutionId); };
                node.ContextMenuStrip.Items.Add(item);
                tree_Institutions.Nodes.Add(node);                    
            }
            

            tree_Institutions.AfterSelect += (sender, e) => { LoadInstitution(Guid.Parse(e.Node.Name)); };

            lbl_Count.Text = string.Format("{0} institutions", _context.Institutions.Count);

            Refresh();
        }
        #endregion

        #region ReloadInstitutions
        private void ReloadInstitutions(Institution institution, ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    Guid institutionId = institution.InstitutionId;
                    TreeNode node = new TreeNode()
                    {
                        Name = institution.InstitutionId.ToString(),
                        Text = institution.Name
                    };
                    node.ContextMenuStrip = new ContextMenuStrip();
                    ToolStripItem item = new ToolStripButton("Delete");
                    item.Click += (sender, e) => { DeleteInstitution(institutionId); };
                    node.ContextMenuStrip.Items.Add(item);
                    int index = _context.Institutions.OrderBy(s => s.Name).ToList().IndexOf(institution);
                    tree_Institutions.Nodes.Insert(index, node);
                    break;
                case ActionType.Update:
                    tree_Institutions.Nodes[institution.InstitutionId.ToString()].Text = institution.Name;
                    break;
                case ActionType.Delete:
                    tree_Institutions.Nodes.RemoveByKey(institution.InstitutionId.ToString());
                    break;
            }

            lbl_Count.Text = string.Format("{0} institutions", _context.Institutions.Count);

            Refresh();
        }
        #endregion
        
        #region SaveNewInstitution
        private void SaveNewInstitution()
        {
            string institutionName = txt_Name.Text;

            if(string.IsNullOrWhiteSpace(institutionName))
            {
                MessageBox.Show("Name cannot be empty!");
                return;
            }
            
            if(_context.Institutions.Any(x => x.Name.Equals(institutionName)))
            {
                MessageBox.Show("This institution has already been entered!");
                return;
            }

            Institution i = new Institution()
                                {
                                    Name = institutionName
                                };

            SqlHelper helper = new SqlHelper(_tournament.Database);

            NonQueryResult result = helper.InsertInstitution(i);

            _context.Institutions.Add(i);

            ReloadInstitutions(i, ActionType.Create);                      

            CreateNewInstitution();            
        }
        #endregion

        #region TextChanged
        private void TextChanged()
        {
            if(!txt_Name.Text.Equals(_initialValue))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region UpdateInstitution
        private void UpdateInstitution(Guid institutionId)
        {
            if (!_isChanged) return;

            string institutionName = txt_Name.Text;
            if (string.IsNullOrWhiteSpace(institutionName))
            {
                MessageBox.Show("Cannot enter empty institution name!");
                return;
            }

            if(_context.Institutions.Any(x => x.Name.Equals(institutionName)))
            {
                MessageBox.Show("This institution has already been entered!");
                return;
            }

            Institution temp = new Institution();
            temp.InstitutionId = institutionId;
            temp.Name = institutionName;

            SqlHelper helper = new SqlHelper(_tournament.Database);

            NonQueryResult result = helper.UpdateInstitution(temp);

            _context.Institutions = _context.Institutions.Where(i => i.InstitutionId != institutionId).ToList();
            _context.Institutions.Add(temp);

            ReloadInstitutions(temp, ActionType.Update);              
         
            _isChanged = false;
        }
        #endregion
    }
}
