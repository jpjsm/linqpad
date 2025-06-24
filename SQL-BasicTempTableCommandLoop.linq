<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>Microsoft.SqlServer.Server</Namespace>
  <Namespace>System.Data.Sql</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Data.SqlTypes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task  Main()
{
	// "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=600;";
	string server = "tcp:fcm-changemanagersql.database.windows.net,1433";// "tcp:chggrd-api-sql-svr-ppe.database.windows.net,1433";
	string initialCatalog = "fcm-changemanagersql"; // "chggrd-api-sql-db-ppe";
	string id = "changemanageradmin";  //"chggrdadmin";
	string pwd = "Ch1ng2M1n1g@r1dm3n"; // "M2JhNTkzMDEtMTA4OC00ZGZkLTlkMDYtMzk4MGQxMThkNzM1";//"NzAwMTk0ZDItNjQzOS00MzFiLWIwNTEtOTMzZjQ4YjA0NTE0";
	
	int rowsinserted;
	
	using (SqlConnection cnx = new SqlConnection($"Server={server};Initial Catalog={initialCatalog};Persist Security Info=False;User ID={id};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
	{
		try
		{
			cnx.Open();
			Console.WriteLine("Connected successfuly");
			
			await CreateTemporaryTablesAsync(cnx);
			
			for (int i = 0; i < 100; i++)
			{
				rowsinserted = await InsertIntoInMemoryTableAsync(cnx, i, $"Label {i,3:0}");
				Console.WriteLine($"Rows Inserted: {rowsinserted}");
			}

			rowsinserted = await UpdateTempTableFromInMemory(cnx);
			Console.WriteLine($"Rows Inserted: {rowsinserted}");

			cnx.Close();
			Console.WriteLine("Connection Closed");
		}
		catch (SqlException xsql)
		{
			Console.WriteLine($"SQL Exception message: {xsql.Message}");
			Console.WriteLine($"SQL Exception message: {xsql.StackTrace}");


			cnx.Close();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Non-sql Exception message: {ex.Message}");

			cnx.Close();
		}
	}
	
	Console.WriteLine("Done.");
}


// You can define other methods, fields, classes and namespaces here
public static async Task CreateTemporaryTablesAsync(SqlConnection cnx)
{
	SqlCommand cmd = new SqlCommand("Create Table #MyTable (TableId int not null, TableLabel nvarchar(25));", cnx);	
	await cmd.ExecuteNonQueryAsync();
}

public static async Task<int>  InsertIntoInMemoryTableAsync(SqlConnection cnx, int id, string label)
{
	SqlCommand cmd = new SqlCommand(" INSERT INTO #MyTable (TableId, TableLabel) VALUES (@ID, @LABEL);", cnx);
	cmd.Parameters.Add("@ID", SqlDbType.Int);
	cmd.Parameters.Add("@LABEL", SqlDbType.NVarChar, 25);
	cmd.Parameters["@ID"].SqlValue = id;
	cmd.Parameters["@LABEL"].SqlValue = label;
	return await cmd.ExecuteNonQueryAsync();
}

public static async Task<int> UpdateTempTableFromInMemory(SqlConnection cnx)
{
	SqlCommand cmd = new SqlCommand("SELECT TableId, TableLabel INTO #MyOtherTable FROM #MyTable;", cnx);
	return await cmd.ExecuteNonQueryAsync();
}
