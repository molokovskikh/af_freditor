using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;

namespace FREditor
{
	/// <summary>
	/// Summary description for frmNameMask.
	/// </summary>
	public class frmNameMask : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox txtBoxNameMaskNM;
		public TextBox txtBoxName;
		private System.Windows.Forms.Panel pnlMask;
		private System.Windows.Forms.Panel pnlButtons;
		private System.Windows.Forms.Button btnCheck;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Data.DataSet dtSet;
		private System.Data.DataTable dtGroups;
		private System.Data.DataColumn GName;
		private System.Data.DataColumn GValue;
		private Inforoom.WinForms.INDataGridView indgvGroups;
		private DataGridViewTextBoxColumn gNameDataGridViewTextBoxColumn;
		private DataGridViewTextBoxColumn gValueDataGridViewTextBoxColumn;
		private IContainer components;

		public frmNameMask()
		{
			// Required for Windows Form Designer support

			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.pnlMask = new System.Windows.Forms.Panel();
			this.txtBoxName = new System.Windows.Forms.TextBox();
			this.txtBoxNameMaskNM = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.btnCheck = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.dtSet = new System.Data.DataSet();
			this.dtGroups = new System.Data.DataTable();
			this.GName = new System.Data.DataColumn();
			this.GValue = new System.Data.DataColumn();
			this.indgvGroups = new Inforoom.WinForms.INDataGridView();
			this.gNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gValueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.pnlMask.SuspendLayout();
			this.pnlButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dtSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtGroups)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.indgvGroups)).BeginInit();
			this.SuspendLayout();
			//
			// pnlMask
			//
			this.pnlMask.Controls.Add(this.txtBoxName);
			this.pnlMask.Controls.Add(this.txtBoxNameMaskNM);
			this.pnlMask.Controls.Add(this.label2);
			this.pnlMask.Controls.Add(this.label1);
			this.pnlMask.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlMask.Location = new System.Drawing.Point(0, 0);
			this.pnlMask.Name = "pnlMask";
			this.pnlMask.Size = new System.Drawing.Size(504, 93);
			this.pnlMask.TabIndex = 8;
			//
			// txtBoxName
			//
			this.txtBoxName.Location = new System.Drawing.Point(126, 48);
			this.txtBoxName.Name = "txtBoxName";
			this.txtBoxName.Size = new System.Drawing.Size(362, 20);
			this.txtBoxName.TabIndex = 7;
			//
			// txtBoxNameMaskNM
			//
			this.txtBoxNameMaskNM.Location = new System.Drawing.Point(126, 24);
			this.txtBoxNameMaskNM.Name = "txtBoxNameMaskNM";
			this.txtBoxNameMaskNM.Size = new System.Drawing.Size(362, 20);
			this.txtBoxNameMaskNM.TabIndex = 6;
			this.txtBoxNameMaskNM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxNameMaskNM_KeyDown);
			//
			// label2
			//
			this.label2.Location = new System.Drawing.Point(22, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Наименование :";
			//
			// label1
			//
			this.label1.Location = new System.Drawing.Point(22, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Маска :";
			//
			// pnlButtons
			//
			this.pnlButtons.Controls.Add(this.btnCheck);
			this.pnlButtons.Controls.Add(this.btnCancel);
			this.pnlButtons.Controls.Add(this.btnOK);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlButtons.Location = new System.Drawing.Point(400, 93);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(104, 216);
			this.pnlButtons.TabIndex = 9;
			//
			// btnCheck
			//
			this.btnCheck.Location = new System.Drawing.Point(16, 80);
			this.btnCheck.Name = "btnCheck";
			this.btnCheck.Size = new System.Drawing.Size(75, 23);
			this.btnCheck.TabIndex = 10;
			this.btnCheck.Text = "Check";
			this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
			//
			// btnCancel
			//
			this.btnCancel.Location = new System.Drawing.Point(16, 48);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			//
			// btnOK
			//
			this.btnOK.Location = new System.Drawing.Point(16, 16);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			//
			// dtSet
			//
			this.dtSet.DataSetName = "NewDataSet";
			this.dtSet.Locale = new System.Globalization.CultureInfo("ru-RU");
			this.dtSet.Tables.AddRange(new System.Data.DataTable[] {
				this.dtGroups
			});
			//
			// dtGroups
			//
			this.dtGroups.Columns.AddRange(new System.Data.DataColumn[] {
				this.GName,
				this.GValue
			});
			this.dtGroups.TableName = "dtGroups";
			//
			// GName
			//
			this.GName.ColumnName = "GName";
			//
			// GValue
			//
			this.GValue.ColumnName = "GValue";
			//
			// indgvGroups
			//
			this.indgvGroups.AllowUserToAddRows = false;
			this.indgvGroups.AllowUserToDeleteRows = false;
			this.indgvGroups.AllowUserToResizeRows = false;
			this.indgvGroups.AutoGenerateColumns = false;
			this.indgvGroups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.indgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.indgvGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
				this.gNameDataGridViewTextBoxColumn,
				this.gValueDataGridViewTextBoxColumn
			});
			this.indgvGroups.DataMember = "dtGroups";
			this.indgvGroups.DataSource = this.dtSet;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GrayText;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.indgvGroups.DefaultCellStyle = dataGridViewCellStyle2;
			this.indgvGroups.Dock = System.Windows.Forms.DockStyle.Fill;
			this.indgvGroups.Location = new System.Drawing.Point(0, 93);
			this.indgvGroups.Name = "indgvGroups";
			this.indgvGroups.ReadOnly = true;
			this.indgvGroups.RowHeadersVisible = false;
			this.indgvGroups.Size = new System.Drawing.Size(400, 216);
			this.indgvGroups.TabIndex = 11;
			//
			// gNameDataGridViewTextBoxColumn
			//
			this.gNameDataGridViewTextBoxColumn.DataPropertyName = "GName";
			this.gNameDataGridViewTextBoxColumn.HeaderText = "Группа";
			this.gNameDataGridViewTextBoxColumn.Name = "gNameDataGridViewTextBoxColumn";
			this.gNameDataGridViewTextBoxColumn.ReadOnly = true;
			//
			// gValueDataGridViewTextBoxColumn
			//
			this.gValueDataGridViewTextBoxColumn.DataPropertyName = "GValue";
			dataGridViewCellStyle1.NullValue = "Значение не задано";
			this.gValueDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
			this.gValueDataGridViewTextBoxColumn.HeaderText = "Значение";
			this.gValueDataGridViewTextBoxColumn.Name = "gValueDataGridViewTextBoxColumn";
			this.gValueDataGridViewTextBoxColumn.ReadOnly = true;
			//
			// frmNameMask
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(504, 309);
			this.Controls.Add(this.indgvGroups);
			this.Controls.Add(this.pnlButtons);
			this.Controls.Add(this.pnlMask);
			this.Name = "frmNameMask";
			this.Text = "Маска разбора товара";
			this.Load += new System.EventHandler(this.frmNameMask_Load);
			this.pnlMask.ResumeLayout(false);
			this.pnlMask.PerformLayout();
			this.pnlButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dtSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtGroups)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.indgvGroups)).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private void frmNameMask_Load(object sender, System.EventArgs e)
		{
			Check();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			//this.Close();
			this.DialogResult = DialogResult.Cancel;
		}

		private void Check()
		{
			var r = new Regex(txtBoxNameMaskNM.Text);
			var m = r.Match(txtBoxName.Text);

			foreach (PriceFields pf in Enum.GetValues(typeof(PriceFields))) {
				if ((pf.ToString() == PriceFields.Name1.ToString()) || (pf.ToString() == PriceFields.Name2.ToString()) || (pf.ToString() == PriceFields.Name3.ToString())) {
					if (m.Groups["Name"].Success) {
						if (dtGroups.Rows.Count > 0) {
							DataRow dr = dtGroups.Rows[dtGroups.Rows.Count - 1];
							if (dr[GName].ToString() != "Name") {
								DataRow newDR = dtGroups.NewRow();
								newDR[GName] = "Name";
								if (m.Groups["Name"].Value != String.Empty)
									newDR[GValue] = m.Groups["Name"].Value;
								else
									newDR[GValue] = "Значение не определено";
								dtGroups.Rows.Add(newDR);
							}
						}
						else {
							DataRow newDR = dtGroups.NewRow();
							newDR[GName] = "Name";
							if (m.Groups["Name"].Value != String.Empty)
								newDR[GValue] = m.Groups["Name"].Value;
							else
								newDR[GValue] = "Значение не определено";
							dtGroups.Rows.Add(newDR);
						}
					}
				}
				else if (m.Groups[pf.ToString()].Success) {
					DataRow newDR = dtGroups.NewRow();
					newDR[GName] = pf.ToString();
					newDR[GValue] = m.Groups[pf.ToString()].Value;
					dtGroups.Rows.Add(newDR);
				}
			}
		}

		private void btnCheck_Click(object sender, System.EventArgs e)
		{
			Check();
		}

		private void txtBoxNameMaskNM_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				this.DialogResult = DialogResult.OK;
		}
	}
}