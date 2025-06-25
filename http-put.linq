<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

//string url = "https://localhost:5001/api/order/829d0df7-9b3b-4e1f-8d6c-9e7e09fb9a79";
string url = "https://localhost:5001/api/order/foo";
string data = "{\"ItemIds\":[\"1\", \"5\"], \"Currency\": \"USD\"}";
Console.WriteLine(data);
StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
using var client = new HttpClient();
var response = await client.PutAsync(url, content);
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
