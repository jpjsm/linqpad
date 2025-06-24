<Query Kind="Statements">
  <Output>DataGrids</Output>
</Query>

string[] frutales = {"cerezo", "manzano", "PERAL", "p@lt\u03C9"};

var foo = frutales.SelectMany(f => new string[]{f.ToLowerInvariant(), f.ToUpperInvariant(), new string(f.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant()}).ToList();

foreach (string frutal in foo)
{
	Console.WriteLine(frutal);
}