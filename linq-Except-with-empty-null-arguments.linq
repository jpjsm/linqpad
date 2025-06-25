<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{
	Guid[] Ids = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };
	Guid[] EmptyArray = new Guid[]{};
	Guid[] NullArray = null;
	try
	{
		Guid[] difference1 = Ids.Except(NullArray).ToArray();
	}
	catch (Exception ex)
	{		
		Console.WriteLine($"As expected argument to Except() cannot be null: '{ex.Message}'");
	}

	Guid[] difference2 = Ids.Except(EmptyArray).ToArray();

	ValidateServiceSubscriptions validate = new ValidateServiceSubscriptions();
	Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(validate)}");

	var foo = validate.Subscription2services.Select(s => s.ToLowerInvariant()).ToList();
	Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(foo)}");

}

// You can define other methods, fields, classes and namespaces here
public class ValidateServiceSubscriptions
{
	public bool IsValid { get; set; }
	public string Msg { get; set; }
	public bool IsServiceIdUpdated { get; set; }
	public Guid OriginalServiceId { get; set; }
	public Guid NewServiceId { get; set; }
	public List<string> Subscription2services { get; set; } = new List<string>();
}
