<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

string common_url = "https://change-manager.fcm.test/exceptionrequestmanager/ExceptionRequest/fromdeployment";
string data_v1 = "{\"deploymentId\":\"798d2426-8e2b-4b66-a08e-70d150e646e7\",\"requestor\":\"Foo Bar\",\"requestorEmail\":\"foo.bar@baraboo.mail\",\"deploymentEstimatedStart\":\"2020-10-22T15:55:31.465Z\",\"deploymentEstimatedEnd\":\"2020-10-29T15:55:31.465Z\",\"serviceId\":\"a3000000-9000-0000-0000-000000000000\",\"subscriptions\":[{\"subscriptionId\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"regions\":[\"string\"]}]}";
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
