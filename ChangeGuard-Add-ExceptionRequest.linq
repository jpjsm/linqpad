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
	DateTime exceptionDatetime = DateTime.UtcNow;
	InsertProactiveExceptionRequest(
		ExceptionRequestId: Guid.NewGuid(),
		RequestDate: exceptionDatetime,
		ExceptionBeginsOn: exceptionDatetime,
		ExceptionEndsOn: exceptionDatetime.AddDays(2),
		ExceptionRequestStatus: "Approved",
		LastUpdate: exceptionDatetime,
		ServiceSubscriptionRegions: new string[] {"A338DE01-A914-4BC4-98B9-5B83338676C0|FF920BD8-CF75-4453-AFE9-12EBC6B0E15B|westus", "A338DE01-A914-4BC4-98B9-5B83338676C0|FF920BD8-CF75-4453-AFE9-12EBC6B0E15B|eastus"},
		approvers: "jujofre@microsoft.com,juanpablo jofre|htyagi@microsoft.com,himanshu tyagi|chbatra@microsoft.com,Chanchal Kumar Batra"
	);
}

/************************ End of Main **************************************/
//
/************************ Method Definitions *******************************/
public bool InsertProactiveExceptionRequest(
	Guid ExceptionRequestId,
	DateTime RequestDate,
	DateTime ExceptionBeginsOn,
	DateTime ExceptionEndsOn,
	String ExceptionRequestStatus,
	DateTime LastUpdate,
	String[] ServiceSubscriptionRegions,
	String approvers = "pahluwalia@microsoft.com,prashant singh ahluwalia|naveend@microsoft.com,naveen duddi haribabu|jujofre@microsoft.com,juanpablo jofre|esokonof@microsoft.com,ese okonofua|htyagi@microsoft.com,himanshu tyagi|yuturchi@microsoft.com,yulia turchin|satyavel@microsoft.com,satya vel|angperez@microsoft.com,angel perez")
{
	string cnxstr = "Server=tcp:chggrd-api-sql-svr-ppe.database.windows.net,1433;Initial Catalog=chggrd-api-sql-db-ppe;Persist Security Info=False;User ID=chggrdadmin;Password=M2JhNTkzMDEtMTA4OC00ZGZkLTlkMDYtMzk4MGQxMThkNzM1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=300;";

	string insert_stmt = @$"
		INSERT INTO [ExceptionRequest].[ExceptionRequests]
		           ([ExceptionRequestId]
		           ,[RequestDate]
		           ,[RequestorName]
		           ,[RequestorEmail]
		           ,[BusinessJustification]
		           ,[ExceptionBeginsOn]
		           ,[ExceptionEndsOn]
		           ,[DeploymentOfOrigin]
		           ,[ExceptionRequestStatus]
		           ,[ApproverId]
		           ,[ApproverName]
		           ,[ApproverEmail]
		           ,[Title]
		           ,[LastUpdate]
		           ,[EventIds]
		           ,[Approvers])
		     VALUES
		           (@ExceptionRequestId
		           ,@RequestDate
		           ,'Test Value'
		           ,'test.value@example.com'
		           ,'This is a test business justification'
		           ,@ExceptionBeginsOn
		           ,@ExceptionEndsOn
		           ,null
		           ,@ExceptionRequestStatus
		           ,null
		           ,null
		           ,'jujofre@microsoft.com'
		           ,'This is a test title'
		           ,@LastUpdate
		           ,''
		           ,@approvers);
				   
		INSERT INTO [ExceptionRequest].[ExceptionRequestServiceSubscriptionRegions]
		           ([ExceptionRequestId]
		           ,[ServiceId]
		           ,[SubscriptionId]
		           ,[Region])
		     VALUES
		           __ServiceSubscriptionRegions__				   
	";

	try
	{
		using (SqlConnection cnx = new SqlConnection(cnxstr))
		{
			cnx.Open();
			string[] servicesubscriptionregion_strs = new string[ServiceSubscriptionRegions.Length];
			for (int i = 0; i < ServiceSubscriptionRegions.Length; i++)
			{
				string[] values = ServiceSubscriptionRegions[i].Split('|');
				servicesubscriptionregion_strs[i] = $"(@ExceptionRequestId, '{values[0]}', '{values[1]}','{values[2]}')";
			}

			string replacementvalue = $"{string.Join(", ", servicesubscriptionregion_strs)};";
			insert_stmt = insert_stmt.Replace("__ServiceSubscriptionRegions__", replacementvalue);

			SqlCommand insertIntoTable = new SqlCommand(insert_stmt, cnx);
			insertIntoTable.Parameters.Add("@ExceptionRequestId", SqlDbType.UniqueIdentifier);
			insertIntoTable.Parameters.Add("@RequestDate", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@ExceptionBeginsOn", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@ExceptionEndsOn", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@ExceptionRequestStatus", SqlDbType.NVarChar, 50);
			insertIntoTable.Parameters.Add("@LastUpdate", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@approvers", SqlDbType.NVarChar, 512);

			insertIntoTable.Parameters["@ExceptionRequestId"].SqlValue = ExceptionRequestId;
			insertIntoTable.Parameters["@RequestDate"].SqlValue = RequestDate;
			insertIntoTable.Parameters["@ExceptionBeginsOn"].SqlValue = ExceptionBeginsOn;
			insertIntoTable.Parameters["@ExceptionEndsOn"].SqlValue = ExceptionEndsOn;
			insertIntoTable.Parameters["@ExceptionRequestStatus"].SqlValue = ExceptionRequestStatus;
			insertIntoTable.Parameters["@LastUpdate"].SqlValue = LastUpdate;
			insertIntoTable.Parameters["@approvers"].SqlValue = approvers;

			insertIntoTable.ExecuteNonQuery();
			return true;
		}
	}
	catch (Exception ex)
	{

		throw;
	}

	return false;
}

