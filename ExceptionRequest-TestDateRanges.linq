<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
		/*  =========== Writing Methods and Classes in LINQPad ====================
			https://albao.wordpress.com/2011/04/26/writing-methods-and-classes-in-linqpad/
		*/
		
		
		
		string query = "http://localhost:30072/exceptionrequest?&RequestedStartFrom={0:O}&RequestedUpTo={1:O}";
	
	
		List<(DateTime start, DateTime end, string scenario)> tests = new List<(DateTime start, DateTime end, string scenario)>
		{
			( new DateTime(2021,2,26,0,0,0,DateTimeKind.Utc), new DateTime(2021,2,26,0,0,0,DateTimeKind.Utc), "Outside: left" ),
			( new DateTime(2021,3,10,0,0,0,DateTimeKind.Utc), new DateTime(2021,3,13,0,0,0,DateTimeKind.Utc), "Outside: right" ),
			( new DateTime(2021,3,1,0,0,0,DateTimeKind.Utc), new DateTime(2021,3,5,0,0,0,DateTimeKind.Utc), "Outside end: left" ),
			( new DateTime(2021,3,7,0,0,0,DateTimeKind.Utc), new DateTime(2021,3,11,0,0,0,DateTimeKind.Utc), "Outside end: right" ),
			( new DateTime(2021,3,1,0,0,0,DateTimeKind.Utc), new DateTime(2021,3,10,0,0,0,DateTimeKind.Utc), "Outside end: left and right" ),
			( new DateTime(2021,3,4,0,0,0,DateTimeKind.Utc), new DateTime(2021,3,8,0,0,0,DateTimeKind.Utc), "Within: smaller than exception" ),
			( new DateTime(2021,3,3,0,0,0,DateTimeKind.Utc), new DateTime(2021,3,9,0,0,0,DateTimeKind.Utc), "Within: full extent of exception" ),
		};
		
		foreach ((DateTime start, DateTime end, string scenario) element in tests)
		{
			using(var client = new HttpClient())
			{
				string url = string.Format(query, element.start, element.end);
				var response = client.GetAsync(url).Result;
				string status = response.StatusCode.ToString();
				int status_code = (int)response.StatusCode;
	
				string json = response.Content.ReadAsStringAsync().Result;

			Console.WriteLine($"{new string('=',132)}\nQuery: {url}\n\n[{status_code}: {status}]:{element.scenario}\n\t {json}\n\n");
			}
		}
	
		Console.WriteLine("Execution complete.");
}

