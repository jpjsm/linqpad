<Query Kind="Program" />

void Main()
{
	string[] words = {"In", "a", "far", "away", "galaxy"};

	Console.WriteLine("With implicit underlying default value of 0 (zero)\nAll things work as intended.");
	foreach (string word in words)
	{
		Enum.TryParse<Foo>(word, ignoreCase: true, out Foo myFooEnum);
		string filteredWord = Enum.GetName(typeof(Foo), myFooEnum);
		Console.WriteLine($"Word: {word} --> {filteredWord}");
	}

	Console.WriteLine("\n\n");
	Console.WriteLine("Without an implicit underlying default value of 0 (zero)\nNOT all things work as intended.");
	foreach (string word in words)
	{
		Enum.TryParse<Boo>(word, ignoreCase: true, out Boo myFooEnum);
		string filteredWord = Enum.GetName(typeof(Boo), myFooEnum);
		Console.WriteLine($"Word: {word} --> {filteredWord}");
	}

	Console.WriteLine("\n\n");
	Console.WriteLine("Without an implicit underlying default value of 0 (zero)\nNOT all things work as intended.\nThis code fixes the issue.");
	foreach (string word in words)
	{
		if(!Enum.TryParse<Boo>(word, ignoreCase: true, out Boo myFooEnum))
		{
			myFooEnum = Boo.Upon;
		}
		string filteredWord = Enum.GetName(typeof(Boo), myFooEnum);
		Console.WriteLine($"Word: {word} --> {filteredWord}");
	}
}

// You can define other methods, fields, classes and namespaces here

public enum Foo
{
	Default, // Implicit underlying value of 0 (zero)
	Once,
	Upon,
	In,
	Far
}

public enum Boo
{
    Default = 32,
	Once,
	Upon,
	In,
	Far
}