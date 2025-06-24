<Query Kind="Program" />

void Main()
{
	for (int i = 0; i < 25; i++)
	{
		Guid guid = Guid.NewGuid();
		string guidB64 = Guid2base64(guid);
		var guidbytes = Convert.FromBase64String(Convert.ToBase64String(guid.ToByteArray()));
		Guid bar = new Guid(guidbytes);
		Console.WriteLine($"guid: {guid} == bar: {bar} ? {guid == bar}");
		//Console.WriteLine($"{guid} {guidB64}  {guidB64.Length} <--> {Base64ToGuid(guidB64)}");
		//Console.WriteLine($"Base64: {"5eVEERjqgkGjxcNlKiwOMg=="} --> Guid {Base64ToGuid("5eVEERjqgkGjxcNlKiwOMg")}");
	}
}

// You can define other methods, fields, classes and namespaces here
private static string Guid2base64(Guid g)
{
	return Convert.ToBase64String(g.ToByteArray())
	              .Replace("/", "_")
				  .Replace("+", "-")
				  .Replace("=", ":");
}

private static string Base64ToGuid(string b)
{
    string foo = b.Replace("_","/").Replace("-","+").Replace(":","=");
	return $"{Convert.FromBase64String(foo)}";
}