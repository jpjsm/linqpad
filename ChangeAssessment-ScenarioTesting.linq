<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>Microsoft.Identity.Client</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	/*  =========== Writing Methods and Classes in LINQPad ====================
		https://albao.wordpress.com/2011/04/26/writing-methods-and-classes-in-linqpad/
	*/
	

	bool tracing = false;
	
	if (tracing)
	{
		using (var client = new HttpClient())
		{
			string eventsretrievalurl = "http://localhost:30014/eventsretrieval/events?startDate=9999-01-01&endDate=9999-12-31";
			var response = client.GetAsync(eventsretrievalurl).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;

			string json = response.Content.ReadAsStringAsync().Result;

			Console.WriteLine($"{new string('=', 132)}\tQuery: {eventsretrievalurl}\n\n[{status_code}: {status}]\n\t {Newtonsoft.Json.Linq.JToken.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented)}\n\n");
		}
	}


	Ev2Deployment rollout = new Ev2Deployment(){
		Requestor = "Testing Requestor",
		RequestorEmail = "testing.requestor@example.com",
		ServiceId = new Guid("FFFFFFFF-0101-0101-0100-000000000000"),
		Subscriptions = new Subscription[] { new Subscription() { SubscriptionId = new Guid("FFFFFFFF-0101-0101-0101-000000000000"), Regions = new string[] {"*"}}}
	};
	
	string changeassessmenturl = "http://localhost:30048/changeassessment/ev2";

	// Scenario 1: 
	// -- Event Scenario:                 No event at the beginning of the deployment
	// -- Exception Source:               N/A
	// -- Exception Scenario:             N/A
	// -- Deployment Allowed to Continue: Yes

	
	using (var client = new HttpClient())
	{
		int expected_status_code = 200;

		rollout.RolloutId = Guid.NewGuid();
		rollout.DeploymentEstimatedStart = DateTime.UtcNow;
		rollout.DeploymentEstimatedEnd = rollout.DeploymentEstimatedStart.AddDays(2.0);

		StringContent payload = new StringContent(JsonConvert.SerializeObject(rollout), Encoding.UTF8, "application/json");

		var response = client.PostAsync(changeassessmenturl, payload).Result;
		string status = response.StatusCode.ToString();
		int status_code = (int)response.StatusCode;
		if (status_code == expected_status_code )
		{
			string json = response.Content.ReadAsStringAsync().Result;
			Ev2DeploymentAssessment results = JsonConvert.DeserializeObject<Ev2DeploymentAssessment>(json);

			string expected_url = string.Empty;

			if (!string.IsNullOrEmpty(results.ExceptionRequestUrl))
			{
				Console.WriteLine($"[Failed] Scenario 1: expected_url: '{results.ExceptionRequestUrl}' != (null, empty)");
			}
		}
		else
		{
			Console.WriteLine($"[Failed] Scenario 1: Expected status code '200' != {status_code} ");
		}

	}

	// Scenario 2: 
	// -- Event Scenario:                 Event1 during the deployment
	// -- Exception Source:               N/A
	// -- Exception Scenario:             No Exception for the duration of the event
	// -- Deployment Allowed to Continue: Pending (until the end of the event)

	DateTime now = DateTime.UtcNow;
	DateTime eventEnd = now.AddMinutes(10.0);
	InsertEvent(Guid.NewGuid(), $"ChangeAssessment-Testing-Scenario 02-{now.ToString("yyyyMMdd_HHmmss")}",now.AddMinutes(-1.0), eventEnd);
	bool continuetest = true;
	using (var client = new HttpClient())
	{
		int expected_status_code = 200;

		rollout.RolloutId = Guid.NewGuid();
		rollout.DeploymentEstimatedStart = DateTime.UtcNow;
		rollout.DeploymentEstimatedEnd = rollout.DeploymentEstimatedStart.AddDays(2.0);

		StringContent payload = new StringContent(JsonConvert.SerializeObject(rollout), Encoding.UTF8, "application/json");

		var response = client.PostAsync(changeassessmenturl, payload).Result;
		string status = response.StatusCode.ToString();
		int status_code = (int)response.StatusCode;
		if (status_code == expected_status_code)
		{
			string json = response.Content.ReadAsStringAsync().Result;
			Ev2DeploymentAssessment results = JsonConvert.DeserializeObject<Ev2DeploymentAssessment>(json);

			if (string.IsNullOrEmpty(results.ExceptionRequestUrl))
			{
				Console.WriteLine($"[Failed] Scenario 2, part 1 POST: expected_url should be different than (null, empty)");
				continuetest = false;
			}
		}
		else
		{
			Console.WriteLine($"[Failed] Scenario 2, part 1 POST: Expected status code '200' != {status_code} ");
			continuetest = false;
		}
	}
	
	if (continuetest)
	{
		// Let's check halfway through the event end
		TimeSpan sleepTime = (eventEnd - DateTime.Now) / 2;
		System.Threading.Thread.Sleep((int)sleepTime.TotalMilliseconds + 1);

		using (var client1 = new HttpClient())
		{
			int expected_status_code = 200;


			var response = client1.GetAsync(changeassessmenturl + $"/{rollout.RolloutId}").Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;
				Ev2DeploymentAssessment results = JsonConvert.DeserializeObject<Ev2DeploymentAssessment>(json);

				if (string.IsNullOrEmpty(results.ExceptionRequestUrl))
				{
					Console.WriteLine($"[Failed] Scenario 2, part 2 GET: expected_url should be defferent than (null, empty)");
					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 2, part 2 GET: Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}
	}

	if (continuetest)
	{
		// Let's check 5 seconds past the event end
		TimeSpan sleepTime = eventEnd.AddSeconds(5.0) - DateTime.Now;
		System.Threading.Thread.Sleep((int)sleepTime.TotalMilliseconds);

		using (var client1 = new HttpClient())
		{
			int expected_status_code = 200;


			var response = client1.GetAsync(changeassessmenturl + $"/{rollout.RolloutId}").Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;
				Ev2DeploymentAssessment results = JsonConvert.DeserializeObject<Ev2DeploymentAssessment>(json);

				if (!string.IsNullOrEmpty(results.ExceptionRequestUrl))
				{
					Console.WriteLine($"[Failed] Scenario 2, part 3 GET: expected_url should be either (null, empty)");
					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 2, part 3 GET: Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}
	}


	Console.WriteLine("Execution complete.");
}

/************************ End of Main **************************************/
//
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


/************************ Class Definitions ********************************/

public class Ev2Deployment
{
	public Guid RolloutId { get; set; }
	public string Requestor { get; set; }
	public string RequestorEmail { get; set; }
	public DateTime DeploymentEstimatedStart { get; set; }
	public DateTime DeploymentEstimatedEnd { get; set; }
	public Guid ServiceId { get; set; }

	public Subscription[] Subscriptions { get; set; }

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
	}
	public override string ToString()
	{
		return
			  $"RolloutId: {RolloutId}" +
			$"\nRequestor: {Requestor}" +
			$"\nRequestorEmail: {RequestorEmail}" +
			$"\nDeploymentEstimatedStart: {DeploymentEstimatedStart.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}" +
			$"\nDeploymentEstimatedEnd: {DeploymentEstimatedEnd.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}" +
			$"\nServiceId: {ServiceId}" +
			 "\nSubscriptions:\n\t" +
			 string.Join("\n\t", string.Join("\n\t", Subscriptions.Select(u => u.SubscriptionId.ToString() + "\n\t\tRegions:" + string.Join("\n\t\t", u.Regions))))
			;
	}
}

public class Subscription
{
	public Guid SubscriptionId { get; set; }
	public string[] Regions { get; set; } // ["*"] => means all regions
}

public class Ev2DeploymentAssessment
{
	public Guid RolloutId { get; internal set; }
	public Ev2DeploymentAssessmentDetail[] Ev2DeploymentAssessmentDetails { get; internal set; }
	public string ExceptionRequestUrl { get; internal set; }
}

public class Ev2DeploymentAssessmentDetail
{
	public Guid ServiceId { get; internal set; }
	public Guid SubscriptionId { get; internal set; }
	public string Region { get; internal set; }
	public bool AllowedToDeploy { get; internal set; }
	public Guid[] ExceptionsReviewed { get; internal set; }
}
