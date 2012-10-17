using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Inforoom.WinForms;
using Test.Support;
using NUnit.Framework;
using System.Windows.Forms;



namespace FREditor.Test
{
	[TestFixture, RequiresSTA]
	class SearchTextInGridViewFixture
	{
		private DataGridView grid ;
		private string text;
		private frmFREMain form;

		[SetUp]
		public void Setup()
		{
			grid = new DataGridView();
			form = new frmFREMain();
			text = "test";
		}

		[Test]
		public void SearchTextInGridView()
		{
			Assert.IsNull(grid.CurrentRow);
			form.SearchTextInGridView(text, grid, false, false);
		}

		[Test]
		public void ChangeActiveTabTest()
		{
			form.dtables = new List<DataTable>();
			form.dtables.Add(new DataTable());
			form.GDS = new ArrayList();
			form.GDS.Add(new INDataGridView {
				Name = "TestGDSName"
			});
			form.TCInnerSheets = new TabControl();
			form.TCInnerSheets.TabPages.Add(new TabPage("page1"));
			var page = new TabPage("page2");
			form.TCInnerSheets.TabPages.Add(page);
			form.TCInnerSheets.TabPages[1].Select();
			form.TCInnerSheets.SelectedIndex = 0;
			form.TCInnerSheets.SelectedIndex = 1;
			form.TCInnerSheets.SelectedTab = page;
			form.tcInnerSheets_SelectedIndexChanged(null, null);
			Assert.That(form.SearchGrid.Name.Contains("TestGDSName"));
		}
	}
}
