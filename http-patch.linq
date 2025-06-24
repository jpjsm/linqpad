<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

string url = "https://localhost:44344/api/order/786fb140-7168-4966-aa1b-d7545c3ad2bb/";
string data = "[{\"op\":\"replace\",\"path\":\"/Currency\", \"value\": \"DDK\"}]";
Console.WriteLine(data);
StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
using var client = new HttpClient();
var response = await client.PatchAsync(url, content);

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
