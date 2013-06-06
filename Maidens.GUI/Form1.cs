using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maidens.Controls;
using Maidens.Helpers;
using Maidens.Models;

namespace Maidens.GUI
{
    public partial class Form1 : Form
    {
        #region Fields
        private static readonly AppSettingsReader Reader = new AppSettingsReader();
        private DataContext _context;
        private Tournament _tournament; 
        #endregion

        #region Construction
        public Form1()
        {
            InitializeComponent();

            
        }
        #endregion

        #region Display

        #region LoadLeftPanel
        private void LoadLeftPanel()
        {
            panel_Left.Controls.Clear();
            TreeView treeView = new TreeView()
                                    {
                                        Font = new Font("Tahoma", 20),
                                        Size = panel_Left.Size
                                    };

            Array values = Enum.GetValues(typeof (LeftPanelEntries));
            foreach (LeftPanelEntries value in values)
            {
                treeView.Nodes.Add(value.ToString(), value.ToString());
            }

            treeView.AfterSelect += delegate(object sender, TreeViewEventArgs args)
            {
                SqlHelper helper = new SqlHelper(_tournament.Database);
                _context = helper.GetDataContext().Result;

                switch ((LeftPanelEntries)Enum.Parse(typeof(LeftPanelEntries), args.Node.Name))
                {
                    case LeftPanelEntries.Institutions:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new InstitutionsControl(panel_Main.Size, _tournament, _context));
                        break;
                    case LeftPanelEntries.Speakers:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new SpeakersControl(panel_Main.Size, _tournament, _context));
                        break;                        
                    case LeftPanelEntries.Judges:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new JudgesControl(panel_Main.Size, _tournament, _context));
                        break;
                    case LeftPanelEntries.Venues:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new VenuesControl(panel_Main.Size, _tournament, _context));
                        break;
                    case LeftPanelEntries.Rounds:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new RoundsControl(panel_Main.Size, _tournament, _context));
                        break;
                    case LeftPanelEntries.Results:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new ResultsControl(panel_Main.Size, _tournament, _context));
                        break;
                    case LeftPanelEntries.Standings:
                        panel_Main.Controls.Clear();
                        panel_Main.Controls.Add(new StandingsControl(panel_Main.Size, _tournament, _context));
                        break;
                }
                Refresh();
            };

            panel_Left.Controls.Add(treeView);
        }
        #endregion

        #endregion

        #region File

        #region newTournamentToolStripMenuItem_Click
        private void newTournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to create a new tournament?", "Create New Tournament", MessageBoxButtons.OKCancel);
            if(result == DialogResult.OK)
            {
                panel_Main.Controls.Clear();

                panel_Main.Controls.Add(new Label()
                {
                    Location = new Point(100, 20),
                    Name = "lbl_TournamentName",
                    Text = "Tournament Name"
                });

                TextBox txt_TournamentName = new TextBox()
                                                 {
                                                    Location = new Point(200,15),
                                                    Name = "txt_TournamentName",
                                                    Size = new Size(150,10) 
                                                 };

                panel_Main.Controls.Add(txt_TournamentName);

                Button createNewButton = new Button()
                {
                    Location = new Point(370, 15),
                    Name = "btn_CreateNewTournament",
                    Text = "Create Tournament"
                };

                createNewButton.Click += delegate(object o, EventArgs args)
                {
                        string tournamentName = txt_TournamentName.Text;
                        if(string.IsNullOrWhiteSpace(tournamentName))
                        {
                            MessageBox.Show("Tournament name cannot be empty!");
                            return;
                        }

                        SaveFileDialog dialog = new SaveFileDialog();
                        dialog.Filter = "Tournament files (*.trn)|*.trn|All files (*.*)|*.*";
                        dialog.Title = "Select the location of your new tournament";
                        dialog.FileName = string.Format("{0}.trn", tournamentName);
                        DialogResult saveResult = dialog.ShowDialog();
                        if (saveResult == DialogResult.OK)
                        {
                            string filename = dialog.FileName;
                            if (string.IsNullOrWhiteSpace(filename))
                            {
                                MessageBox.Show("File name cannot be empty!");
                                return;
                            }

                            if (File.Exists(filename))
                            {
                                MessageBox.Show("File already exists!");
                                return;
                            }

                            string directory = Path.GetDirectoryName(filename);
                            string tournamentDirectory = Path.Combine(directory, tournamentName);
                            if (Directory.Exists(tournamentDirectory))
                            {
                                MessageBox.Show("Directory already exists!");
                            }
                            
                            Directory.CreateDirectory(tournamentDirectory);

                            string databaseDirectory = Path.Combine(tournamentDirectory, "database");

                            if(!Directory.Exists(databaseDirectory))
                            {
                                Directory.CreateDirectory(databaseDirectory);
                            }

                            string databaseLocation = Path.Combine(databaseDirectory, "database.db");

                            _tournament = new Tournament()
                                                        {
                                                            Database = databaseLocation,
                                                            Location = filename,
                                                            Name = tournamentName,
                                                            RoundNumber = 1
                                                        };

                            File.WriteAllText(filename, _tournament.ToString()); 
                            SqlHelper helper = new SqlHelper(databaseLocation, true);   

                            PopulateTournament(_tournament);

                            panel_Main.Controls.Clear();
                            LoadLeftPanel();
                            Text = string.Format("Maidens - {0}", tournamentName);
                        }
                };

                panel_Main.Controls.Add(createNewButton);                
                Refresh();
            }
        }
        #endregion

        #region openTournamentToolStripMenuItem_Click
        private void openTournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tournament files (*.trn)|*.trn|All files (*.*)|*.*";
            dialog.Title = "Select the location of the tournament you wish to load";
            DialogResult openResult = dialog.ShowDialog();
            if(openResult == DialogResult.OK)
            {
                string filename = dialog.FileName;
                if(string.IsNullOrWhiteSpace(filename))
                {
                    MessageBox.Show("File name cannot be empty!");
                    return;
                }

                if(!File.Exists(filename))
                {
                    MessageBox.Show("File does not exist!");
                    return;
                }

                string contents = File.ReadAllText(filename);
                Tournament tournament = Tournament.Parse(contents);
                if(tournament != null)
                {
                    _tournament = tournament;
                    Text = string.Format("Maidens - {0}", tournament.Name);
                    panel_Main.Controls.Clear();
                    LoadLeftPanel();
                    Refresh();
                }
            }
        }
        #endregion

        #region exitToolStripMenuItem_Click
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exiting...", MessageBoxButtons.YesNo);

            if(result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        #endregion

        #region PopulateTournament
        private void PopulateTournament(Tournament tournament)
        {
            string libDirectory = Path.Combine(Path.GetDirectoryName(tournament.Location), tournament.Name, "lib");

            if(!Directory.Exists(libDirectory))
            {
                Directory.CreateDirectory(libDirectory);
            }

            string sourceDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib");

            string[] files = Directory.GetFiles(sourceDirectory);

            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(libDirectory, Path.GetFileName(file)));
            }           
        }
        #endregion

        #endregion

        #region About

        #region aboutToolStripMenuItem1_Click
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string message = (string) Reader.GetValue("About/Message", typeof (string));
            MessageBox.Show(message);
        }
        #endregion

        

        
        
        #endregion
    }
}
