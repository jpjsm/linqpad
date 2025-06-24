<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>Microsoft.SqlServer.Server</Namespace>
  <Namespace>System.Data.Sql</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Data.SqlTypes</Namespace>
</Query>

void Main()
{
	string server = "tcp:chggrd-api-sql-svr-ppe.database.windows.net,1433";
	string initialCatalog = "chggrd-api-sql-db-ppe";
	string id = "chggrdadmin";
	string pwd = "M2JhNTkzMDEtMTA4OC00ZGZkLTlkMDYtMzk4MGQxMThkNzM1";//"NzAwMTk0ZDItNjQzOS00MzFiLWIwNTEtOTMzZjQ4YjA0NTE0";
	using (SqlConnection cnx = new SqlConnection($"Server={server};Initial Catalog={initialCatalog};Persist Security Info=False;User ID={id};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
	{
		cnx.Open();
		Console.WriteLine("Connected successfuly");
	}
}

// You can define other methods, fields, classes and namespaces here
