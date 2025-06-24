<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

string insert_stmt =
"INSERT INTO [ServicesInfo].[DataStudio_PeopleHierarchy_Snapshot]" +
	"([EmailName]" +
	",[FullName]" +
	",[PreferredFullName]" +
	")" +
"VALUES" +
	"(@EmailName" +
	",@FullName" +
	",@PreferredFullName" +
	");";

string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=test-fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
Regex dashRegex = new Regex("^[A-Za-z]-");
HashSet<string> insertedEmails = new HashSet<string>();

using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	cnx.Open();
	// SqlCommand drop_table = new SqlCommand("TRUNCATE TABLE DataStudio_PeopleHierarchy_Snapshot;", cnx);
	//drop_table.ExecuteNonQuery();

	// Find inserted ones
	SqlCommand qry_emails = new SqlCommand("SELECT [EmailName] FROM [ServicesInfo].[DataStudio_PeopleHierarchy_Snapshot]", cnx);

	SqlDataReader emailsReader = qry_emails.ExecuteReader();

	if (emailsReader.HasRows)
	{
		while (emailsReader.Read())
		{
			insertedEmails.Add(emailsReader.GetString(0).ToLowerInvariant());
		}
	}
	
	emailsReader.Close();

    // Define insertions
	SqlCommand cmd = new SqlCommand(insert_stmt, cnx);
	cmd.Parameters.Add("@EmailName", SqlDbType.NVarChar, 256);

	cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 256);
	cmd.Parameters.Add("@PreferredFullName", SqlDbType.NVarChar, 256);

	using (var csvReader = new StreamReader("C:\\Users\\jujofre\\Desktop\\genevareference.westcentralus.kusto.windows.net--2020-11-25--04-49-58.csv"))
	{
		int rowcount = 1;

		using (var csv = new CsvReader(csvReader, CultureInfo.InvariantCulture))
		{
			var records = csv.GetRecords<dynamic>();
			foreach (var record in records)
			{
				rowcount++;

				if (dashRegex.IsMatch(record.EmailName))
				{
					continue;
				}

				if (insertedEmails.Add(record.EmailName.ToLowerInvariant()))
				{

					cmd.Parameters["@EmailName"].SqlValue = record.EmailName;
					cmd.Parameters["@FullName"].SqlValue = record.FullName;
					cmd.Parameters["@PreferredFullName"].SqlValue = record.PreferredFullName;

					try
					{
						var _ = cmd.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Console.WriteLine($"{ex.Message}\n{ex.Data}");
						Console.WriteLine($"{cmd.CommandText}");
						Console.WriteLine($"{record.EmailName}, {record.FullName}, {record.PreferredFullName}");
						Console.WriteLine($"Row number: {rowcount}");
						throw;
					}
				}

				if ((rowcount % 5000) == 1)
				{
					Console.WriteLine($"rows read: {rowcount}; rows in database: {insertedEmails.Count}");
				}
			}
		}

		Console.WriteLine($"rows read: {rowcount}; rows in database: {insertedEmails.Count}");
	}
}

