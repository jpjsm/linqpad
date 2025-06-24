<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	string jsonFoo1 = "{\"FooName\":null,\"FooRequiredText\":\"other text\", \"PlayingSuit\":\"\"}";
	string jsonFoo2 = "{\"FooName\":\"\",\"FooRequiredText\":\"\", \"PlayingSuit\":\"Picas\"}";
	string jsonFoo3 = "{\"FooName\":\"null\",\"FooRequiredText\":null, \"PlayingSuit\":\"Clubs\"}";

	Foo foo = new Foo();
	Console.WriteLine($"Foo :{JsonConvert.SerializeObject(foo, Newtonsoft.Json.Formatting.None)}");

	Foo foo1 = JsonConvert.DeserializeObject<Foo>(jsonFoo1);
	Console.WriteLine($"Foo1:{JsonConvert.SerializeObject(foo1, Newtonsoft.Json.Formatting.None)}");

	Foo foo2 = JsonConvert.DeserializeObject<Foo>(jsonFoo2);
	Console.WriteLine($"Foo2:{JsonConvert.SerializeObject(foo2, Newtonsoft.Json.Formatting.None)}");

	Foo foo3 = JsonConvert.DeserializeObject<Foo>(jsonFoo3);
	Console.WriteLine($"Foo3:{JsonConvert.SerializeObject(foo3, Newtonsoft.Json.Formatting.None)}");

}

// You can define other methods, fields, classes and namespaces here
public enum BridgeSuits
{
	Clubs,
	Diamonds,
	Hearts,
	Spades,
	NoTrump
}

public class Foo
{
	private string required = "initial text";
	private BridgeSuits enumeratedValue = BridgeSuits.NoTrump;
	
	public string FooName {get;set;}
	public string FooRequiredText
	{
		get { return required; }
		set { 
			if (!string.IsNullOrWhiteSpace(value))
			{
				required = value;
			}
		}
	}
	
	public string PlayingSuit
	{
		get
		{
			return Enum.GetName(typeof(BridgeSuits), enumeratedValue);
		}
		
		set
		{
			if (!Enum.TryParse<BridgeSuits>(value, ignoreCase: true, out enumeratedValue))
			{
				enumeratedValue = BridgeSuits.NoTrump;
				Console.WriteLine($"Warning: an unexpected value of '{value}' was received, default value of '{Enum.GetName(typeof(BridgeSuits), enumeratedValue)}' was assigned instead.");
			}
		}
	}
}