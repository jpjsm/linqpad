<Query Kind="Program">
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>


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
		Boo myBooEnum = word.ToEnum<Boo>(returnDefault: true);

		string filteredWord = myBooEnum.ToEnumString();
		Console.WriteLine($"Word: {word} --> {filteredWord}");
	}
	
	Boo unAssignedValue = (Boo) 64;

	Console.WriteLine($"{unAssignedValue} --> {unAssignedValue.ToEnumString()}");

}

// You can define other methods, fields, classes and namespaces here

public enum Foo
{
	[EnumMember(Value = "Initial Value")]
	Default, // Implicit underlying value of 0 (zero)
	In,
	A,
	Far,
	Away
}

public enum Boo
{
	[EnumMember(Value = "Milky Way")]
	Universe = 32, // Explicit start at 32
	In,
	A,
	[EnumDefaultValueAttribute()]
	Far,
	Away
}

public static class Utils
{
	public static string ToEnumString<T>(this T instance)
		where T : Enum
	{
		var enumType = typeof(T);
		if (!Enum.IsDefined(enumType, instance))
		{
			return $"Un-assigned value of {instance}";
		}
		
		var name = Enum.GetName(enumType, instance);
		var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).FirstOrDefault();
		return enumMemberAttribute != null ? enumMemberAttribute.Value : name;
	}

	public static T ToEnum<T>(this string label, bool ignoreCase = true, bool returnDefault = false)
		where T : Enum
	{
		var enumType = typeof(T);
		T enumDefault = default;
		bool defaultFound = false;
		foreach (var name in Enum.GetNames(enumType))
		{
			var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).FirstOrDefault();
			if (enumMemberAttribute != null)
			{
				if (string.Compare(enumMemberAttribute.Value, label, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture) == 0) 
				return (T)Enum.Parse(enumType, name);
			}

			if (string.Compare(name, label, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture) == 0)
			{
				return (T)Enum.Parse(enumType, name);
			}
			
			// Let's capture the default, in case we need it later
			if (returnDefault)
			{
				var defaultMemberAttribute = ((EnumDefaultValueAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumDefaultValueAttribute), true)).FirstOrDefault();
				if (defaultMemberAttribute != null)
				{
					enumDefault = (T)Enum.Parse(enumType, name);
					defaultFound = true;
				}
			}
		}
		
		if (returnDefault)
		{
			if (defaultFound)
			{
				return enumDefault;
			}
			
			return (T)Enum.GetValues(enumType).GetValue(0); // This makes explicitly the first value mentioned in the Enum definition, as the default value of the Enum.
		}
		//throw exception or whatever handling you want or
		throw new ArgumentOutOfRangeException($"Label not defined: {label}");
	}
}

public class EnumDefaultValueAttribute : System.Attribute
{

	public bool IsDefaultValue
	{
		get { return true; }
	}
	
		public EnumDefaultValueAttribute()
		{
			// default constructor
		}
}