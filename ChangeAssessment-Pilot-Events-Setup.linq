<Query Kind="Program">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	DateTime eventStart;
	DateTime eventEnd;

/*	
	// Test Event 1
	eventStart = new DateTime(2021, 03, 15, 13, 10, 00,DateTimeKind.Local).ToUniversalTime();
	eventEnd =   new DateTime(2021, 03, 15, 13, 40, 00,DateTimeKind.Local).ToUniversalTime();
	InsertEvent(Guid.NewGuid(), $"Test Event 1A - Pilot 2021-03-11", eventStart, eventEnd);

	// Test Event 2
	eventStart = new DateTime(2021, 03, 15, 16, 40, 00, DateTimeKind.Local).ToUniversalTime();
	eventEnd =   new DateTime(2021, 03, 15, 17, 10, 00, DateTimeKind.Local).ToUniversalTime();
	InsertEvent(Guid.NewGuid(), $"Test Event 2A - Pilot 2021-03-11", eventStart, eventEnd);

*/
	// Test Event 3
	eventStart = new DateTime(2021, 03, 15, 16, 05, 00, DateTimeKind.Local).ToUniversalTime();
	eventEnd = new DateTime(2021, 03, 15, 16, 25, 00, DateTimeKind.Local).ToUniversalTime();
	InsertEvent(Guid.NewGuid(), $"Test Event 3A - Pilot 2021-03-15", eventStart, eventEnd);

	// Test Event 4
	eventStart = new DateTime(2021, 03, 15, 16, 35, 00, DateTimeKind.Local).ToUniversalTime();
	eventEnd = new DateTime(2021, 03, 15, 16, 55, 00, DateTimeKind.Local).ToUniversalTime();
	InsertEvent(Guid.NewGuid(), $"Test Event 4A - Pilot 2021-03-15", eventStart, eventEnd);

	// Test Event 5
	eventStart = new DateTime(2021, 03, 15, 17, 05, 00, DateTimeKind.Local).ToUniversalTime();
	eventEnd = new DateTime(2021, 03, 15, 17, 25, 00, DateTimeKind.Local).ToUniversalTime();
	InsertEvent(Guid.NewGuid(), $"Test Event 5A - Pilot 2021-03-15", eventStart, eventEnd);

}
/************************ Method Definitions *******************************/
public bool InsertEvent(Guid eventid, string eventname, DateTime eventstartdate, DateTime eventenddate)
{
	string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=600;";

	string insert_stmt = @$"
	INSERT INTO [EventsRetrieval].[Events]
           ([Id]
           ,[Name]
           ,[StartDate]
           ,[EndDate])
     VALUES
           (@eventid
           ,@eventname
           ,@eventstartdate
           ,@eventenddate);
	
	
	INSERT INTO [EventsRetrieval].[Regions]
           ([EventId]
           ,[Id]
           ,[Name])
     VALUES
           (@eventId
           ,'B0000000-0000-2000-0008-000000000000'
           ,'*');
	";

	try
	{
		using (SqlConnection cnx = new SqlConnection(cnxstr))
		{
			cnx.Open();

			SqlCommand insertIntoTable = new SqlCommand(insert_stmt, cnx);
			insertIntoTable.Parameters.Add("@eventid", SqlDbType.UniqueIdentifier);
			insertIntoTable.Parameters.Add("@eventname", SqlDbType.NVarChar, 256);
			insertIntoTable.Parameters.Add("@eventstartdate", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@eventenddate", SqlDbType.DateTime2, 7);

			insertIntoTable.Parameters["@eventid"].SqlValue = eventid;
			insertIntoTable.Parameters["@eventname"].SqlValue = eventname;
			insertIntoTable.Parameters["@eventstartdate"].SqlValue = eventstartdate;
			insertIntoTable.Parameters["@eventenddate"].SqlValue = eventenddate;

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

