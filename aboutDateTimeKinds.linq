<Query Kind="Statements" />

DateTime localtime = DateTime.Now;
Console.WriteLine($"localtime    {localtime.ToString("o")} kind {localtime.Kind} ");
Console.WriteLine($"localtime    {localtime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")} kind {localtime.Kind} ");

DateTime updatedtime = new DateTime(localtime.Ticks, DateTimeKind.Utc);
Console.WriteLine($"updatedtime  {updatedtime.ToString("o")} kind {updatedtime.Kind} ");
