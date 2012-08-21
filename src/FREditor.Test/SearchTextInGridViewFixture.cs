using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			Assert.DoesNotThrow(() => form.SearchTextInGridView(text, grid, false, false));
		}
	}
}
