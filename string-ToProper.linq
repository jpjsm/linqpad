<Query Kind="Program">
  <Output>DataGrids</Output>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	string[] names = new string[] {
		"juan                        pablo " + "\u2028\u2029" + " jofre",
		" \u000A\u000B\u000C\u000Dlos árboles " + "\u000A\u000B\u000C\u000D" + "  mueren de\u000Apie",
		"NASA",
		"VBScripting",
		"o'hara",
		"d'arte'"
		};

	foreach (string name in names)
	{
		Console.WriteLine($"{name} --> {ToProper(name)}");
	}
}
// You can define other methods, fields, classes and namespaces here
public static readonly char[] WhiteSpaceChars = new char[]{
    // See this link to get understanding of what's considered a white-space
	// characters in .Net 5.0.
	// https://docs.microsoft.com/en-us/dotnet/api/system.char.iswhitespace?view=net-5.0
	 '\u0009'
	, '\u000A'
	, '\u000B'
	, '\u000C'
	, '\u000D'
	, '\u0085'
	
	, '\u2028'
	, '\u2029'

	, '\u0020'
	, '\u00A0'
	, '\u1680'
	, '\u2000'
	, '\u2001'
	, '\u2002'
	, '\u2003'
	, '\u2004'
	, '\u2005'
	, '\u2006'
	, '\u2007'
	, '\u2008'
	, '\u2009'
	, '\u200A'
	, '\u202F'
	, '\u205F'
	, '\u3000'
};

public static string ToProper(string text)
{
	if (string.IsNullOrWhiteSpace(text))
	{
		return text;
	}

	text = text.Trim();
	text = Regex.Replace(text, $"({string.Join('|', WhiteSpaceChars)})+", " ", RegexOptions.Singleline);
    
	
	CultureInfo culture_info = CultureInfo.InvariantCulture;
	TextInfo text_info = culture_info.TextInfo;
	text = text_info.ToTitleCase(text);
	
	if (!text.Contains('\''))
	{
		return text;
	}
	
	StringBuilder sb = new StringBuilder(text);
	// Capitalize letter after the apostrophe
	int i = sb.ToString().IndexOf('\'');
	while (i != -1 && i+1 < sb.Length)
	{
		sb[i+1] = Char.ToUpper(sb[i+1], CultureInfo.InvariantCulture);
		i = sb.ToString().IndexOf('\'', i+1);
	}
	
	return sb.ToString();
}
