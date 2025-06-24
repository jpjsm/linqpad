<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
</Query>

/*  =========== Writing Methods and Classes in LINQPad ====================
	https://albao.wordpress.com/2011/04/26/writing-methods-and-classes-in-linqpad/
*/



string url = "http://40.64.66.25/weatherforecast";
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-ChangeGuard-Canary", "Yes");
var response = await client.GetAsync(url);
string status = response.StatusCode.ToString();
int status_code = (int)response.StatusCode;

string json = response.Content.ReadAsStringAsync().Result;

Console.WriteLine(json);
List<WeatherDetail> weatherForecast = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherDetail>>(json);
string result = string.Join("\n\t", weatherForecast);
Console.WriteLine($"[{status_code}: {status}]:\n\t {result}");

} // Add here the closing curly bracket that's ommitted at the end
public class WeatherDetail
{
	public DateTime Date { get; set; }

	public int TemperatureC { get; set; }

	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

	public string Summary { get; set; }
	
	public override string ToString()
	{
		return $"{Date}, C°:${TemperatureC}, F°: ${TemperatureF}, Feeling: ${Summary}";
	}
//}  