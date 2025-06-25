<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
</Query>

void Main()
{
	/*  =========== Writing Methods and Classes in LINQPad ====================
		https://albao.wordpress.com/2011/04/26/writing-methods-and-classes-in-linqpad/
	*/



	string query = "http://localhost:30072/exceptionrequest/changeassessmentev2query?&ServiceId={0}&DeploymentDatetime={1:O}";


	List<(Guid ServiceId, DateTime DeploymentDatetime, string scenario, int expectedStatusCode)> tests = new List<(Guid ServiceId, DateTime DeploymentDatetime, string scenario, int expectedStatusCode)>
		{
			( new Guid("889ACFB9-923F-4E3F-9BF2-2A3F9D95FE4F"), new DateTime(2021,2,26,0,0,0,DateTimeKind.Utc), "Outside: left", 204 )
			, ( new Guid("889ACFB9-923F-4E3F-9BF2-2A3F9D95FE4F"), new DateTime(2021,3,13,0,0,0,DateTimeKind.Utc), "Outside: right", 204 )
			, ( new Guid("FFEEDDCC-923F-4E3F-9BF2-2A3F9D95FE4F"), new DateTime(2021,2,26,0,0,0,DateTimeKind.Utc), "Outside: left", 204 )
			, ( new Guid("FFEEDDCC-923F-4E3F-9BF2-2A3F9D95FE4F"), new DateTime(2021,3,13,0,0,0,DateTimeKind.Utc), "Outside: right", 204 )
			, ( new Guid("889ACFB9-923F-4E3F-9BF2-2A3F9D95FE4F"), new DateTime(2021,3,8,0,0,0,DateTimeKind.Utc), "Inside, Exception for service defined", 200 )
			, ( new Guid("FFEEDDCC-923F-4E3F-9BF2-2A3F9D95FE4F"), new DateTime(2021,3,8,0,0,0,DateTimeKind.Utc), "Inside, Exception for service NOT defined", 204 )
		};

	foreach ((Guid ServiceId, DateTime DeploymentDatetime, string scenario, int expectedStatusCode) element in tests)
	{
		using (var client = new HttpClient())
		{
			string url = string.Format(query, element.ServiceId, element.DeploymentDatetime);
			var response = client.GetAsync(url).Result;
			string status = response.StatusCode.ToString();
			int status_code = (int)response.StatusCode;

			string json = response.Content.ReadAsStringAsync().Result;

			string testStatus = element.expectedStatusCode == status_code ? "Succeed" : "Failed";
			Console.WriteLine($"{new string('=', 132)}\nTest Status: {testStatus}\tQuery: {url}\n\n[{status_code}: {status}]:{element.scenario}\n\t {json}\n\n");
		}
	}

	Console.WriteLine("Execution complete.");
}

