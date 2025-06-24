<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

string insert_ServiceTree_OrganizationCommonMetadata = 
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

string insert_ServiceTree_OrganizationHierarchy =
"INSERT INTO [ServicesInfo].[ServiceTree_OrganizationHierarchy]" +
"           ([DivisionId]" +
"           ,[OrganizationId]" +
"           ,[ServiceGroupId]" +
"           ,[TeamGroupId]" +
"           ,[ServiceId]" +
"           ,[SubscriptionId]" +
"           ,[DivisionName]" +
"           ,[OrganizationName]" +
"           ,[ServiceGroupName]" +
"           ,[TeamGroupName]" +
"           ,[ServiceName]" +
"           ,[SubscriptionName])" +
"     VALUES" +
"           (@DivisionId" +
"           ,@OrganizationId" +
"           ,@ServiceGroupId" +
"           ,@TeamGroupId" +
"           ,@ServiceId" +
"           ,@SubscriptionId" +
"           ,@DivisionName" +
"           ,@OrganizationName" +
"           ,@ServiceGroupName" +
"           ,@TeamGroupName" +
"           ,@ServiceName" +
"           ,@SubscriptionName);";


string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

Guid test_Division_Id =     new Guid("00000000-1000-0000-0000-000000000000");
string test_Division_Name = "Test Division";


Guid test_Organization_Id = new Guid("00000000-1000-1000-0000-000000000000");
string test_Organization_Name = "Test Organization";

Guid test_ServiceGroup_Id = new Guid("00000000-1000-1000-1000-000000000000");
string test_ServiceGroup_Name = "Test ServiceGroup";

Console.WriteLine($"test_Division_Id:       {test_Division_Id}");
Console.WriteLine($"test_Organization_Id:   {test_Organization_Id}");
Console.WriteLine($"test_ServiceGroup_Id:   {test_ServiceGroup_Id}");

using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	cnx.Open();
	SqlCommand cmd_ServiceTree_OrganizationCommonMetadata = new SqlCommand(insert_ServiceTree_OrganizationCommonMetadata, cnx);
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters.Add("@OrganizationId", SqlDbType.UniqueIdentifier);
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters.Add("@OrganizationName", SqlDbType.NVarChar, 128);
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters.Add("@OrganizationLevel", SqlDbType.NVarChar, 128);
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters.Add("@BusinessOwner", SqlDbType.NVarChar, 256);
	
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters["@OrganizationId"].SqlValue = test_ServiceGroup_Id;
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters["@OrganizationName"].SqlValue = test_ServiceGroup_Name;
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters["@OrganizationLevel"].SqlValue = "ServiceGroup";
	cmd_ServiceTree_OrganizationCommonMetadata.Parameters["@BusinessOwner"].SqlValue = "heididuan;CHBATRA;APTHANKY;NAVEEND;ESOKONOF;BARAMAS;pahluwalia;jujofre";
	cmd_ServiceTree_OrganizationCommonMetadata.ExecuteNonQuery();

	SqlCommand cmd_insert_ServiceTree_OrganizationHierarchy = new SqlCommand(insert_ServiceTree_OrganizationHierarchy, cnx);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@DivisionId", SqlDbType.UniqueIdentifier);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@OrganizationId", SqlDbType.UniqueIdentifier);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@ServiceGroupId", SqlDbType.UniqueIdentifier);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@ServiceGroupId"].IsNullable = true;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@TeamGroupId", SqlDbType.UniqueIdentifier);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@TeamGroupId"].IsNullable = true;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@ServiceId", SqlDbType.UniqueIdentifier);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@SubscriptionId", SqlDbType.UniqueIdentifier);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@DivisionName", SqlDbType.NVarChar, 128);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@OrganizationName", SqlDbType.NVarChar, 128);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@ServiceGroupName", SqlDbType.NVarChar, 128);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@ServiceGroupName"].IsNullable = true;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@TeamGroupName", SqlDbType.NVarChar, 128);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@TeamGroupName"].IsNullable = true;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@ServiceName", SqlDbType.NVarChar, 128);
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters.Add("@SubscriptionName", SqlDbType.NVarChar, 128);

	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@DivisionId"].SqlValue = test_Division_Id;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@OrganizationId"].SqlValue = test_Organization_Id;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@ServiceGroupId"].SqlValue = test_ServiceGroup_Id;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@TeamGroupId"].SqlValue = DBNull.Value;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@DivisionName"].SqlValue = test_Division_Name;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@OrganizationName"].SqlValue = test_Organization_Name;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@ServiceGroupName"].SqlValue = test_ServiceGroup_Name;
	cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@TeamGroupName"].SqlValue = string.Empty;


	for (byte i = 0; i < 16; i++)
	{
		Guid test_Service_Id = new Guid(0, (short)0x1000, (short)0x1000, new byte[8] { 16, 0, (byte)(16 + i), 0, 0, 0, 0, 0 });
		cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@ServiceId"].SqlValue = test_Service_Id;
		cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@ServiceName"].SqlValue = $"Test Service {i,3}";

		Console.WriteLine($"test_Service_Id:        {test_Service_Id}");

		for (int j = 1; j <= 64; j++)
		{
			Guid test_Subscription_Id = new Guid(0, (short)0x1000, (short)0x1000, new byte[8] { 16, 0, (byte)(16 + i), 0, 0, 0, 0, (byte)j });
			cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@SubscriptionId"].SqlValue = test_Subscription_Id;
			cmd_insert_ServiceTree_OrganizationHierarchy.Parameters["@SubscriptionName"].SqlValue = $"Test Subscription {i,3}";

			Console.WriteLine($"test_Subscription_Id:   {test_Subscription_Id}");
			cmd_insert_ServiceTree_OrganizationHierarchy.ExecuteNonQuery();
			Console.WriteLine($"test_Subscription_Id:   {test_Subscription_Id}");
		}
	}
}
