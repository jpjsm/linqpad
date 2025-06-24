<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	Foo foo = new Foo() { Sometext = "Hello World", Someint = 12 };
	Foo[] foos = new Foo[] {
		new Foo() { Sometext = "Hello World 12", Someint = 12 }
		, new Foo() { Sometext = "Hello World 13", Someint = 13 }
		, new Foo() { Sometext = "Hello World 14", Someint = 14 }
		, new Foo() { Sometext = "Hello World 15", Someint = 15 }
		, new Foo() { Sometext = "Hello World 16", Someint = 16 }
	};
	string json = JsonConvert.SerializeObject(foo);
	
	dynamic newfoo = JsonConvert.DeserializeObject<Foo>(json);

	Console.WriteLine($"newfoo.Sometext: {newfoo.Sometext}");
	newfoo.Sometext = newfoo.Sometext.ToString().GetHashCode().ToString();

	Console.WriteLine($"Updated newfoo.Sometext: {newfoo.Sometext}");
	Console.WriteLine("The entire newfoo object:");
	Console.WriteLine(JsonConvert.SerializeObject(newfoo, Newtonsoft.Json.Formatting.None));
	
	Console.WriteLine("======================= About Arrays =======================");
	json = JsonConvert.SerializeObject(foos);

	Console.WriteLine($"Serialized array: {json}");
	dynamic newfoos = JsonConvert.DeserializeObject<List<Foo>>(json);

	Console.WriteLine($"Get array size from new dynamic object: {newfoos.Count}");

	Console.WriteLine($"newfoos[2].Sometext: {newfoos[2].Sometext}");
	Console.WriteLine($"newfoos[2].Someint: {newfoos[2].Someint}");
}

// You can define other methods, fields, classes and namespaces here
public class Foo
{
	public string Sometext {get;set;}
	public int Someint {get;set;}
}