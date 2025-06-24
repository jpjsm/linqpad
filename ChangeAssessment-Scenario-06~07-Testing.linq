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


	Ev2Deployment rollout = new Ev2Deployment()
	{
		Requestor = "Testing Requestor",
		RequestorEmail = "testing.requestor@example.com",
		ServiceId = new Guid("FFFFFFFF-0101-0101-0100-000000000000"),
		Subscriptions = new Subscription[] {
			new Subscription() { SubscriptionId = new Guid("FFFFFFFF-0101-0101-0101-000000000000"), Regions = new string[] {"*"}},
			new Subscription() { SubscriptionId = new Guid("FFFFFFFF-0101-0101-0102-000000000000"), Regions = new string[] {"*"}}
		}
	};

	string changeassessmenturl = "http://localhost:30048/changeassessment/ev2";

	// Scenario 6: 
	// -- Event Scenario:                 Event during the deployment
	// -- Exception Source:               Proactive
	// -- Exception Scenario:             DECLINED
	// -- Deployment Allowed to Continue: NO

	// Let's have an event
	DateTime now = DateTime.UtcNow;
	DateTime eventStart = now.AddMinutes(-1.0);
	DateTime eventEnd = now.AddMinutes(1.0);
	InsertEvent(Guid.NewGuid(), $"ChangeAssessment-Testing-Scenario 06~07-{now.ToString("yyyyMMdd_HHmmss")}", eventStart, eventEnd);

	// Let's have a proactive DECLINED exception to cover the event
	Guid exceptionRequestId = Guid.NewGuid();
	DateTime requestDate = now;
	InsertExceptionRequest(exceptionRequestId, requestDate, eventStart, eventEnd, "Declined", requestDate);

	bool continuetest = true;
	using (var client = new HttpClient())
	{
		Console.WriteLine("Starting: Scenario 6, Part 1");
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

			if (!string.IsNullOrEmpty(results.ExceptionRequestUrl))
			{
				Console.WriteLine($"[Failed] Scenario 6, part 1 POST: expected_url should be (null, empty)");
				Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
				continuetest = false;
			}

			if (!results.Ev2DeploymentAssessmentDetails.All(d => !d.AllowedToDeploy))
			{
				Console.WriteLine($"[Failed] Scenario 6, part 1 POST: AllowedToDeploy should be all false");
				Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
				continuetest = false;
			}

			if (!results.Ev2DeploymentAssessmentDetails.All(d => d.ExceptionsReviewed.Length > 0))
			{
				Console.WriteLine($"[Failed] Scenario 6, part 1 POST: All details should have an exception reviewed");
				Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
				continuetest = false;
			}
		}
		else
		{
			Console.WriteLine($"[Failed] Scenario 6, part 1 POST: Expected status code '200' != {status_code} ");
			continuetest = false;
		}
	}

	if (continuetest)
	{
		// Scenario 7: 
		// -- Event Scenario:                 Second deployment during the same event 
		// -- Exception Source:               N/A
		// -- Exception Scenario:             (Proactive Exception declined from previous scenario)
		// -- Deployment Allowed to Continue: No

		// Let's start another deployment half way through the event
		TimeSpan sleepTime = new TimeSpan((eventEnd - DateTime.UtcNow).Ticks / 2);
		Console.WriteLine($"Starting: Scenario 7, Part 1. Sleeping for {sleepTime.TotalMinutes} minutes");
		System.Threading.Thread.Sleep((int)sleepTime.TotalMilliseconds + 1);

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

				if (!string.IsNullOrEmpty(results.ExceptionRequestUrl))
				{
					Console.WriteLine($"[Failed] Scenario 7, part 1 POST: expected_url should be (null, empty)");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}

				if (!results.Ev2DeploymentAssessmentDetails.All(d => !d.AllowedToDeploy))
				{
					Console.WriteLine($"[Failed] Scenario 7, part 1 POST: AllowedToDeploy should be all false");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}

				if (!results.Ev2DeploymentAssessmentDetails.All(d => d.ExceptionsReviewed.Length > 0))
				{
					Console.WriteLine($"[Failed] Scenario 7, part 1 POST: All details should have an exception reviewed");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 7, part 1 POST: Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}

	}

	if (DateTime.UtcNow < eventEnd)
	{
		int waitmilliseconds = Convert.ToInt32((eventEnd - DateTime.UtcNow).TotalMilliseconds);
		Console.WriteLine($"Waiting {waitmilliseconds:N0} milliseconds until the created event expires.");
		System.Threading.Thread.Sleep(waitmilliseconds);
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

public bool InsertExceptionRequest(
	Guid ExceptionRequestId,
	DateTime RequestDate,
	DateTime ExceptionBeginsOn,
	DateTime ExceptionEndsOn,
	String ExceptionRequestStatus,
	DateTime LastUpdate)
{
	string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=600;";

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
		           ,'pahluwalia@microsoft.com,prashant singh ahluwalia|naveend@microsoft.com,naveen duddi haribabu|jujofre@microsoft.com,juanpablo jofre|esokonof@microsoft.com,ese okonofua|htyagi@microsoft.com,himanshu tyagi|yuturchi@microsoft.com,yulia turchin|satyavel@microsoft.com,satya vel|angperez@microsoft.com,angel perez');
				   
		INSERT INTO [ExceptionRequest].[ExceptionRequestServiceSubscriptionRegions]
		           ([ExceptionRequestId]
		           ,[ServiceId]
		           ,[SubscriptionId]
		           ,[Region])
		     VALUES
		           (@ExceptionRequestId
		           ,'FFFFFFFF-0101-0101-0100-000000000000'
		           ,'FFFFFFFF-0101-0101-0101-000000000000'
		           ,'*'),				   
		           (@ExceptionRequestId
		           ,'FFFFFFFF-0101-0101-0100-000000000000'
		           ,'FFFFFFFF-0101-0101-0102-000000000000'
		           ,'*')				   
	";

	try
	{
		using (SqlConnection cnx = new SqlConnection(cnxstr))
		{
			cnx.Open();

			SqlCommand insertIntoTable = new SqlCommand(insert_stmt, cnx);
			insertIntoTable.Parameters.Add("@ExceptionRequestId", SqlDbType.UniqueIdentifier);
			insertIntoTable.Parameters.Add("@RequestDate", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@ExceptionBeginsOn", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@ExceptionEndsOn", SqlDbType.DateTime2, 7);
			insertIntoTable.Parameters.Add("@ExceptionRequestStatus", SqlDbType.NVarChar, 50);
			insertIntoTable.Parameters.Add("@LastUpdate", SqlDbType.DateTime2, 7);

			insertIntoTable.Parameters["@ExceptionRequestId"].SqlValue = ExceptionRequestId;
			insertIntoTable.Parameters["@RequestDate"].SqlValue = RequestDate;
			insertIntoTable.Parameters["@ExceptionBeginsOn"].SqlValue = ExceptionBeginsOn;
			insertIntoTable.Parameters["@ExceptionEndsOn"].SqlValue = ExceptionEndsOn;
			insertIntoTable.Parameters["@ExceptionRequestStatus"].SqlValue = ExceptionRequestStatus;
			insertIntoTable.Parameters["@LastUpdate"].SqlValue = LastUpdate;

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
	[JsonProperty("rolloutId")]
	public Guid RolloutId { get; internal set; }
	[JsonProperty("ev2DeploymentAssessmentDetails")]
	public Ev2DeploymentAssessmentDetail[] Ev2DeploymentAssessmentDetails { get; internal set; }
	[JsonProperty("exceptionRequestUrl")]
	public string ExceptionRequestUrl { get; internal set; }
}

public class Ev2DeploymentAssessmentDetail
{
	[JsonProperty("serviceId")]
	public Guid ServiceId { get; internal set; }
	[JsonProperty("subscriptionId")]
	public Guid SubscriptionId { get; internal set; }
	[JsonProperty("region")]
	public string Region { get; internal set; }
	[JsonProperty("allowedToDeploy")]
	public bool AllowedToDeploy { get; internal set; }
	[JsonProperty("exceptionsReviewed")]
	public Guid[] ExceptionsReviewed { get; internal set; }
}
