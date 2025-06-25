<Query Kind="Program">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	Guid g;
	string cnxstr = "Server=tcp:jujofre-test-sql.database.windows.net,1433;Initial Catalog=Asgard;Persist Security Info=False;User ID=LIf;Password=ABCdef123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
	
	string sql_insert = "INSERT INTO [dbo].[Dagr] ([Id],[DataName]) VALUES(@Dagr_Id, @Dagr_DataName)" +
						"INSERT INTO [dbo].[Joro] ([Id],[DataName]) VALUES(@Joro_Id, @Joro_DataName)" +
						"INSERT INTO [dbo].[Mani] ([Id],[DataName]) VALUES(@Mani_Id, @Mani_DataName)" +
						"INSERT INTO [dbo].[Sol]  ([Id],[DataName]) VALUES(@Sol_Id, @Sol_DataName)";

	string[] sql_qry_table_rows = {
		"SELECT COUNT(*) FROM DBO.DAGR",
		"SELECT COUNT(*) FROM DBO.JORO",
		"SELECT COUNT(*) FROM DBO.MANI",
		"SELECT COUNT(*) FROM DBO.SOL"
};		
	
	using (SqlConnection cnx = new SqlConnection(cnxstr))
	{
		cnx.Open();
	
		SqlCommand insertIntoTables = new SqlCommand(sql_insert, cnx);
		insertIntoTables.Parameters.Add("@Dagr_Id", SqlDbType.UniqueIdentifier);
		insertIntoTables.Parameters.Add("@Dagr_DataName", SqlDbType.NVarChar, 64);
		insertIntoTables.Parameters.Add("@Joro_Id", SqlDbType.UniqueIdentifier);
		insertIntoTables.Parameters.Add("@Joro_DataName", SqlDbType.NVarChar, 64);
		insertIntoTables.Parameters.Add("@Mani_Id", SqlDbType.UniqueIdentifier);
		insertIntoTables.Parameters.Add("@Mani_DataName", SqlDbType.NVarChar, 64);
		insertIntoTables.Parameters.Add("@Sol_Id", SqlDbType.UniqueIdentifier);
		insertIntoTables.Parameters.Add("@Sol_DataName", SqlDbType.NVarChar, 64);
		
		for (int i = 0; i < 100; i++)
		{
			g = Guid.NewGuid();
			insertIntoTables.Parameters["@Dagr_Id"].SqlValue = g;
			insertIntoTables.Parameters["@Dagr_DataName"].SqlValue = Guid2base64(g);

			g = Guid.NewGuid();
			insertIntoTables.Parameters["@Joro_Id"].SqlValue = g;
			insertIntoTables.Parameters["@Joro_DataName"].SqlValue = Guid2base64(g);

			g = Guid.NewGuid();
			insertIntoTables.Parameters["@Mani_Id"].SqlValue = g;
			insertIntoTables.Parameters["@Mani_DataName"].SqlValue = Guid2base64(g);

			g = Guid.NewGuid();
			insertIntoTables.Parameters["@Sol_Id"].SqlValue = g;
			insertIntoTables.Parameters["@Sol_DataName"].SqlValue = Guid2base64(g);

			insertIntoTables.ExecuteNonQuery();
		}
		
		foreach(string qry in sql_qry_table_rows)
		{
			SqlCommand qryTableRowCount = new SqlCommand(qry, cnx);
			Console.WriteLine($"{qry} --> {qryTableRowCount.ExecuteScalar()}");
		}
	}
}

// You can define other methods, fields, classes and namespaces here
private string Guid2base64(Guid g)
{
	return Convert.ToBase64String(g.ToByteArray()).Replace("/", "_").Replace("+", "-").Replace("=", string.Empty);
}