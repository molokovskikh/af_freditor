using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Inforoom.WinForms;

namespace Inforoom.FREditor
{
	public partial class frmPriceDataSearch : Form
	{
		private INDataGridView _grid;

		public frmPriceDataSearch(INDataGridView grid)
		{
			InitializeComponent();
			_grid = grid;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			_grid.CurrentCell = _grid.Rows[10].Cells[3];
		}
	}
}
