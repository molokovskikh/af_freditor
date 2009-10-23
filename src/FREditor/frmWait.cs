using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace FREditor
{
	/// <summary>
	/// Summary description for frmWait.
	/// </summary>
	public class frmWait : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblWait;
		private System.ComponentModel.IContainer components;

		public bool Stop = false;

		public frmFREMain.OpenPriceDelegate openPrice = null;
		private System.Windows.Forms.Timer tmrWait;
		public System.Data.DataRow drP = null;

		public frmWait()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.lblWait = new System.Windows.Forms.Label();
			this.tmrWait = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// lblWait
			// 
			this.lblWait.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.lblWait.Location = new System.Drawing.Point(0, 0);
			this.lblWait.Name = "lblWait";
			this.lblWait.Size = new System.Drawing.Size(442, 130);
			this.lblWait.TabIndex = 0;
			this.lblWait.Text = "Пожалуйста, подождите...";
			this.lblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tmrWait
			// 
			this.tmrWait.Tick += new System.EventHandler(this.tmrWait_Tick);
			// 
			// frmWait
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(442, 130);
			this.ControlBox = false;
			this.Controls.Add(this.lblWait);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "frmWait";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Load += new System.EventHandler(this.frmWait_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void tmrWait_Tick(object sender, System.EventArgs e)
		{
			tmrWait.Enabled = false;
			if ((openPrice != null) && (drP != null))
				try
				{
					openPrice(drP);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Не удалось открыть прайс-лист. Сообщение было отправлено разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Program.SendMessageOnException(null, new Exception("Ошибка при открытии прайс-листа.", ex));
				}

			Close();
		}

		private void frmWait_Load(object sender, System.EventArgs e)
		{
			tmrWait.Enabled = true;
		}

	}
}
