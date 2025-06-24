<Query Kind="Statements" />

Class[] demoClasses = CreateDemoClasses();

var classTableOfNames = demoClasses.SelectMany(c => c.Orders
							.SelectMany(o => o.Families
							.Select(f => (ClassName:c.ClassName, OrderName:o.OrderName, FamilyName: f.FamilyName))));

var classTableOfIds = demoClasses.SelectMany(c => c.Orders
							.SelectMany(o => o.Families
							.Select(f => (ClassId: c.ClassId, OrderId: o.OrderId, FamilyId: f.FamilyId))));

foreach (var classElement in classTableOfNames)
{
	Console.WriteLine($"{classElement.ClassName} | {classElement.OrderName} | {classElement.FamilyName}");
}

foreach (var classElement in classTableOfIds)
{
	Console.WriteLine($"{classElement.ClassId} | {classElement.OrderId} | {classElement.FamilyId}");
}

}

public class Family
{
	public string FamilyName {get; set;}
	public Guid FamilyId {get;set;}
}

public class Order
{
	public string OrderName {get; set;}
	public Guid OrderId { get; set; }
	public Family[] Families {get;set;}
}

public class Class
{
	public string ClassName { get; set; }
	public Guid ClassId { get; set; }
	public Order[] Orders { get; set; }

}

public Class[] CreateDemoClasses()
{
	StringBuilder baseGuidString = new StringBuilder("f0000000-0000-0000-0000-000000000000");
	Class[] classes = new Class[3];

	for (int i = 0; i < 3; i++)
	{
		StringBuilder classGuidString = baseGuidString;
		classGuidString[1] = $"{i + 1}"[0];
		Console.WriteLine($"classGuidString: {classGuidString}");
		Guid classGuid = new Guid(classGuidString.ToString());

		string className = $"{i+ 1}.0.0";

		Order[] orders = new Order[3];
		for (int j = 0; j < 3; j++)
		{
			StringBuilder orderGuidString = classGuidString;
			orderGuidString[9] = $"{j + 1}"[0];
			Console.WriteLine($"orderGuidString: {orderGuidString}");
			Guid orderGuid = new Guid(orderGuidString.ToString());

			string orderName = $"{i+1}.{j+1}.0";

			Family[] families = new Family[3];
			for (int k = 0; k < 3; k++)
			{
				StringBuilder familyGuidString = orderGuidString;
				familyGuidString[14] = $"{k + 1}"[0];
				Console.WriteLine($"familyGuidString: {orderGuidString}");
				Guid familyGuid = new Guid(familyGuidString.ToString());

				string familyName = $"{i+1}.{j+1}.{k+1}";

				families[k] = new Family() { FamilyName = familyName, FamilyId = familyGuid };
			}

			orders[j] = new Order() { OrderName = orderName, OrderId = orderGuid, Families = families };
		}

		classes[i] = new Class() { ClassName = className, ClassId = classGuid, Orders = orders };
	}
	
	return classes;



