<Query Kind="Program">
  <Output>DataGrids</Output>
</Query>

void Main()
{
	string[] guids_to_check = {
		"a0a46d95-77ef-4cb4-a6d8-6aa5a288c1db",
		"ffffffff-0101-0101-0100-000000000000",
		"a338de01-a914-4bc4-98b9-5b83338676c0",
		"889acfb9-923f-4e3f-9bf2-2a3f9d95fe4f",
		"df36aee8-c644-400b-a0ab-fd0f1191211d",
	};
	
	Guid received_guid;
	foreach (string guid_to_check in guids_to_check)
	{
		if(!Guid.TryParse(guid_to_check, out received_guid))
		{
			Console.WriteLine($"Not a valid GUID '{guid_to_check}'");
		}
	}
	
}

// You can define other methods, fields, classes and namespaces here
