<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	string common_url = "https://change-manager.fcm.test/changeassessment/ev2";
	string data_v1 = "{\"rolloutId\":\"892b3db1-6658-4b7c-a11e-2b598b642049\",\"requestor\":\"Johnny English\",\"requestorEmail\":\"johnny.english@baraboo.mail\",\"deploymentEstimatedStart\":\"2020-10-24T01:34:47.436Z\",\"deploymentEstimatedEnd\":\"2020-10-31T01:34:47.436Z\",\"cloudName\":\"AzurePublic\",\"serviceId\":\"a2000000-1000-0000-0000-000000000000\",\"subscriptions\":[{\"subscriptionId\":\"a2000000-1000-1000-0000-000000000000\",\"regions\":[\"*\"]},{\"subscriptionId\":\"a2000000-1000-2000-0000-000000000000\",\"regions\":[\"*\"]},{\"subscriptionId\":\"a2000000-1000-3000-0000-000000000000\",\"regions\":[\"*\"]}],\"eventsHappening\":[{\"eventId\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"eventName\":\"string\",\"eventBegins\":\"2020-10-20T01:34:47.436Z\",\"eventEnds\":\"2020-10-20T01:34:47.436Z\",\"regionsImpacted\":[{\"cloudName\":\"string\",\"regionName\":\"string\"}]}]}";
	StringContent content_v1 = new StringContent(data_v1, Encoding.UTF8, "application/json");
	using var client = new HttpClient();
	
	var response = await client.PostAsync(common_url, content_v1);
	
	string status = response.StatusCode.ToString();
	int status_code = (int)response.StatusCode;
	var headers = response.Headers.ToDictionary(k => k.Key.ToString(), d => string.Join(", ", d.Value));
	Console.WriteLine("Headers:");
	foreach (var kvp in headers)
	{
		Console.WriteLine($"    {kvp.Key}: {kvp.Value}");
	}
	var result = await response.Content.ReadAsStringAsync();
	Console.WriteLine($"[{status_code}: {status}]: {result}");
	
	
	Console.WriteLine("Done!!");
}

// You can define other methods, fields, classes and namespaces here
