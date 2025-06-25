<Query Kind="Statements">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
string reset_data_stmt = "REVOKE EXECUTE ON [dbo].[fn_diagramobjects] FROM PUBLIC; " +
"REVOKE EXECUTE ON [dbo].[sp_alterdiagram] FROM PUBLIC; " +
"REVOKE EXECUTE ON [dbo].[sp_creatediagram] FROM PUBLIC; " +
"REVOKE EXECUTE ON [dbo].[sp_dropdiagram] FROM PUBLIC; " +
"REVOKE EXECUTE ON [dbo].[sp_helpdiagramdefinition] FROM PUBLIC; " +
"REVOKE EXECUTE ON [dbo].[sp_helpdiagrams] FROM PUBLIC; " +
"REVOKE EXECUTE ON [dbo].[sp_renamediagram] FROM PUBLIC";
using (SqlConnection cnx = new SqlConnection(cnxstr))
{
	cnx.Open();
	SqlCommand cmd_reset_data = new SqlCommand(reset_data_stmt, cnx);
	cmd_reset_data.ExecuteNonQuery();
}

Console.WriteLine("Done");