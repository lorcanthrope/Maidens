namespace Maidens.Controls
{
    partial class InstitutionsControl
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
            this.tree_Institutions = new System.Windows.Forms.TreeView();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_CreateNewInstitution = new System.Windows.Forms.Button();
            this.lbl_Count = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tree_Institutions
            // 
            this.tree_Institutions.Location = new System.Drawing.Point(4, 4);
            this.tree_Institutions.Name = "tree_Institutions";
            this.tree_Institutions.Size = new System.Drawing.Size(199, 392);
            this.tree_Institutions.TabIndex = 0;
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Location = new System.Drawing.Point(209, 64);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(35, 13);
            this.lbl_Name.TabIndex = 1;
            this.lbl_Name.Text = "Name";
            // 
            // txt_Name
            // 
            this.txt_Name.Location = new System.Drawing.Point(250, 61);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(195, 20);
            this.txt_Name.TabIndex = 2;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(451, 59);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 3;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_CreateNewInstitution
            // 
            this.btn_CreateNewInstitution.Location = new System.Drawing.Point(212, 19);
            this.btn_CreateNewInstitution.Name = "btn_CreateNewInstitution";
            this.btn_CreateNewInstitution.Size = new System.Drawing.Size(54, 23);
            this.btn_CreateNewInstitution.TabIndex = 4;
            this.btn_CreateNewInstitution.Text = "New";
            this.btn_CreateNewInstitution.UseVisualStyleBackColor = true;
            // 
            // lbl_Count
            // 
            this.lbl_Count.AutoSize = true;
            this.lbl_Count.Location = new System.Drawing.Point(209, 372);
            this.lbl_Count.Name = "lbl_Count";
            this.lbl_Count.Size = new System.Drawing.Size(65, 13);
            this.lbl_Count.TabIndex = 5;
            this.lbl_Count.Text = "0 institutions";
            // 
            // InstitutionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_Count);
            this.Controls.Add(this.btn_CreateNewInstitution);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.txt_Name);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.tree_Institutions);
            this.Name = "InstitutionsControl";
            this.Size = new System.Drawing.Size(676, 399);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tree_Institutions;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.TextBox txt_Name;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_CreateNewInstitution;
        private System.Windows.Forms.Label lbl_Count;
    }
}
