namespace Maidens.Controls
{
    partial class RoundsControl
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
            this.btn_CreateNewRound = new System.Windows.Forms.Button();
            this.btn_LoadRound = new System.Windows.Forms.Button();
            this.dgv_Round = new System.Windows.Forms.DataGridView();
            this.chk_Powerpair = new System.Windows.Forms.CheckBox();
            this.lbl_Motion = new System.Windows.Forms.Label();
            this.txt_Motion = new System.Windows.Forms.TextBox();
            this.btn_RerunBallots = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Round)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_Rounds
            // 
            this.cmb_Rounds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Rounds.FormattingEnabled = true;
            this.cmb_Rounds.Location = new System.Drawing.Point(13, 13);
            this.cmb_Rounds.Name = "cmb_Rounds";
            this.cmb_Rounds.Size = new System.Drawing.Size(121, 21);
            this.cmb_Rounds.TabIndex = 0;
            // 
            // btn_CreateNewRound
            // 
            this.btn_CreateNewRound.Location = new System.Drawing.Point(221, 11);
            this.btn_CreateNewRound.Name = "btn_CreateNewRound";
            this.btn_CreateNewRound.Size = new System.Drawing.Size(119, 23);
            this.btn_CreateNewRound.TabIndex = 1;
            this.btn_CreateNewRound.Text = "Create New Round";
            this.btn_CreateNewRound.UseVisualStyleBackColor = true;
            // 
            // btn_LoadRound
            // 
            this.btn_LoadRound.Location = new System.Drawing.Point(140, 11);
            this.btn_LoadRound.Name = "btn_LoadRound";
            this.btn_LoadRound.Size = new System.Drawing.Size(75, 23);
            this.btn_LoadRound.TabIndex = 2;
            this.btn_LoadRound.Text = "Load Round";
            this.btn_LoadRound.UseVisualStyleBackColor = true;
            // 
            // dgv_Round
            // 
            this.dgv_Round.AllowUserToAddRows = false;
            this.dgv_Round.AllowUserToDeleteRows = false;
            this.dgv_Round.AllowUserToResizeColumns = false;
            this.dgv_Round.AllowUserToResizeRows = false;
            this.dgv_Round.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgv_Round.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Round.Location = new System.Drawing.Point(4, 65);
            this.dgv_Round.MultiSelect = false;
            this.dgv_Round.Name = "dgv_Round";
            this.dgv_Round.ReadOnly = true;
            this.dgv_Round.RowHeadersVisible = false;
            this.dgv_Round.Size = new System.Drawing.Size(669, 331);
            this.dgv_Round.TabIndex = 3;
            // 
            // chk_Powerpair
            // 
            this.chk_Powerpair.AutoSize = true;
            this.chk_Powerpair.Location = new System.Drawing.Point(346, 15);
            this.chk_Powerpair.Name = "chk_Powerpair";
            this.chk_Powerpair.Size = new System.Drawing.Size(73, 17);
            this.chk_Powerpair.TabIndex = 4;
            this.chk_Powerpair.Text = "Powerpair";
            this.chk_Powerpair.UseVisualStyleBackColor = true;
            // 
            // lbl_Motion
            // 
            this.lbl_Motion.AutoSize = true;
            this.lbl_Motion.Location = new System.Drawing.Point(20, 45);
            this.lbl_Motion.Name = "lbl_Motion";
            this.lbl_Motion.Size = new System.Drawing.Size(42, 13);
            this.lbl_Motion.TabIndex = 5;
            this.lbl_Motion.Text = "Motion:";
            // 
            // txt_Motion
            // 
            this.txt_Motion.Location = new System.Drawing.Point(74, 42);
            this.txt_Motion.Name = "txt_Motion";
            this.txt_Motion.Size = new System.Drawing.Size(559, 20);
            this.txt_Motion.TabIndex = 6;
            // 
            // btn_RerunBallots
            // 
            this.btn_RerunBallots.Location = new System.Drawing.Point(563, 11);
            this.btn_RerunBallots.Name = "btn_RerunBallots";
            this.btn_RerunBallots.Size = new System.Drawing.Size(91, 23);
            this.btn_RerunBallots.TabIndex = 7;
            this.btn_RerunBallots.Text = "Rerun Ballots";
            this.btn_RerunBallots.UseVisualStyleBackColor = true;
            // 
            // RoundsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_RerunBallots);
            this.Controls.Add(this.txt_Motion);
            this.Controls.Add(this.lbl_Motion);
            this.Controls.Add(this.chk_Powerpair);
            this.Controls.Add(this.dgv_Round);
            this.Controls.Add(this.btn_LoadRound);
            this.Controls.Add(this.btn_CreateNewRound);
            this.Controls.Add(this.cmb_Rounds);
            this.Name = "RoundsControl";
            this.Size = new System.Drawing.Size(676, 399);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Round)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_Rounds;
        private System.Windows.Forms.Button btn_CreateNewRound;
        private System.Windows.Forms.Button btn_LoadRound;
        private System.Windows.Forms.DataGridView dgv_Round;
        private System.Windows.Forms.CheckBox chk_Powerpair;
        private System.Windows.Forms.Label lbl_Motion;
        private System.Windows.Forms.TextBox txt_Motion;
        private System.Windows.Forms.Button btn_RerunBallots;

    }
}
