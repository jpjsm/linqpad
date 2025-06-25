<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

string common_url = "https://localhost:5001/ChangeRiskEstimator";
string data_v1 = "{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"requestor\":\"string\",\"requestorEmail\":\"string\",\"deploymentEstimatedStart\":\"2020-09-29T20:04:42.948Z\",\"deploymentEstimatedEnd\":\"2020-09-29T20:04:42.948Z\",\"services\":[{\"serviceId\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"subscriptions\":[{\"subscriptionId\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"regions\":[\"string\"]}]}]}";
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
