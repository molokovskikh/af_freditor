using NUnit.Framework;

namespace FREditor.Test
{
	[TestFixture]
	public class FilterFixture
	{
		[Test]
		public void Can_search_if_name_not_specified()
		{
			var filter = new Filter(null, 0, 0, 0);
			Assert.That(filter.CanSearch(), Is.False);
			filter.Name = "test";
			Assert.That(filter.CanSearch(), Is.True);
		}
	}
}