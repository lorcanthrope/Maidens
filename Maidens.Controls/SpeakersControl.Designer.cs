namespace Maidens.Controls
{
    partial class SpeakersControl
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
            this.tree_Speakers = new System.Windows.Forms.TreeView();
            this.btn_CreateNewSpeaker = new System.Windows.Forms.Button();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.lbl_Institution = new System.Windows.Forms.Label();
            this.cmb_Institution = new System.Windows.Forms.ComboBox();
            this.chk_SpecialNeeds = new System.Windows.Forms.CheckBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.lbl_Count = new System.Windows.Forms.Label();
            this.chk_Active = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tree_Speakers
            // 
            this.tree_Speakers.Location = new System.Drawing.Point(4, 4);
            this.tree_Speakers.Name = "tree_Speakers";
            this.tree_Speakers.Size = new System.Drawing.Size(199, 392);
            this.tree_Speakers.TabIndex = 0;
            // 
            // btn_CreateNewSpeaker
            // 
            this.btn_CreateNewSpeaker.Location = new System.Drawing.Point(212, 19);
            this.btn_CreateNewSpeaker.Name = "btn_CreateNewSpeaker";
            this.btn_CreateNewSpeaker.Size = new System.Drawing.Size(54, 23);
            this.btn_CreateNewSpeaker.TabIndex = 1;
            this.btn_CreateNewSpeaker.Text = "New";
            this.btn_CreateNewSpeaker.UseVisualStyleBackColor = true;
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Location = new System.Drawing.Point(209, 64);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(35, 13);
            this.lbl_Name.TabIndex = 2;
            this.lbl_Name.Text = "Name";
            // 
            // txt_Name
            // 
            this.txt_Name.Location = new System.Drawing.Point(250, 61);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(195, 20);
            this.txt_Name.TabIndex = 3;
            // 
            // lbl_Institution
            // 
            this.lbl_Institution.AutoSize = true;
            this.lbl_Institution.Location = new System.Drawing.Point(209, 94);
            this.lbl_Institution.Name = "lbl_Institution";
            this.lbl_Institution.Size = new System.Drawing.Size(52, 13);
            this.lbl_Institution.TabIndex = 4;
            this.lbl_Institution.Text = "Institution";
            // 
            // cmb_Institution
            // 
            this.cmb_Institution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Institution.FormattingEnabled = true;
            this.cmb_Institution.Location = new System.Drawing.Point(267, 91);
            this.cmb_Institution.Name = "cmb_Institution";
            this.cmb_Institution.Size = new System.Drawing.Size(177, 21);
            this.cmb_Institution.TabIndex = 5;
            // 
            // chk_SpecialNeeds
            // 
            this.chk_SpecialNeeds.AutoSize = true;
            this.chk_SpecialNeeds.Location = new System.Drawing.Point(212, 122);
            this.chk_SpecialNeeds.Name = "chk_SpecialNeeds";
            this.chk_SpecialNeeds.Size = new System.Drawing.Size(95, 17);
            this.chk_SpecialNeeds.TabIndex = 6;
            this.chk_SpecialNeeds.Text = "Special Needs";
            this.chk_SpecialNeeds.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(369, 118);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 7;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // lbl_Count
            // 
            this.lbl_Count.AutoSize = true;
            this.lbl_Count.Location = new System.Drawing.Point(209, 372);
            this.lbl_Count.Name = "lbl_Count";
            this.lbl_Count.Size = new System.Drawing.Size(50, 13);
            this.lbl_Count.TabIndex = 8;
            this.lbl_Count.Text = "speakers";
            // 
            // chk_Active
            // 
            this.chk_Active.AutoSize = true;
            this.chk_Active.Checked = true;
            this.chk_Active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Active.Location = new System.Drawing.Point(212, 146);
            this.chk_Active.Name = "chk_Active";
            this.chk_Active.Size = new System.Drawing.Size(56, 17);
            this.chk_Active.TabIndex = 9;
            this.chk_Active.Text = "Active";
            this.chk_Active.UseVisualStyleBackColor = true;
            // 
            // SpeakersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chk_Active);
            this.Controls.Add(this.lbl_Count);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.chk_SpecialNeeds);
            this.Controls.Add(this.cmb_Institution);
            this.Controls.Add(this.lbl_Institution);
            this.Controls.Add(this.txt_Name);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.btn_CreateNewSpeaker);
            this.Controls.Add(this.tree_Speakers);
            this.Name = "SpeakersControl";
            this.Size = new System.Drawing.Size(676, 399);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tree_Speakers;
        private System.Windows.Forms.Button btn_CreateNewSpeaker;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.TextBox txt_Name;
        private System.Windows.Forms.Label lbl_Institution;
        private System.Windows.Forms.ComboBox cmb_Institution;
        private System.Windows.Forms.CheckBox chk_SpecialNeeds;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Label lbl_Count;
        private System.Windows.Forms.CheckBox chk_Active;
    }
}
