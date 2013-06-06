namespace Maidens.Controls
{
    partial class JudgesControl
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
            this.tabpages = new System.Windows.Forms.TabControl();
            this.tabpage_JudgeEntry = new System.Windows.Forms.TabPage();
            this.btn_Save = new System.Windows.Forms.Button();
            this.cmb_Institution = new System.Windows.Forms.ComboBox();
            this.chk_HasInstitution = new System.Windows.Forms.CheckBox();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.btn_CreateNewJudge = new System.Windows.Forms.Button();
            this.lbl_Count = new System.Windows.Forms.Label();
            this.tree_Judges = new System.Windows.Forms.TreeView();
            this.tabpage_RankedView = new System.Windows.Forms.TabPage();
            this.dgv_Ranked = new System.Windows.Forms.DataGridView();
            this.chk_Active = new System.Windows.Forms.CheckBox();
            this.tabpages.SuspendLayout();
            this.tabpage_JudgeEntry.SuspendLayout();
            this.tabpage_RankedView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Ranked)).BeginInit();
            this.SuspendLayout();
            // 
            // tabpages
            // 
            this.tabpages.Controls.Add(this.tabpage_JudgeEntry);
            this.tabpages.Controls.Add(this.tabpage_RankedView);
            this.tabpages.Location = new System.Drawing.Point(0, 0);
            this.tabpages.Name = "tabpages";
            this.tabpages.SelectedIndex = 0;
            this.tabpages.Size = new System.Drawing.Size(673, 396);
            this.tabpages.TabIndex = 0;
            // 
            // tabpage_JudgeEntry
            // 
            this.tabpage_JudgeEntry.Controls.Add(this.chk_Active);
            this.tabpage_JudgeEntry.Controls.Add(this.btn_Save);
            this.tabpage_JudgeEntry.Controls.Add(this.cmb_Institution);
            this.tabpage_JudgeEntry.Controls.Add(this.chk_HasInstitution);
            this.tabpage_JudgeEntry.Controls.Add(this.txt_Name);
            this.tabpage_JudgeEntry.Controls.Add(this.lbl_Name);
            this.tabpage_JudgeEntry.Controls.Add(this.btn_CreateNewJudge);
            this.tabpage_JudgeEntry.Controls.Add(this.lbl_Count);
            this.tabpage_JudgeEntry.Controls.Add(this.tree_Judges);
            this.tabpage_JudgeEntry.Location = new System.Drawing.Point(4, 22);
            this.tabpage_JudgeEntry.Name = "tabpage_JudgeEntry";
            this.tabpage_JudgeEntry.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage_JudgeEntry.Size = new System.Drawing.Size(665, 370);
            this.tabpage_JudgeEntry.TabIndex = 0;
            this.tabpage_JudgeEntry.Text = "Judge Entry";
            this.tabpage_JudgeEntry.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(369, 126);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 7;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // cmb_Institution
            // 
            this.cmb_Institution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Institution.FormattingEnabled = true;
            this.cmb_Institution.Location = new System.Drawing.Point(310, 99);
            this.cmb_Institution.Name = "cmb_Institution";
            this.cmb_Institution.Size = new System.Drawing.Size(134, 21);
            this.cmb_Institution.TabIndex = 6;
            // 
            // chk_HasInstitution
            // 
            this.chk_HasInstitution.AutoSize = true;
            this.chk_HasInstitution.Location = new System.Drawing.Point(211, 103);
            this.chk_HasInstitution.Name = "chk_HasInstitution";
            this.chk_HasInstitution.Size = new System.Drawing.Size(93, 17);
            this.chk_HasInstitution.TabIndex = 5;
            this.chk_HasInstitution.Text = "Has Institution";
            this.chk_HasInstitution.UseVisualStyleBackColor = true;
            // 
            // txt_Name
            // 
            this.txt_Name.Location = new System.Drawing.Point(249, 66);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(195, 20);
            this.txt_Name.TabIndex = 4;
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Location = new System.Drawing.Point(208, 69);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(35, 13);
            this.lbl_Name.TabIndex = 3;
            this.lbl_Name.Text = "Name";
            // 
            // btn_CreateNewJudge
            // 
            this.btn_CreateNewJudge.Location = new System.Drawing.Point(205, 28);
            this.btn_CreateNewJudge.Name = "btn_CreateNewJudge";
            this.btn_CreateNewJudge.Size = new System.Drawing.Size(54, 23);
            this.btn_CreateNewJudge.TabIndex = 2;
            this.btn_CreateNewJudge.Text = "New";
            this.btn_CreateNewJudge.UseVisualStyleBackColor = true;
            // 
            // lbl_Count
            // 
            this.lbl_Count.AutoSize = true;
            this.lbl_Count.Location = new System.Drawing.Point(205, 344);
            this.lbl_Count.Name = "lbl_Count";
            this.lbl_Count.Size = new System.Drawing.Size(38, 13);
            this.lbl_Count.TabIndex = 1;
            this.lbl_Count.Text = "judges";
            // 
            // tree_Judges
            // 
            this.tree_Judges.Location = new System.Drawing.Point(0, 4);
            this.tree_Judges.Name = "tree_Judges";
            this.tree_Judges.Size = new System.Drawing.Size(199, 363);
            this.tree_Judges.TabIndex = 0;
            // 
            // tabpage_RankedView
            // 
            this.tabpage_RankedView.Controls.Add(this.dgv_Ranked);
            this.tabpage_RankedView.Location = new System.Drawing.Point(4, 22);
            this.tabpage_RankedView.Name = "tabpage_RankedView";
            this.tabpage_RankedView.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage_RankedView.Size = new System.Drawing.Size(665, 370);
            this.tabpage_RankedView.TabIndex = 1;
            this.tabpage_RankedView.Text = "Ranked View";
            this.tabpage_RankedView.UseVisualStyleBackColor = true;
            // 
            // dgv_Ranked
            // 
            this.dgv_Ranked.AllowDrop = true;
            this.dgv_Ranked.AllowUserToAddRows = false;
            this.dgv_Ranked.AllowUserToDeleteRows = false;
            this.dgv_Ranked.AllowUserToResizeColumns = false;
            this.dgv_Ranked.AllowUserToResizeRows = false;
            this.dgv_Ranked.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Ranked.Location = new System.Drawing.Point(0, 7);
            this.dgv_Ranked.MultiSelect = false;
            this.dgv_Ranked.Name = "dgv_Ranked";
            this.dgv_Ranked.ReadOnly = true;
            this.dgv_Ranked.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgv_Ranked.RowHeadersVisible = false;
            this.dgv_Ranked.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv_Ranked.ShowCellToolTips = false;
            this.dgv_Ranked.Size = new System.Drawing.Size(659, 328);
            this.dgv_Ranked.TabIndex = 0;
            // 
            // chk_Active
            // 
            this.chk_Active.AutoSize = true;
            this.chk_Active.Checked = true;
            this.chk_Active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Active.Location = new System.Drawing.Point(211, 131);
            this.chk_Active.Name = "chk_Active";
            this.chk_Active.Size = new System.Drawing.Size(56, 17);
            this.chk_Active.TabIndex = 8;
            this.chk_Active.Text = "Active";
            this.chk_Active.UseVisualStyleBackColor = true;
            // 
            // JudgesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabpages);
            this.Name = "JudgesControl";
            this.Size = new System.Drawing.Size(676, 399);
            this.tabpages.ResumeLayout(false);
            this.tabpage_JudgeEntry.ResumeLayout(false);
            this.tabpage_JudgeEntry.PerformLayout();
            this.tabpage_RankedView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Ranked)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabpages;
        private System.Windows.Forms.TabPage tabpage_JudgeEntry;
        private System.Windows.Forms.TreeView tree_Judges;
        private System.Windows.Forms.TabPage tabpage_RankedView;
        private System.Windows.Forms.Label lbl_Count;
        private System.Windows.Forms.Button btn_CreateNewJudge;
        private System.Windows.Forms.TextBox txt_Name;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.ComboBox cmb_Institution;
        private System.Windows.Forms.CheckBox chk_HasInstitution;
        private System.Windows.Forms.DataGridView dgv_Ranked;
        private System.Windows.Forms.CheckBox chk_Active;
    }
}
