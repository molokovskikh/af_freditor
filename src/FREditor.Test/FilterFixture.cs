using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Inforoom.WinForms;
using NUnit.Framework;
using Timer = System.Windows.Forms.Timer;

namespace FREditor.Test
{
	[TestFixture]
	public class FilterFixture
	{
		[Test]
		public void Can_search_if_name_not_specified()
		{
			var filter = new Filter(null, 0);
			Assert.That(filter.CanSearch(), Is.False);
			filter.Name = "test";
			Assert.That(filter.CanSearch(), Is.True);
		}

		[Test, STAThread]
		public static void Source_base_test()
		{
			using (var form = new frmFREMain()) {
				var load_method = form.GetType().GetMethod("Form1_Load", BindingFlags.NonPublic | BindingFlags.Instance);
				load_method.Invoke(form, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {form, null}, CultureInfo.CurrentCulture );
				var cbSource = (ComboBox)form.Controls.Find("cbSource", true)[0];
				var cbRegions = (ComboBox)form.Controls.Find("cbRegions", true)[0];
				
				cbSource.SelectedIndex = 4;
				Assert.That(cbSource.Items.Count, Is.EqualTo(5));
				var vrn = cbRegions.Items.Cast<DataRowView>().FirstOrDefault(r => Convert.ToInt32(r[0]) == 1);
				cbRegions.SelectedIndex = cbRegions.Items.IndexOf(vrn);
				Thread.Sleep(2000);

				var load_method1 = form.GetType().GetMethod("tmrSearch_Tick", BindingFlags.NonPublic | BindingFlags.Instance);
				load_method1.Invoke(form, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {form, null}, CultureInfo.CurrentCulture );

				var dataGrid = (INDataGridView)form.Controls.Find("indgvFirm", true)[0];
				var rows =  new DataGridViewRow[dataGrid.Rows.Count];
				dataGrid.Rows.CopyTo(rows, 0);
				Assert.That(dataGrid.Rows.Count, Is.GreaterThan(0));
				var lanCell = dataGrid.Rows.Cast<DataGridViewRow>().Select(r => r.Cells[2].Value).ToList();
				lanCell.ForEach( e => Assert.That(e, Is.EqualTo("LAN")));
			}
		}
	}
}