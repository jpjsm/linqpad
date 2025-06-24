<Query Kind="Statements">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
string reset_data_stmt = "delete [ChangeAssessment].[ServiceSubscriptionRegionRiskEstimations];\n" +
"delete [ChangeAssessment].[ChangeAssessmentEvaluations];\n" +
"delete [ExceptionRequest].[ExceptionRequestApproverNotes];\n" +
"delete [ExceptionRequest].[ExceptionRequestFormerApproverIds];\n" +
"delete [ExceptionRequest].[ExceptionRequestServiceSubscriptionRegions];\n" +
"delete [ExceptionRequest].[ExceptionRequestStatusChange];" +
"delete [ExceptionRequest].[ExceptionRequests];";
using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	cnx.Open();
	SqlCommand cmd_reset_data = new SqlCommand(reset_data_stmt, cnx);
	cmd_reset_data.ExecuteNonQuery();
}

Console.WriteLine("Done");