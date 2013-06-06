namespace Maidens.Controls
{
    partial class StandingsControl
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
            this.btn_Load = new System.Windows.Forms.Button();
            this.dgv_Standings = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Standings)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_Rounds
            // 
            this.cmb_Rounds.FormattingEnabled = true;
            this.cmb_Rounds.Location = new System.Drawing.Point(14, 12);
            this.cmb_Rounds.Name = "cmb_Rounds";
            this.cmb_Rounds.Size = new System.Drawing.Size(121, 21);
            this.cmb_Rounds.TabIndex = 0;
            // 
            // btn_Load
            // 
            this.btn_Load.Location = new System.Drawing.Point(141, 10);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(75, 23);
            this.btn_Load.TabIndex = 1;
            this.btn_Load.Text = "Load Round";
            this.btn_Load.UseVisualStyleBackColor = true;
            // 
            // dgv_Standings
            // 
            this.dgv_Standings.AllowUserToAddRows = false;
            this.dgv_Standings.AllowUserToDeleteRows = false;
            this.dgv_Standings.AllowUserToResizeColumns = false;
            this.dgv_Standings.AllowUserToResizeRows = false;
            this.dgv_Standings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Standings.Location = new System.Drawing.Point(4, 58);
            this.dgv_Standings.Name = "dgv_Standings";
            this.dgv_Standings.ReadOnly = true;
            this.dgv_Standings.RowHeadersVisible = false;
            this.dgv_Standings.Size = new System.Drawing.Size(669, 321);
            this.dgv_Standings.TabIndex = 2;
            // 
            // StandingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv_Standings);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.cmb_Rounds);
            this.Name = "StandingsControl";
            this.Size = new System.Drawing.Size(676, 399);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Standings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_Rounds;
        private System.Windows.Forms.Button btn_Load;
        private System.Windows.Forms.DataGridView dgv_Standings;
    }
}
