using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Inforoom.WinForms;
using NUnit.Framework;
using Test.Support;
using Test.Support.Suppliers;

namespace FREditor.Test
{
	[TestFixture]
	public class MainFormFixture
	{
		private frmFREMain form;

		[SetUp]
		public void Setup()
		{
			var supplier = TestSupplier.Create();
			supplier.Prices[0].CostType = CostType.MultiFile;
			supplier.Save();

			form = new frmFREMain();
			form.Form1_Load(form, null);
		}

		[Test, STAThread]
		public void Update_button_state()
		{
			((TextBox)form.Controls.Find("tbFirmName", true)[0]).Text = "Тест";
			((CheckBox)form.Controls.Find("checkBoxShowDisabled", true)[0]).Checked = true;
			form.tmrSearch_Tick(null, null);
			var button = form.Controls.Find("createCostCollumnInManyFilesPrice", true)[0];
			Assert.That(button.Enabled, Is.True);
		}

		[Test, STAThread]
		public void Source_base_test()
		{
			var cbSource = (ComboBox)form.Controls.Find("cbSource", true)[0];
			var cbRegions = (ComboBox)form.Controls.Find("cbRegions", true)[0];

			cbSource.SelectedIndex = 4;
			Assert.That(cbSource.Items.Count, Is.EqualTo(5));
			var vrn = cbRegions.Items.Cast<DataRowView>().FirstOrDefault(r => Convert.ToInt64(r[0]) == 1);
			cbRegions.SelectedIndex = cbRegions.Items.IndexOf(vrn);
			Thread.Sleep(2000);

			form.tmrSearch_Tick(null, null);

			var dataGrid = (INDataGridView)form.Controls.Find("indgvFirm", true)[0];
			var rows =  new DataGridViewRow[dataGrid.Rows.Count];
			dataGrid.Rows.CopyTo(rows, 0);
			Assert.That(dataGrid.Rows.Count, Is.GreaterThan(0));
			var lanCell = dataGrid.Rows.Cast<DataGridViewRow>().Select(r => r.Cells[2].Value).ToList();
			lanCell.ForEach( e => Assert.That(e, Is.EqualTo("LAN")));
		}
	}
}