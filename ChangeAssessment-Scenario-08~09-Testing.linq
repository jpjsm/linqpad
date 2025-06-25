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
	int startof = -1;
	string deploymentUrlId = string.Empty;

	// Scenario 8: 
	// -- Event Scenario:                 Event during the deployment
	// -- Exception Source:               Reactive
	// -- Exception Scenario:             APPROVED
	// -- Deployment Allowed to Continue: Pending while the exception is being created; yes, after the exception is approved.

	// Let's have an event
	DateTime now = DateTime.UtcNow;
	DateTime eventStart = now.AddMinutes(-1.0);
	DateTime eventEnd = now.AddMinutes(1.0);
	InsertEvent(Guid.NewGuid(), $"ChangeAssessment-Testing-Scenario 08~09-{now.ToString("yyyyMMdd_HHmmss")}", eventStart, eventEnd);

	Ev2DeploymentAssessment deploymentAssessment = null;
	bool continuetest = true;
	using (var client = new HttpClient())
	{
		Console.WriteLine("Starting: Scenario 8, Part 1: Creating Deployment");
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

			deploymentAssessment = JsonConvert.DeserializeObject<Ev2DeploymentAssessment>(json);
			startof = deploymentAssessment.ExceptionRequestUrl.LastIndexOf('/') + 1;
			deploymentUrlId = deploymentAssessment.ExceptionRequestUrl.Substring(startof);

			if (string.IsNullOrEmpty(deploymentAssessment.ExceptionRequestUrl))
			{
				Console.WriteLine($"[Failed] Scenario 8, part 1 POST: expected_url should be different than (null, empty)");
				Console.WriteLine(JsonConvert.SerializeObject(deploymentAssessment, Newtonsoft.Json.Formatting.Indented));
				continuetest = false;
			}

			if (!deploymentAssessment.Ev2DeploymentAssessmentDetails.Any(d => !d.AllowedToDeploy))
			{
				Console.WriteLine($"[Failed] Scenario 8, part 1 POST: At least one AllowedToDeploy should be false");
				Console.WriteLine(JsonConvert.SerializeObject(deploymentAssessment, Newtonsoft.Json.Formatting.Indented));
				continuetest = false;
			}

			if (!deploymentAssessment.Ev2DeploymentAssessmentDetails.Any(d => d.ExceptionsReviewed == null || d.ExceptionsReviewed.Length == 0))
			{
				Console.WriteLine($"[Failed] Scenario 8, part 1 POST: At least one detail should not have an exception reviewed");
				Console.WriteLine(JsonConvert.SerializeObject(deploymentAssessment, Newtonsoft.Json.Formatting.Indented));
				continuetest = false;
			}
		}
		else
		{
			Console.WriteLine($"[Failed] Scenario 8, part 1 POST: Expected status code '200' != {status_code} ");
			continuetest = false;
		}
	}

	dynamic changeAssessmentEvaluation = null;
	if (continuetest)
	{
		using (var client = new HttpClient())
		{
			//Get the ChageAssessment created to validate it
			int expected_status_code = 200;


			var response = client.GetAsync(changeassessmenturl + "/?&deploymentUrl=" + deploymentUrlId).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;

				changeAssessmentEvaluation = JsonConvert.DeserializeObject(json);

			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 8, part 1A GET (retrieving the ChageAssessment created to validate it): Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}
	}

	dynamic exceptionrequest = null;

	if (continuetest)
	{
		string exceptionrequestfromdeploymenturl = "http://localhost:30072/exceptionrequest/fromdeployment/?&deploymentUrlId=";

		using (var client = new HttpClient())
		{
			Console.WriteLine("Starting: Scenario 8, Part 2: Create ExceptionRequest from DeploymentUrl");
			Console.WriteLine($"                     ... Sleeping for 15 seconds");
			System.Threading.Thread.Sleep(15000);

			int expected_status_code = 201;
			
			startof = deploymentAssessment.ExceptionRequestUrl.LastIndexOf('/') + 1;
			deploymentUrlId = deploymentAssessment.ExceptionRequestUrl.Substring(startof);

			
			StringContent payload = new StringContent(string.Empty, Encoding.UTF8, "application/json");
			
			var response = client.PostAsync(exceptionrequestfromdeploymenturl + deploymentUrlId, payload).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;

				exceptionrequest = JsonConvert.DeserializeObject(json);
				Guid exceptionrequestDeploymentOfOrigin = (Guid) exceptionrequest.deploymentOfOrigin;
				if (exceptionrequestDeploymentOfOrigin != deploymentAssessment.RolloutId)
				{
					Console.WriteLine($"[Failed] Scenario 8, part 2 POST: DeploymentOfOrgin should match RolloutId");
					Console.WriteLine(JsonConvert.SerializeObject(exceptionrequest, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 8, part 2 POST: Expected status code '200' != {status_code} ");
				string json = response.Content.ReadAsStringAsync().Result;
				Console.WriteLine(json);
				continuetest = false;
			}
		}
	}

	if (continuetest)
	{
		// Get EarliestEventStart and LatestEventEnd, these dates should match the event dates
		DateTime EarliestEventStart = (DateTime)changeAssessmentEvaluation.earliestEventStart;
		DateTime LatestEventEnd = (DateTime)changeAssessmentEvaluation.latestEventEnd;

		// Validate exception request created from URL takes he same timespan of the event associated with
		DateTime ExceptionBeginsOn = (DateTime)exceptionrequest.exceptionBeginsOn;
		DateTime ExceptionEndsOn = (DateTime)exceptionrequest.exceptionEndsOn;

		int BeginDifferenceMilliseconds = System.Convert.ToInt32(System.Math.Abs((ExceptionBeginsOn - EarliestEventStart).TotalMilliseconds));
		int EndDifferenceMilliseconds = System.Convert.ToInt32(System.Math.Abs((ExceptionEndsOn - LatestEventEnd).TotalMilliseconds));
		if (BeginDifferenceMilliseconds > 500 || EndDifferenceMilliseconds > 500)
		{
			Console.WriteLine("[Failed] Exception duration doesn't match event duration.");
			Console.WriteLine($"EarliestEventStart: {EarliestEventStart:O}, ExceptionBeginsOn: {ExceptionBeginsOn:O}. Beginning difference: {System.Math.Abs((ExceptionBeginsOn - EarliestEventStart).TotalMilliseconds)}.");
			Console.WriteLine($"LatestEventEnd: {LatestEventEnd:O}, ExceptionEndsOn: {ExceptionEndsOn:O}. Ending difference: {System.Math.Abs((ExceptionEndsOn - eventEnd).TotalMilliseconds)}.");
		}
	}

	if (continuetest)
	{
		// Let's check deployment is still pending
		Console.WriteLine($"Starting: Scenario 8, Part 3. Checking deployment is still pending while the exception gets processed");

		using (var client1 = new HttpClient())
		{
			int expected_status_code = 200;

			var response = client1.GetAsync(changeassessmenturl + $"/{rollout.RolloutId}").Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code == expected_status_code)
			{
				string json = response.Content.ReadAsStringAsync().Result;

				dynamic results = JsonConvert.DeserializeObject(json);

				if (string.IsNullOrEmpty(results.exceptionRequestUrl.ToString()))
				{
					Console.WriteLine($"[Failed] Scenario 8, part 3 GET: expected_url should be different than (null, empty)");
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
		string exceptionrequesturl = "http://localhost:30072/exceptionrequest/";

		using (var client = new HttpClient())
		{
			Console.WriteLine("Starting: Scenario 8, Part4: Submit ExceptionRequest for Revision");
			int expected_status_code = 200;

			exceptionrequest.businessJustification = $"BusinessJustification: ChangeAssessment-Testing-Scenario 08~09-{now.ToString("yyyyMMdd_HHmmss")}";
			exceptionrequest.title = $"Title: ChangeAssessment-Testing-Scenario 08~09-{now.ToString("yyyyMMdd_HHmmss")}";
			exceptionrequest.status = "PendingApproval";

			string exceptionrequeststr = JsonConvert.SerializeObject(exceptionrequest, Newtonsoft.Json.Formatting.None);
			StringContent payload = new StringContent(exceptionrequeststr, Encoding.UTF8, "application/json");

			string url = exceptionrequesturl + exceptionrequest.exceptionRequestId.ToString();
			var response = client.PutAsync(url, payload).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code != expected_status_code)
			{
				Console.WriteLine($"[Failed] Scenario 8, part 4 PUT: Expected status code '200' != {status_code} ");
				string json = response.Content.ReadAsStringAsync().Result;
				Console.WriteLine(json);
				continuetest = false;
			}
		}
	}
	
	if (continuetest)
	{
		// retrieve updated exception request to update it again
		string exceptionrequesturl = "http://localhost:30072/exceptionrequest/";

		using (var client = new HttpClient())
		{
			int expected_status_code = 200;


			string url = exceptionrequesturl + exceptionrequest.exceptionRequestId.ToString();
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
				Console.WriteLine($"[Failed] Scenario 8, part 4B GET: Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}
	}

	if (continuetest)
	{
		string exceptionrequesturl = "http://localhost:30072/exceptionrequest/";

		using (var client = new HttpClient())
		{
			Console.WriteLine("Starting: Scenario 8, Part 5: Approve ExceptionRequest");
			int expected_status_code = 200;

			exceptionrequest.approvedBy = "jujofre@microsoft.com";
			exceptionrequest.status = "Approved";

			string exceptionrequeststr = JsonConvert.SerializeObject(exceptionrequest, Newtonsoft.Json.Formatting.None);
			StringContent payload = new StringContent(exceptionrequeststr, Encoding.UTF8, "application/json");

			string url = exceptionrequesturl + exceptionrequest.exceptionRequestId.ToString();
			var response = client.PutAsync(url, payload).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;
			if (status_code != expected_status_code)
			{
				Console.WriteLine($"[Failed] Scenario 8, part 5 PUT: Expected status code '200' != {status_code} ");
				string json = response.Content.ReadAsStringAsync().Result;
				Console.WriteLine(json);
				continuetest = false;
			}
		}
	}

	if (continuetest)
	{
		// Let's check deployment resumed once exception was approved
		Console.WriteLine($"Starting: Scenario 8, Part 6. Checking deployment resumed once exception was approved");

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
					Console.WriteLine($"[Failed] Scenario 3, part 1 POST: expected_url should be (null, empty)");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}

				if (!results.Ev2DeploymentAssessmentDetails.All(d => d.AllowedToDeploy))
				{
					Console.WriteLine($"[Failed] Scenario 8, part 6 GET: AllowedToDeploy should be all true");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}

				if (!results.Ev2DeploymentAssessmentDetails.All(d => d.ExceptionsReviewed.Length > 0))
				{
					Console.WriteLine($"[Failed] Scenario 8, part 6 GET: All details should have an exception reviewed");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 8, part 6 GET: Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}
	}

	if (continuetest)
	{
		// Scenario 9: 
		// -- Event Scenario:                 Second deployment during the same event 
		// -- Exception Source:               N/A
		// -- Exception Scenario:             (reactive Exception approved from previous scenario)
		// -- Deployment Allowed to Continue: No

		// Let's start another deployment half way through the event
		Console.WriteLine($"Starting: Scenario 9, Part 1. ");

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
					Console.WriteLine($"[Failed] Scenario 9, part 1 POST: expected_url should be (null, empty)");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}

				if (!results.Ev2DeploymentAssessmentDetails.All(d => d.AllowedToDeploy))
				{
					Console.WriteLine($"[Failed] Scenario 9, part 1 POST: AllowedToDeploy should be all true");
					Console.WriteLine(JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
					continuetest = false;
				}
			}
			else
			{
				Console.WriteLine($"[Failed] Scenario 9, part 1 POST: Expected status code '200' != {status_code} ");
				continuetest = false;
			}
		}

	}

if (DateTime.UtcNow < eventEnd)
{
		int waitmilliseconds = Convert.ToInt32((eventEnd - DateTime.UtcNow).TotalMilliseconds);
		Console.WriteLine($"Waiting {waitmilliseconds:N0} milliseconds until the event created expires.");
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

public bool _InsertExceptionRequest(
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
