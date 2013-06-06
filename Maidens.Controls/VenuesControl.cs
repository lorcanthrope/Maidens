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
    public partial class VenuesControl : UserControl
    {
        #region Fields
        private DataContext _context;
        private readonly Tournament _tournament;

        private bool _initialActive;
        private string _initialName;
        private bool _initialSpecialNeeds;
        private bool _isChanged;

        #endregion

        #region Construction
        public VenuesControl(Size size, Tournament tournament, DataContext context)
        {
            InitializeComponent();

            Size = size;

            _context = context;
            _tournament = tournament;

            LoadVenues();

            btn_CreateNewVenue.Click += (sender, e) => { CreateNewVenue(); };
            CreateNewVenue();
        }
        #endregion

        #region ActiveChanged
        private void ActiveChanged()
        {
            if(!_initialActive.Equals(chk_Active.Checked))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region CreateNewVenue
        private void CreateNewVenue()
        {
            txt_Name.Text = "New Venue";
            ActiveControl = txt_Name;
            tree_Venues.SelectedNode = null;
            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { SaveNewVenue(); };
            _initialName = string.Empty;
            _initialSpecialNeeds = false;
            _isChanged = false;
        }
        #endregion

        #region DeleteVenue
        private void DeleteVenue(Guid venueId)
        {
            SqlHelper helper = new SqlHelper(_tournament.Database);
            NonQueryResult result = helper.DeleteVenue(venueId);

            Venue toBeDeleted = _context.Venues.First(v => v.VenueId == venueId);

            _context.Venues = _context.Venues.Where(v => v.VenueId != venueId).ToList();

            ReloadVenues(toBeDeleted, ActionType.Delete);
        }
        #endregion

        #region LoadVenue
        private void LoadVenue(Guid venueId)
        {
            Venue venue = _context.Venues.First(v => v.VenueId == venueId);

            txt_Name.Text = venue.Name;
            _initialName = venue.Name;

            chk_SpecialNeeds.Checked = venue.SpecialNeedsVenue;
            _initialSpecialNeeds = venue.SpecialNeedsVenue;

            chk_Active.Checked = venue.Active;
            _initialActive = venue.Active;

            txt_Name.TextChanged += (sender, e) => { NameChanged(); };
            chk_SpecialNeeds.CheckedChanged += (sender, e) => { SpecialNeedsChanged(); };
            chk_Active.CheckedChanged += (sender, e) => { ActiveChanged(); };

            EventsHelper.RemoveClickEvents(btn_Save);
            btn_Save.Click += (sender, e) => { UpdateVenue(venueId); };
            _isChanged = false;
            Refresh();
        }
        #endregion

        #region LoadVenues
        private void LoadVenues()
        {
            tree_Venues.Nodes.Clear();

            foreach(Venue v in _context.Venues.OrderBy(v => v.Name))
            {
                Guid venueId = v.VenueId;
                TreeNode node = new TreeNode()
                {
                    Name = v.VenueId.ToString(),
                    Text = v.Name
                };
                node.ContextMenuStrip = new ContextMenuStrip();
                ToolStripItem item = new ToolStripButton("Delete");
                item.Click += (sender, e) => { DeleteVenue(venueId); };
                node.ContextMenuStrip.Items.Add(item);
                if(!v.Active)
                {
                    node.BackColor = Color.OrangeRed;
                }

                tree_Venues.Nodes.Add(node);
            }

            tree_Venues.AfterSelect += (sender, e) => { LoadVenue(Guid.Parse(e.Node.Name)); };

            lbl_Count.Text = string.Format("{0} venues active, {1} venues inactive", _context.Venues.Count(v => v.Active), _context.Venues.Count(v => !v.Active));

            Refresh();
        }
        #endregion

        #region NameChanged
        private void NameChanged()
        {
            if(!string.Equals(txt_Name.Text, _initialName))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region ReloadVenues
        private void ReloadVenues(Venue v, ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    Guid venueId = v.VenueId;
                    TreeNode node = new TreeNode()
                    {
                        Name = venueId.ToString(),
                        Text = v.Name
                    };
                    node.ContextMenuStrip = new ContextMenuStrip();
                    ToolStripItem item = new ToolStripButton("Delete");
                    item.Click += (sender, e) => { DeleteVenue(venueId); };
                    node.ContextMenuStrip.Items.Add(item);
                    int index = _context.Venues.OrderBy(s => s.Name).ToList().IndexOf(v);
                    tree_Venues.Nodes.Insert(index, node);
                    break;
                case ActionType.Update:
                    tree_Venues.Nodes[v.VenueId.ToString()].Text = v.Name;
                    if(v.Active)
                    {
                        tree_Venues.Nodes[v.VenueId.ToString()].BackColor = Color.White;
                    }
                    else
                    {
                        tree_Venues.Nodes[v.VenueId.ToString()].BackColor = Color.OrangeRed;
                    }
                    break;
                case ActionType.Delete:
                    tree_Venues.Nodes.RemoveByKey(v.VenueId.ToString());
                    break;
            }

            lbl_Count.Text = string.Format("{0} venues active, {1} venues inactive", _context.Venues.Count(ven => ven.Active), _context.Venues.Count(ven => !ven.Active));

            Refresh();

        }
        #endregion

        #region SaveNewVenue
        private void SaveNewVenue()
        {
            string venueName = txt_Name.Text;

            if(string.IsNullOrWhiteSpace(venueName))
            {
                MessageBox.Show("Venue name cannot be empty!");
                return;
            }

            if(_context.Venues.Any(ven => ven.Name == venueName))
            {
                MessageBox.Show("Venue already exists!");
                return;
            }

            Venue v = new Venue()
                          {
                              SpecialNeedsVenue = chk_SpecialNeeds.Checked,
                              Name = venueName
                          };

            SqlHelper helper = new SqlHelper(_tournament.Database);

            NonQueryResult result = helper.InsertVenue(v);

            _context.Venues.Add(v);

            ReloadVenues(v, ActionType.Create);

            CreateNewVenue();
        }
        #endregion

        #region SpecialNeedsChanged
        private void SpecialNeedsChanged()
        {
            if(!chk_SpecialNeeds.Checked.Equals(_initialSpecialNeeds))
            {
                _isChanged = true;
            }
        }
        #endregion

        #region UpdateVenue
        private void UpdateVenue(Guid venueId)
        {
            if(!_isChanged) return;

            string venueName = txt_Name.Text;
            if(string.IsNullOrWhiteSpace(venueName))
            {
                MessageBox.Show("Cannot enter empty venue name!");
                return;
            }

            SqlHelper helper = new SqlHelper(_tournament.Database);

            Venue venue = _context.Venues.First(v => v.VenueId == venueId);
            venue.Name = venueName;
            venue.SpecialNeedsVenue = chk_SpecialNeeds.Checked;
            venue.Active = chk_Active.Checked;

            NonQueryResult result = helper.UpdateVenue(venue);

            _context.Venues = _context.Venues.Where(v => v.VenueId != venueId).ToList();
            _context.Venues.Add(venue);

            ReloadVenues(venue, ActionType.Update);

            _isChanged = false;
        }
        #endregion
    }
}
