<Query Kind="Program" />

void Main()
{
	(var Data, var Flat) = GenerateData();
	
	List<A> fromFlat = Flat
		                .GroupBy(A_Groups => A_Groups.A_Id)
						.Select(Ag => new A() 
						                	{ 
												Id = Ag.Key,
							                	Label = $"A{Ag.Key,3:0}",
							               		Bs = Ag.Select(bg => (bg.B_Id, bg.C_Id))
														.GroupBy(B_Groups => B_Groups.B_Id)
														.Select(Bg => new B() 
																		{ 
																			Id = Bg.Key,
																			Label = $"B{Bg.Key,3:0}",
																			Cs = Bg.Select(c => new C() 
																								{ 
																									Id =  c.C_Id,
																									Label = $"C{c.C_Id,3:0}"
																								}).ToList(),
																		} ).ToList()
											})
						  .ToList();
	foreach (var a in fromFlat)
	{
		Console.WriteLine($"A_Id: {a.Id}, Label: {a.Label}");
		foreach(var b in a.Bs)
		{
			Console.WriteLine($"    B_Id: {b.Id}, Label: {b.Label}");
			foreach (var c in b.Cs)
			{
				Console.WriteLine($"        C_Id: {c.Id}, Label: {c.Label}");
			}
		}
	}					  
						  
	} // end of statements
	
	public (List<A> Hierarchy, List<ABC_tuple> Flat) GenerateData()
	{
		List<A> data = new List<A>();
		List<ABC_tuple> flat = new List<ABC_tuple>();
	
		for (int i = 0; i < 6; i++)
		{
			A new_A = new A { Id = i + 1, Label = $"A{i + 1,3:0}" };
	
			for (int j = 0; j < 6; j++)
			{
				B new_B = new B { Id = j + 1, Label = $"B{j + 1,3:0}" };
	
				for (int k = 0; k < 6; k++)
				{
					new_B.Cs.Add(new C { Id = k + 1, Label = $"K{j + 1,3:0}" });
					flat.Add(new ABC_tuple{A_Id = i+1, B_Id = j+1, C_Id = k+1});
				}
	
				new_A.Bs.Add(new_B);
			}
	
			data.Add(new_A);
		}
		
		return (data, flat);
}

public class ABC_tuple
{
	public int A_Id { get; set; }
	public int B_Id { get; set; }
	public int C_Id { get; set; }
}

public class C
{
	public int Id {get;set;}
	public string Label {get;set;}
}

public class B
{
	public int Id {get;set;}
	public string Label {get;set;}
	public List<C> Cs {get;set;}
	
	public B()
	{
		Cs = new List<C>();
	}
}

public class A
{
	public int Id { get; set; }
	public string Label { get; set; }
	public List<B> Bs { get; set; }

	public A()
	{
		Bs = new List<B>();
	}
}
//}
