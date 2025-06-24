<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>Microsoft.SqlServer.Server</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Data.Sql</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Data.SqlTypes</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

void Main()
{
	DateTime now = DateTime.UtcNow;
	string scenarioMessage = $"ChangeAssessment-Testing-Scenario 05-{now.ToString("yyyyMMdd_HHmmss")}";
	
	string exceptionrequestbodystr = "{" +
		"  \"requestorName\": \"JuanPablo Jofre\"," +
		"  \"requestorEmail\": \"jujofre@microsoft.com\"," +
		"  \"businessJustification\": \"\"," +
		"  \"exceptionBeginsOn\": \"\"," +
		"  \"exceptionEndsOn\": \"\"," +
		"  \"services\": [" +
		"    {" +
		"      \"serviceId\": \"FFFFFFFF-0101-0101-0100-000000000000\"," +
		"      \"serviceName\": \"Testing Service Name 01\"," +
		"      \"subscriptions\": [" +
		"        {" +
		"          \"subscriptionId\": \"FFFFFFFF-0101-0101-0101-000000000000\"," +
		"          \"regions\": [" +
		"            \"*\"" +
		"          ]" +
		"        }," +
		"        {" +
		"          \"subscriptionId\": \"FFFFFFFF-0101-0101-0102-000000000000\"," +
		"          \"regions\": [" +
		"            \"*\"" +
		"          ]" +
		"        }" +
		"      ]" +
		"    }" +
		"  ]," +
		"  \"approvers\": [" +
		"    {" +
		"      \"approverId\": \"3fa85f64-c717-4562-03fc-2c963f66afa6\"," +
		"      \"approverName\": \"Prashant Singh Ahluwalia\"," +
		"      \"approverEmail\": \"pahluwalia@microsoft.com\"" +
		"    }," +
		"    {" +
		"      \"approverId\": \"3fa85f64-c717-4562-03fb-2c963f66afa6\"," +
		"      \"approverName\": \"Naveen Duddi Haribabu\"," +
		"      \"approverEmail\": \"naveend@microsoft.com\"" +
		"    }," +
		"    {" +
		"      \"approverId\": \"3fa85f64-c717-4562-03ec-2c963f66afa6\"," +
		"      \"approverName\": \"JuanPablo Jofre\"," +
		"      \"approverEmail\": \"jujofre@microsoft.com\"" +
		"    }," +
		"    {" +
		"      \"approverId\": \"3fa85f64-c717-4562-03fc-2c963f66afa6\"," +
		"      \"approverName\": \"Ese Okonofua\"," +
		"      \"approverEmail\": \"esokonof@microsoft.com\"" +
		"    }," +
		"  ]," +
		"  \"title\": \"\"," +
		"  \"eventId\": \"\"," +
		"  \"status\": \"\"" +
		"}";
		
	dynamic exceptionrequestbody = JsonConvert.DeserializeObject(exceptionrequestbodystr);

	DateTime exceptionBeginsOn = DateTime.UtcNow.AddSeconds(10);
	DateTime exceptionEndsOn = exceptionBeginsOn.AddHours(1);

	dynamic exceptionrequest = null;
	Guid originalExceptionRequestId = Guid.Empty;

	// Let's have an event
	DateTime eventEnd = exceptionBeginsOn;
	DateTime eventStart = exceptionEndsOn;
	Guid eventId = Guid.NewGuid();
	InsertEvent(eventId, scenarioMessage, eventStart, eventEnd);


	bool continuetest = true;
	
	// Starting: Scenario 5, Part1: Submit ExceptionRequest for Revision
	if (continuetest)
	{
		string exceptionrequesturl = "http://localhost:30072/exceptionrequest/preapproval";

		using (var client = new HttpClient())
		{
			Console.WriteLine("Starting: Scenario 5, Part1: Submit ExceptionRequest for Revision");
			int expected_status_code = 201;

			exceptionrequestbody.businessJustification = $"BusinessJustification: {scenarioMessage}";
			exceptionrequestbody.title = $"Title: {scenarioMessage}";
			exceptionrequestbody.status = "PendingApproval";
			exceptionrequestbody.exceptionBeginsOn = exceptionBeginsOn.ToString("O");
			exceptionrequestbody.exceptionEndsOn = exceptionEndsOn.ToString("O");
			exceptionrequestbody.eventId = eventId.ToString().ToLowerInvariant();


			string payloadstr = JsonConvert.SerializeObject(exceptionrequestbody, Newtonsoft.Json.Formatting.None);
			StringContent payload = new StringContent(payloadstr, Encoding.UTF8, "application/json");

			string url = exceptionrequesturl;
			var response = client.PostAsync(url, payload).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;

				exceptionrequest = JsonConvert.DeserializeObject(json);
				originalExceptionRequestId = (Guid)exceptionrequest.exceptionRequestId;
				Console.WriteLine($"originalExceptionRequestId: {originalExceptionRequestId}");
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 5, part 1 POST: Expected status code '{expected_status_code}' != {status_code} ");
				string json = response.Content.ReadAsStringAsync().Result;
				Console.WriteLine(json);
				continuetest = false;
			}
		}
	}

	//Starting: Scenario 5, Part2: Update ExceptionRequest generated
	if (continuetest)
	{
		Console.WriteLine("Starting: Scenario 5, Part2: Update ExceptionRequest generated");
		// retrieve updated exception request 
		string exceptionrequesturl = "http://localhost:30072/exceptionrequest/";

		using (var client = new HttpClient())
		{
			int expected_status_code = 200;


			string url = exceptionrequesturl + originalExceptionRequestId.ToString();
			var response = client.GetAsync(url).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;
				exceptionrequest = JsonConvert.DeserializeObject(json);
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 5, part 2 GET: Expected status code '{expected_status_code}' != {status_code} ");
				continuetest = false;
			}
		}
	}

	dynamic newexceptionrequest = null;
	Guid newExceptionRequestId = Guid.Empty;

    // Starting: Scenario 5, Part3: Submit ANOTHER New ExceptionRequest for Revision => should pick up previous
	if (continuetest)
	{
		string exceptionrequesturl = "http://localhost:30072/exceptionrequest/preapproval";

		using (var client = new HttpClient())
		{
			Console.WriteLine("Starting: Scenario 5, Part3: Submit ANOTHER New ExceptionRequest for Revision => should pick up previous");
			int expected_status_code = 200;

			exceptionrequestbody.businessJustification = $"BusinessJustification: {scenarioMessage}";
			exceptionrequestbody.title = $"Title: {scenarioMessage}";
			exceptionrequestbody.status = "PendingApproval";
			exceptionrequestbody.exceptionBeginsOn = exceptionBeginsOn.AddMinutes(2.0).ToString("O");
			exceptionrequestbody.exceptionEndsOn = exceptionEndsOn.ToString("O");
			exceptionrequestbody.eventId = eventId.ToString().ToLowerInvariant();


			string payloadstr = JsonConvert.SerializeObject(exceptionrequestbody, Newtonsoft.Json.Formatting.None);
			StringContent payload = new StringContent(payloadstr, Encoding.UTF8, "application/json");

			string url = exceptionrequesturl;
			var response = client.PostAsync(url, payload).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;

				newexceptionrequest = JsonConvert.DeserializeObject(json);
				newExceptionRequestId = (Guid)newexceptionrequest.exceptionRequestId;
				Console.WriteLine($"newExceptionRequestId: {newExceptionRequestId}");
				
				if (newExceptionRequestId != originalExceptionRequestId)
				{
					Console.WriteLine($"[Failed] Scenario 5, Part 3: POST: NewExceptionRequestID {newExceptionRequestId} != {originalExceptionRequestId} OriginalExceptionRequestID");

					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 5, Part 3: POST: Expected status code '{expected_status_code}' != {status_code} ");
				string json = response.Content.ReadAsStringAsync().Result;
				Console.WriteLine(json);
				continuetest = false;
			}
		}
	}

	Console.WriteLine($"Complete: {scenarioMessage}");
}

// You can define other methods, fields, classes and namespaces here
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
