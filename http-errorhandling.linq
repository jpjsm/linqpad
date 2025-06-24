<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
</Query>

string base_address = "http://40.64.66.25/";
UriBuilder pronosticoBackendUriB = new UriBuilder(base_address);
pronosticoBackendUriB.Path = "weatherforecast";

Console.WriteLine(pronosticoBackendUriB.Uri);

using var client = new HttpClient();
try
{
	var response = await client.GetAsync(pronosticoBackendUriB.Uri);
	Console.WriteLine($"Status: {response.StatusCode} code: {(int)response.StatusCode}");
	
	string json = response.Content.ReadAsStringAsync().Result;
	
	if (string.IsNullOrWhiteSpace(json))
	{
		throw new ApplicationException("No content in response from backend.");
	}
	
	Console.WriteLine(json);
	List<WeatherForecast> weatherForecasts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherForecast>>(json);

	foreach (WeatherForecast weatherForecast in weatherForecasts)
	{
		Console.WriteLine(weatherForecast);
	}
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
	throw;
}


Console.WriteLine("Done!");

}//
public class WeatherForecast
{
	public DateTime Date { get; set; }

	public int TemperatureC { get; set; }

	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

	public string Summary { get; set; }

	public override string ToString()
	{
		return $"{Date}, C°:${TemperatureC}, F°: ${TemperatureF}, Feeling: ${Summary}";
	}
