<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	string insert_stmt = 
	"INSERT INTO [ServicesInfo].[ServiceTree_OrganizationHierarchy]" +
		"([DivisionId]" +
		",[OrganizationId]" +
		",[ServiceGroupId]" +
		",[TeamGroupId]" +
		",[ServiceId]" +
		",[SubscriptionId]" +
		",[DivisionName]" +
		",[OrganizationName]" +
		",[ServiceGroupName]" +
		",[TeamGroupName]" +
		",[ServiceName]" +
		",[SubscriptionName]" +
		")" + 
	"VALUES" +
		"(@DivisionId" +
		",@OrganizationId" +
		",@ServiceGroupId" +
		",@TeamGroupId" +
		",@ServiceId" +
		",@SubscriptionId" +
		",@DivisionName" +
		",@OrganizationName" +
		",@ServiceGroupName" +
		",@TeamGroupName" +
		",@ServiceName" +
		",@SubscriptionName" +
		");";
	
	string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
	
	int skip = 0; // last failure value
	
	using (SqlConnection cnx = new SqlConnection(cnxstr))
	{
		using (TextReader  reader = File.OpenText(@"C:\Users\jujofre\Desktop\DataStudio_ServiceTree_Hierarchy_Snapshot-FCM.csv"))
		{
			//reader.ReadLine();
			
			using (var csvfile = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
			{
				cnx.Open();
				if (skip == 0)
				{
					//SqlCommand drop_table = new SqlCommand("TRUNCATE TABLE ServiceTree_OrganizationHierarchy;", cnx);
				}
				
				SqlCommand cmd = new SqlCommand(insert_stmt, cnx);
				cmd.Parameters.Add("@DivisionId", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@OrganizationId", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ServiceGroupId", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@ServiceGroupId"].IsNullable = true;
				cmd.Parameters.Add("@TeamGroupId", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TeamGroupId"].IsNullable = true;
				cmd.Parameters.Add("@ServiceId", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@SubscriptionId", SqlDbType.UniqueIdentifier);
	
				cmd.Parameters.Add("@DivisionName", SqlDbType.NVarChar, 128);
				cmd.Parameters.Add("@OrganizationName", SqlDbType.NVarChar, 128);
				cmd.Parameters.Add("@ServiceGroupName", SqlDbType.NVarChar, 128);
				cmd.Parameters["@ServiceGroupName"].IsNullable = true;
				cmd.Parameters.Add("@TeamGroupName", SqlDbType.NVarChar, 128);
				cmd.Parameters["@TeamGroupName"].IsNullable = true;
				cmd.Parameters.Add("@ServiceName", SqlDbType.NVarChar, 128);
				cmd.Parameters.Add("@SubscriptionName", SqlDbType.NVarChar, 128);
	
				var lines = csvfile.GetRecords<ServiceTree_OrganizationHierarchy>();
	
				int rowcount = 0;
				foreach (var h in lines)
				{
					if (++rowcount < skip)
					{
						continue;
					}
					// h is the same instance as hierarchy.
					cmd.Parameters["@DivisionId"].SqlValue = h.DivisionId;
					
					cmd.Parameters["@OrganizationId"].SqlValue = h.OrganizationId;
					
					if (h.ServiceGroupId.HasValue)
					{
						cmd.Parameters["@ServiceGroupId"].SqlValue = h.ServiceGroupId.Value;
					}
					else
					{
						cmd.Parameters["@ServiceGroupId"].SqlValue = DBNull.Value;
					}
					
					if (h.TeamGroupId.HasValue)
					{
						cmd.Parameters["@TeamGroupId"].SqlValue = h.TeamGroupId;
					}
					else
					{
						cmd.Parameters["@TeamGroupId"].SqlValue = DBNull.Value;
					}
					
					cmd.Parameters["@ServiceId"].SqlValue = h.ServiceId;
					cmd.Parameters["@SubscriptionId"].SqlValue = h.SubscriptionId;
	
					cmd.Parameters["@DivisionName"].SqlValue = h.DivisionName;
					cmd.Parameters["@OrganizationName"].SqlValue = h.OrganizationName;
					cmd.Parameters["@ServiceGroupName"].SqlValue = h.ServiceGroupName;
					cmd.Parameters["@TeamGroupName"].SqlValue = h.TeamGroupName;
					cmd.Parameters["@ServiceName"].SqlValue = h.ServiceName;
					cmd.Parameters["@SubscriptionName"].SqlValue = h.SubscriptionName;
					try
					{
						var _ = cmd.ExecuteNonQuery();
					}
					catch(SqlException sqlx){
						if (!sqlx.Message.StartsWith("Violation of PRIMARY KEY constraint"))
						{
							Console.WriteLine($"{sqlx.Message}\n{sqlx.Data}");
							Console.WriteLine($"{cmd.CommandText}");
							Console.WriteLine($"{h}");
							Console.WriteLine($"Row number: {rowcount}");
							throw;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"{ex.Message}\n{ex.Data}");
						Console.WriteLine($"{cmd.CommandText}");
						Console.WriteLine($"{h}");
						Console.WriteLine($"Row number: {rowcount}");
						throw;
					}
				}
	
				Console.WriteLine($"Total rows: {rowcount}");
			}
		}
	}
	
	Console.WriteLine("Done!");
}

public class ServiceTree_OrganizationHierarchy
{
	public Guid? DivisionId { get; set; }
	public Guid? OrganizationId { get; set; }
	public Guid? ServiceGroupId { get; set; }
	public Guid? TeamGroupId { get; set; }
	public Guid? ServiceId { get; set; }
	public Guid? SubscriptionId { get; set; }
	public string DivisionName { get; set; }
	public string OrganizationName { get; set; }
	public string ServiceGroupName { get; set; }
	public string TeamGroupName { get; set; }
	public string ServiceName { get; set; }
	public string SubscriptionName { get; set; }

	public override string ToString()
	{
		return $"{DivisionId}\t{OrganizationId}\t{ServiceGroupId}\t{TeamGroupId}\t{ServiceId}\t{SubscriptionId}";
	}
}	
//}
