namespace Maidens.Controls
{
    partial class VenuesControl
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
            this.tree_Venues = new System.Windows.Forms.TreeView();
            this.btn_CreateNewVenue = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.chk_SpecialNeeds = new System.Windows.Forms.CheckBox();
            this.lbl_Count = new System.Windows.Forms.Label();
            this.chk_Active = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tree_Venues
            // 
            this.tree_Venues.Location = new System.Drawing.Point(3, 0);
            this.tree_Venues.Name = "tree_Venues";
            this.tree_Venues.Size = new System.Drawing.Size(199, 392);
            this.tree_Venues.TabIndex = 1;
            // 
            // btn_CreateNewVenue
            // 
            this.btn_CreateNewVenue.Location = new System.Drawing.Point(209, 21);
            this.btn_CreateNewVenue.Name = "btn_CreateNewVenue";
            this.btn_CreateNewVenue.Size = new System.Drawing.Size(54, 23);
            this.btn_CreateNewVenue.TabIndex = 8;
            this.btn_CreateNewVenue.Text = "New";
            this.btn_CreateNewVenue.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(448, 61);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 7;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // txt_Name
            // 
            this.txt_Name.Location = new System.Drawing.Point(247, 63);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(195, 20);
            this.txt_Name.TabIndex = 6;
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Location = new System.Drawing.Point(206, 66);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(35, 13);
            this.lbl_Name.TabIndex = 5;
            this.lbl_Name.Text = "Name";
            // 
            // chk_SpecialNeeds
            // 
            this.chk_SpecialNeeds.AutoSize = true;
            this.chk_SpecialNeeds.Location = new System.Drawing.Point(209, 89);
            this.chk_SpecialNeeds.Name = "chk_SpecialNeeds";
            this.chk_SpecialNeeds.Size = new System.Drawing.Size(95, 17);
            this.chk_SpecialNeeds.TabIndex = 9;
            this.chk_SpecialNeeds.Text = "Special Needs";
            this.chk_SpecialNeeds.UseVisualStyleBackColor = true;
            // 
            // lbl_Count
            // 
            this.lbl_Count.AutoSize = true;
            this.lbl_Count.Location = new System.Drawing.Point(208, 379);
            this.lbl_Count.Name = "lbl_Count";
            this.lbl_Count.Size = new System.Drawing.Size(45, 13);
            this.lbl_Count.TabIndex = 10;
            this.lbl_Count.Text = " venues";
            // 
            // chk_Active
            // 
            this.chk_Active.AutoSize = true;
            this.chk_Active.Checked = true;
            this.chk_Active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Active.Location = new System.Drawing.Point(209, 112);
            this.chk_Active.Name = "chk_Active";
            this.chk_Active.Size = new System.Drawing.Size(56, 17);
            this.chk_Active.TabIndex = 11;
            this.chk_Active.Text = "Active";
            this.chk_Active.UseVisualStyleBackColor = true;
            // 
            // VenuesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chk_Active);
            this.Controls.Add(this.lbl_Count);
            this.Controls.Add(this.chk_SpecialNeeds);
            this.Controls.Add(this.btn_CreateNewVenue);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.txt_Name);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.tree_Venues);
            this.Name = "VenuesControl";
            this.Size = new System.Drawing.Size(676, 399);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tree_Venues;
        private System.Windows.Forms.Button btn_CreateNewVenue;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TextBox txt_Name;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.CheckBox chk_SpecialNeeds;
        private System.Windows.Forms.Label lbl_Count;
        private System.Windows.Forms.CheckBox chk_Active;
    }
}
