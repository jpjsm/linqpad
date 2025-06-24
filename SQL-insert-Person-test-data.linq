<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

Random rnd = new Random();

string insert_stmt =
"INSERT INTO [dbo].[Person] ([Id],[Name],[Height]) VALUES(@Id,@Name,@Height);";

string cnxstr = "Server=tcp:fcm-test03-sqldb.database.windows.net,1433;Initial Catalog=fcm-test03-sqldb;Persist Security Info=False;User ID=fcm-test03-sqladmin;Password=uI18rwBd5ATE^q7h8bj&;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	cnx.Open();
	SqlCommand cmd = new SqlCommand(insert_stmt, cnx);
	cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier);

	cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
	cmd.Parameters.Add("@Height", SqlDbType.Real);

	int rowcount = 1;
	while (rowcount <= 100)
	{


		cmd.Parameters["@Id"].SqlValue = Guid.NewGuid();
		cmd.Parameters["@Name"].SqlValue = $"Name {rowcount}";
		cmd.Parameters["@Height"].SqlValue = 172.0 + rnd.NextDouble()*12.0 - 6.0;

		try
		{
			var _ = cmd.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{ex.Message}\n{ex.Data}");
			Console.WriteLine($"{cmd.CommandText}");
			Console.WriteLine($"Row number: {rowcount}");
			throw;
		}
		
		rowcount++;
	}
	
	Console.WriteLine($"rows inserted: {rowcount}");
}

