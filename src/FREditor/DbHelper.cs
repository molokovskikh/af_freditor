using System;
using MySql.Data.MySqlClient;

namespace FREditor
{
	public class DbHelper
	{
		public static void SetLogParameters(MySqlConnection connection)
		{
			var command = new MySqlCommand(
				"set @INHost = ?Host; set @INUser = ?User;", connection);
			command.Parameters.AddWithValue("?Host", Environment.MachineName);
			command.Parameters.AddWithValue("?User", Environment.UserName);
			command.ExecuteNonQuery();
		}
	}
}