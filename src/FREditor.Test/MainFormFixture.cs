using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Common.Tools;
using Inforoom.WinForms;
using NUnit.Framework;
using Test.Support;
using Test.Support.Suppliers;

namespace FREditor.Test
{
	[TestFixture, RequiresSTA]
	public class MainFormFixture : IntegrationFixture
	{
		private frmFREMain form;
		private TestSupplier supplier;

		[SetUp]
		public void Setup()
		{
			supplier = TestSupplier.CreateNaked();
			var price = supplier.Prices[0];
			price.CostType = CostType.MultiFile;
			price.Costs[0].PriceItem.Source.SourceType = PriceSourceType.Lan;
			session.Save(supplier);
			session.Transaction.Commit();

			form = new frmFREMain();
			form.Testing = true;
			form.Form1_Load(form, null);
		}

		[Test]
		public void Update_button_state()
		{
			SearchAndSelect();

			var button = form.Controls.Find("createCostCollumnInManyFilesPrice", true)[0];
			Assert.That(button.Enabled, Is.True);
		}

		[Test]
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

			var dataGrid = Grid("indgvFirm");
			var rows = new DataGridViewRow[dataGrid.Rows.Count];
			dataGrid.Rows.CopyTo(rows, 0);
			Assert.That(dataGrid.Rows.Count, Is.GreaterThan(0));
			var lanCell = dataGrid.Rows.Cast<DataGridViewRow>().Select(r => r.Cells[2].Value).ToList();
			lanCell.ForEach(e => Assert.That(e, Is.EqualTo("LAN")));
		}

		[Test]
		public void NotSaveIfPriceInInbound()
		{
			form.InboundPriceItemsForTests = new[] { "123" };
			Assert.That(form.IsPriceInInbound("123", true), Is.True);
			Assert.That(form.IsPriceInInbound("1234", true), Is.False);
		}

		[Test]
		public void Parent_synonym_test()
		{
			session.Transaction.Begin();
			var supplier2 = TestSupplier.CreateNaked();
			supplier2.Prices[0].ParentSynonym = supplier.Prices[0].Id;
			session.Save(supplier2);
			session.Transaction.Commit();

			form.Form1_Load(form, null);

			var cbSynonym = ((ComboBox)form.Controls.Find("cbSynonym", true)[0]);
			foreach (var item in cbSynonym.Items) {
				var row = (DataRowView)item;
				if (Convert.ToUInt32(row[0]) == supplier.Id) {
					cbSynonym.SelectedItem = row;
				}
			}

			form.tmrSearch_Tick(null, null);

			var indgvFirm = Grid("indgvFirm");
			var index = indgvFirm.Rows.Cast<DataGridViewRow>().IndexOf(r => Convert.ToUInt32(((DataRowView)r.DataBoundItem)["CCode"]) == supplier2.Id);
			indgvFirm.CurrentCell = indgvFirm[0, index];

			var grid = Grid("indgvPrice");
			var views = grid.Rows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).Cast<DataRowView>().ToList();
			var dataGridViewRow = views.First();

			Assert.AreEqual(dataGridViewRow.DataView[0][0], supplier.Prices[0].Id);
		}

		[Test]
		public void Update_cost_type()
		{
			session.Transaction.Begin();
			supplier.Prices[0].CostType = CostType.MultiColumn;
			supplier.Prices[0].NewPriceCost();
			session.Save(supplier);
			session.Transaction.Commit();

			SearchAndSelect();

			var grid = Grid("indgvPrice");
			var views = grid.Rows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).Cast<DataRowView>().ToList();
			var dataGridViewRow = views.First();
			dataGridViewRow["PCostType"] = 1;

			form.tsbApply_Click(null, null);
		}

		[Test]
		public void EncodeTest()
		{
			Assert.AreEqual(supplier.Prices[0].Costs[0].PriceItem.Format.PriceEncode, 0);

			SearchAndSelect();

			var tabcontrol = Control<TabControl>("tbControl");
			var tpPrice = Control<TabPage>("tpPrice");
			tabcontrol.SelectedTab = tpPrice;
			form.tbControl_SelectedIndexChanged(null, null);

			var priceEncoding = Control<ComboBox>("priceEncoding");

			priceEncoding.SelectedItem =
				priceEncoding.Items.OfType<EncodeSourceType>().FirstOrDefault(i => i.PriceEncode == 866);
			form.tsbApply_Click(null, null);

			session.Refresh(supplier);
			Assert.AreEqual(supplier.Prices[0].Costs[0].PriceItem.Format.PriceEncode, 866);
		}

		private void SearchAndSelect()
		{
			((CheckBox)form.Controls.Find("checkBoxShowDisabled", true)[0]).Checked = true;
			((TextBox)form.Controls.Find("tbFirmName", true)[0]).Text = "Тест";
			form.tmrSearch_Tick(null, null);

			SelectSupplier();
		}

		private void SelectSupplier()
		{
			var grid = Grid("indgvFirm");
			var index = grid.Rows.Cast<DataGridViewRow>().IndexOf(r => Convert.ToUInt32(((DataRowView)r.DataBoundItem)["CCode"]) == supplier.Id);
			grid.CurrentCell = grid[0, index];
		}

		private T Control<T>(string name) where T : Control
		{
			return (T)form.Controls.Find(name, true).FirstOrDefault();
		}

		private INDataGridView Grid(string name)
		{
			return Control<INDataGridView>(name);
		}
	}
}