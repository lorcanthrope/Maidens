namespace Maidens.Controls
{
    partial class ResultsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmb_Rounds = new System.Windows.Forms.ComboBox();
            this.btn_LoadRound = new System.Windows.Forms.Button();
            this.tree_Rooms = new System.Windows.Forms.TreeView();
            this.dgv_Debate = new System.Windows.Forms.DataGridView();
            this.btn_Save = new System.Windows.Forms.Button();
            this.dgv_Judges = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Debate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Judges)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_Rounds
            // 
            this.cmb_Rounds.FormattingEnabled = true;
            this.cmb_Rounds.Location = new System.Drawing.Point(4, 16);
            this.cmb_Rounds.Name = "cmb_Rounds";
            this.cmb_Rounds.Size = new System.Drawing.Size(121, 21);
            this.cmb_Rounds.TabIndex = 0;
            // 
            // btn_LoadRound
            // 
            this.btn_LoadRound.Location = new System.Drawing.Point(131, 14);
            this.btn_LoadRound.Name = "btn_LoadRound";
            this.btn_LoadRound.Size = new System.Drawing.Size(75, 23);
            this.btn_LoadRound.TabIndex = 1;
            this.btn_LoadRound.Text = "Load";
            this.btn_LoadRound.UseVisualStyleBackColor = true;
            // 
            // tree_Rooms
            // 
            this.tree_Rooms.Location = new System.Drawing.Point(4, 43);
            this.tree_Rooms.Name = "tree_Rooms";
            this.tree_Rooms.Size = new System.Drawing.Size(211, 353);
            this.tree_Rooms.TabIndex = 2;
            // 
            // dgv_Debate
            // 
            this.dgv_Debate.AllowUserToAddRows = false;
            this.dgv_Debate.AllowUserToDeleteRows = false;
            this.dgv_Debate.AllowUserToResizeColumns = false;
            this.dgv_Debate.AllowUserToResizeRows = false;
            this.dgv_Debate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Debate.Location = new System.Drawing.Point(221, 43);
            this.dgv_Debate.MultiSelect = false;
            this.dgv_Debate.Name = "dgv_Debate";
            this.dgv_Debate.RowHeadersVisible = false;
            this.dgv_Debate.Size = new System.Drawing.Size(451, 181);
            this.dgv_Debate.TabIndex = 3;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(597, 16);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // dgv_Judges
            // 
            this.dgv_Judges.AllowUserToAddRows = false;
            this.dgv_Judges.AllowUserToDeleteRows = false;
            this.dgv_Judges.AllowUserToResizeColumns = false;
            this.dgv_Judges.AllowUserToResizeRows = false;
            this.dgv_Judges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Judges.Location = new System.Drawing.Point(222, 231);
            this.dgv_Judges.Name = "dgv_Judges";
            this.dgv_Judges.ReadOnly = true;
            this.dgv_Judges.RowHeadersVisible = false;
            this.dgv_Judges.Size = new System.Drawing.Size(450, 150);
            this.dgv_Judges.TabIndex = 5;
            // 
            // ResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv_Judges);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.dgv_Debate);
            this.Controls.Add(this.tree_Rooms);
            this.Controls.Add(this.btn_LoadRound);
            this.Controls.Add(this.cmb_Rounds);
            this.Name = "ResultsControl";
            this.Size = new System.Drawing.Size(676, 399);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Debate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Judges)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_Rounds;
        private System.Windows.Forms.Button btn_LoadRound;
        private System.Windows.Forms.TreeView tree_Rooms;
        private System.Windows.Forms.DataGridView dgv_Debate;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.DataGridView dgv_Judges;
    }
}
