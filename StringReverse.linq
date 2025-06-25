<Query Kind="Program" />

void Main()
{
	string[] nombres = {
		"abcde", "adiascar", "marcela", "andres", "carolina", "juan pablo", "nhora", "doris", "martin", "monica", "federico", "daniela"
	};
	
	foreach(string nombre in nombres)
	{
		Console.WriteLine($"{nombre}  -->  {Reverse(nombre)}");
	}
}

// You can define other methods, fields, classes and namespaces here
private static string Reverse(string input)
{
	StringBuilder txt = new StringBuilder(input);
	
	int r = input.Length - 1;
	
	
	for(int i = 0; i < input.Length/2; i++)
	{
		char t = txt[i];
		txt[i] = txt[r-i];
		txt[r-i] = t;
	}
	
	return txt.ToString();
}