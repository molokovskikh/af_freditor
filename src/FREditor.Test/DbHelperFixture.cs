using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace FREditor.Test
{
	[TestFixture]
	public class DbHelperFixture
	{
		[Test]
		public void Set_parameters()
		{
			using (var connection = new MySqlConnection(Literals.ConnectionString())) {
				connection.Open();
				DbHelper.SetLogParameters(connection);
			}
		}
	}
}