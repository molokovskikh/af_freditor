namespace FREditor
{
    partial class frmMatchResult
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.matchResGV = new System.Windows.Forms.DataGridView();
            this.Supplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Matches = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.matchResGV)).BeginInit();
            this.SuspendLayout();
            // 
            // matchResGV
            // 
            this.matchResGV.AllowUserToAddRows = false;
            this.matchResGV.AllowUserToDeleteRows = false;
            this.matchResGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.matchResGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Supplier,
            this.Matches});
            this.matchResGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matchResGV.Location = new System.Drawing.Point(0, 0);
            this.matchResGV.MultiSelect = false;
            this.matchResGV.Name = "matchResGV";
            this.matchResGV.ReadOnly = true;
            this.matchResGV.RowHeadersVisible = false;
            this.matchResGV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.matchResGV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.matchResGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.matchResGV.Size = new System.Drawing.Size(652, 346);
            this.matchResGV.TabIndex = 0;
            this.matchResGV.DoubleClick += new System.EventHandler(this.matchResGV_DoubleClick);
            // 
            // Supplier
            // 
            this.Supplier.HeaderText = "Поставщик";
            this.Supplier.Name = "Supplier";
            this.Supplier.ReadOnly = true;
            this.Supplier.Width = 550;
            // 
            // Matches
            // 
            this.Matches.HeaderText = "Процент совпадений";
            this.Matches.Name = "Matches";
            this.Matches.ReadOnly = true;
            // 
            // frmMatchResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 346);
            this.Controls.Add(this.matchResGV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "frmMatchResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Результат сопоставления";
            ((System.ComponentModel.ISupportInitialize)(this.matchResGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView matchResGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn Supplier;
        private System.Windows.Forms.DataGridViewTextBoxColumn Matches;

    }
}