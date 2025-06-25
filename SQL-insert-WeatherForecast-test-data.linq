<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

Random rnd = new Random();

string insert_stmt =
"INSERT INTO [dbo].[WeatherForecast] ([Date],[TemperatureC],[Summary]) VALUES(@Date,@TemperatureC,@Summary);";

string cnxstr = "Server=tcp:fcm-test03-sqldb.database.windows.net,1433;Initial Catalog=fcm-test03-sqldb;Persist Security Info=False;User ID=fcm-test03-sqladmin;Password=uI18rwBd5ATE^q7h8bj&;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


int Temperature = 4;
DateTime start = DateTime.UtcNow;
using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	cnx.Open();
	SqlCommand cmd = new SqlCommand(insert_stmt, cnx);
	cmd.Parameters.Add("@Date", SqlDbType.DateTime2);

	cmd.Parameters.Add("@TemperatureC", SqlDbType.Int);
	cmd.Parameters.Add("@Summary", SqlDbType.NVarChar, 50);

	int rowcount = 1;
	while (rowcount <= 100)
	{
		cmd.Parameters["@Date"].SqlValue = start;
		cmd.Parameters["@TemperatureC"].SqlValue = Temperature;
		cmd.Parameters["@Summary"].SqlValue = GetSummary(Temperature);

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
		
		Temperature = Temperature + rnd.Next(-3,5);
		start = start.AddHours(6.0);
		rowcount++;
	}
	
	Console.WriteLine($"rows inserted: {rowcount}");
}

}

string GetSummary(int t)
{
	int[] summary_values = { int.MinValue, -10, 0, 5, 13, 20, 25, 30, 38, 43 };
	string[] summary_labels = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
	
	int i = 0;
	while (i < summary_values.Length )
	{
		if (t < summary_values[i])
		{
			break;
		}
		
		i++;
	}
	
	return summary_labels[i-1];
	
//}
