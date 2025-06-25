<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
</Query>

string url = "http://20.51.81.54/ServicesInfo/service";
string data = "[ \"a1000000-1000-0000-0000-000000000000\", \"a1000000-2000-0000-0000-000000000000\" , \"a2000000-2000-0000-0000-000000000000\", \"a2000000-2000-0000-0000-000000000000\"  ]";
//string data = "{\"services\":[]}";
Console.WriteLine(data);
StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
using var client = new HttpClient();

var request = new HttpRequestMessage
{
	Method = HttpMethod.Get,
	RequestUri = new Uri(url),
	Content = content,
};

var response = await client.SendAsync(request);

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
