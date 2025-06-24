<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

string insert_stmt = 
"INSERT INTO [ServicesInfo].[ServiceTree_OrganizationCommonMetadata]" +
	"([OrganizationId]" +
	",[OrganizationName]" +
	",[OrganizationLevel]" +
	",[BusinessOwner]" +
	")" + 
"VALUES" +
	"(@OrganizationId" +
	",@OrganizationName" +
	",@OrganizationLevel" +
	",@BusinessOwner" +
	");";

string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=test-fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	using (TextReader  reader = File.OpenText(@"C:\Users\jujofre\Desktop\ServiceTree_OrganizationCommonMetadata.csv"))
	{
		//reader.ReadLine();
		
		using (var csvfile = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
		{
			cnx.Open();
			SqlCommand drop_table = new SqlCommand("TRUNCATE TABLE [ServicesInfo].[ServiceTree_OrganizationCommonMetadata];", cnx);
			Console.WriteLine($"Total rows dropped: {drop_table.ExecuteNonQuery()}");
			SqlCommand cmd = new SqlCommand(insert_stmt, cnx);
			cmd.Parameters.Add("@OrganizationId", SqlDbType.UniqueIdentifier);

			cmd.Parameters.Add("@OrganizationName", SqlDbType.NVarChar, 128);
			cmd.Parameters.Add("@OrganizationLevel", SqlDbType.NVarChar, 128);
			cmd.Parameters.Add("@BusinessOwner", SqlDbType.NVarChar, 256);

			var lines = csvfile.GetRecords<ServiceTree_OrganizationCommonMetadata>();

			int rowcount = 0;
			foreach (var h in lines)
			{
				// h is the same instance as hierarchy.			
				cmd.Parameters["@OrganizationId"].SqlValue = h.OrganizationId;
				
				cmd.Parameters["@OrganizationName"].SqlValue = h.OrganizationName;
				cmd.Parameters["@OrganizationLevel"].SqlValue = h.OrganizationLevel;
				cmd.Parameters["@BusinessOwner"].SqlValue = h.BusinessOwner;
				try
				{
					++rowcount;
					
					var _ = cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"{ex.Message}\n{ex.Data}");
					Console.WriteLine($"{cmd.CommandText}");
					Console.WriteLine($"{h}");
					Console.WriteLine($"Row number: {rowcount}");
					throw;
				}

				// Console.WriteLine($"{rowcount,7:N0} {h}");
			}

			Console.WriteLine($"Total rows: {rowcount,7:N0}");
		}
	}
}

}

public class ServiceTree_OrganizationCommonMetadata
{
	public Guid OrganizationId { get; set; }
	public string OrganizationName { get; set; }
	public string OrganizationLevel { get; set; }
	public string BusinessOwner { get; set; }
	
	public override string ToString()
	{
		return $"{OrganizationId}\t{OrganizationName}\t{OrganizationLevel}\t{BusinessOwner}";
	}
//}

