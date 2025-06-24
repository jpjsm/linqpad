<Query Kind="Program">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	DateTime eventStart;
	DateTime eventEnd;

	
	eventStart = new DateTime(2021, 05, 26, 00, 00, 00, DateTimeKind.Local);
	eventEnd =   new DateTime(2021, 05, 31, 23, 59, 59, DateTimeKind.Local);
	InsertEvent(Guid.NewGuid(), $"Non-traditional OrgId InScope 2021-05-26 ~ 31 - BRAZILSOUTH_PUBLIC, TAIWANN_PUBLIC, SPAINC_PUBLIC, CHILENC_PUBLIC", eventStart, eventEnd);
	Console.WriteLine("Done!");
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
           ,'BRAZILSOUTH_PUBLIC'),
           (@eventId
           ,'B0000000-0000-2000-0008-000000000002'
           ,'TAIWANN_PUBLIC'),
           (@eventId
           ,'B0000000-0000-2000-0008-000000000003'
           ,'SPAINC_PUBLIC'),
           (@eventId
           ,'B0000000-0000-2000-0008-000000000004'
           ,'CHILENC_PUBLIC');
		   
		   ;
		   
	INSERT INTO [EventsRetrieval].[InCCOAScopeOrgIds]
           ([EventId]
           ,[OrgId])
		VALUES
           (@eventId
             ,'be4ea164-8632-421f-88d7-39377e9af49c')
          ,(@eventId
             ,'62708d8d-c741-4236-ba76-286a71daca34');
		   
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
			insertIntoTable.Parameters["@eventstartdate"].SqlValue = eventstartdate.ToUniversalTime();
			insertIntoTable.Parameters["@eventenddate"].SqlValue = eventenddate.ToUniversalTime();

			insertIntoTable.ExecuteNonQuery();
			Console.WriteLine($"[Inserted]eventid: {eventid}, eventname: {eventname}, eventstartdate: {eventstartdate.ToUniversalTime().ToString("u")} ~ eventenddate: {eventenddate.ToUniversalTime().ToString("u")}");
			return true;
		}
	}
	catch (Exception ex)
	{

		throw;
	}

	return false;
}

