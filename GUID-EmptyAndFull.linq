<Query Kind="Statements">
  <Output>DataGrids</Output>
</Query>

Console.WriteLine(
	"{0}, {1}, {2}, {3}",
	Guid.Empty.ToString(),
	Convert.ToBase64String(Guid.Empty.ToByteArray()).Replace("/", "_").Replace("+", "-").Replace("=", string.Empty),
	new Guid("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), 
	Convert.ToBase64String((new Guid("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")).ToByteArray()).Replace("/", "_").Replace("+", "-").Replace("=", string.Empty)
);