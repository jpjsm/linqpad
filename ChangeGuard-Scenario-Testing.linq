<Query Kind="Program">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>Microsoft.SqlServer.Server</Namespace>
  <Namespace>System.Data.Sql</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Data.SqlTypes</Namespace>
</Query>

void Main()
{
	string workingDirectory = @"C:\FCM\FCM-ChangeManager - local\src\ScenarioTesting";
	string local_secrets = @"C:\FCM\.local\.secrets\appsettings.secrets.json";
	string user = "sa";
	string pwd = Guid.NewGuid().ToString();

	Mastercnxstr = $"Server=tcp:localhost,5433;Initial Catalog=master;Persist Security Info=False;User ID={user};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=600;";
	Changemanagersqlcnxstr = $"Server=tcp:localhost,5433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID={user};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=600;";
	
	Console.WriteLine("Update secrets");
	UpdateSecrets(local_secrets, user, pwd, workflow_file: string.Empty, requestExceptionApproval_url: string.Empty, requestExceptionDecisionNotification_url: string.Empty);
	
	Console.WriteLine("docker-compose up --build -d");
	DockerCompose("up --build -d", workingDirectory, pwd);


	Console.WriteLine("wait a little for the db container to start setup");
	Thread.Sleep(15000);

	Console.WriteLine("Start creating the DB");
	CreateDatabase(user, pwd);

	Console.WriteLine("wait a little for the db container to finish setup");
	Thread.Sleep(15000);

	return;

	Console.WriteLine("docker-compose down");
	DockerCompose("down", workingDirectory, pwd);
	Console.WriteLine("Done.");
}

// You can define other methods, fields, classes and namespaces here
public static string Changemanagersqlcnxstr = string.Empty;
public static string Mastercnxstr = string.Empty;


public bool UpdateSecrets(
	string secrets_file, 
	string user, 
	string pwd, 
	string workflow_file, 
	string requestExceptionApproval_url, 
	string requestExceptionDecisionNotification_url
)
{
	string userId_pattern = @";User ID=.+?;";
	string pwd_pattern = @";Password=.+?;";
	string secrets = File.ReadAllText(secrets_file);

	secrets = Regex.Replace(secrets, userId_pattern, $";User ID={user};");
	secrets = Regex.Replace(secrets, pwd_pattern, $";Password={pwd};");

	File.WriteAllText(secrets_file, secrets);
	Console.WriteLine($"Updated  appsettings.secrets.json");
	return true;
}
public bool DockerCompose(string command, string workingDirectory, string sa_password)
{
	string stdout = null;
	string stderr = null;
	
	ProcessStartInfo startInfo = new ProcessStartInfo()
	{
		FileName = "cmd.exe",
		Arguments = $"/c docker-compose {command}",
		UseShellExecute = false,
		WorkingDirectory = workingDirectory,
		CreateNoWindow = true,
		RedirectStandardOutput = true,
		RedirectStandardError = true
	};
	
	startInfo.Environment.Add("fcmchangemanagersapwd", sa_password);

	Process proc = new Process()
	{
		StartInfo = startInfo		
	};

	proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => { stderr += e.Data + Environment.NewLine; });
	proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) => { stdout += e.Data + Environment.NewLine; });
	
	proc.Start();
	proc.BeginOutputReadLine();
	proc.BeginErrorReadLine();
	
	proc.WaitForExit();

	Console.WriteLine($"StdOut: {stdout}");
	Console.WriteLine($"StdErr: {stderr}");

	return true;
}

public bool CreateDatabase(string user, string pwd)
{
	string mastercnxstr = $"Server=tcp:localhost,5433;Initial Catalog=master;Persist Security Info=False;User ID={user};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=600;";
	string changemanagersqlcnxstr = $"Server=tcp:localhost,5433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=sa;Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=600;";

	
	string list_databases = "select [name] as [Database_Name] from sys.databases";
	string list_tables = "SELECT S.[Name] + '.' + T.[Name]  AS [Table_Name] FROM sys.tables T join sys.schemas S on T.schema_id = S.schema_id";

	string[] create_database_lines = File.ReadAllLines(@"C:\FCM\FCM-ChangeManager\src\ScenarioTesting\create-db-fcm-changemanager.sql");
	string[] create_database_objects_lines = File.ReadAllLines(@"C:\FCM\FCM-ChangeManager\src\ScenarioTesting\create-fcm-changemanager-objects.sql");
	string[] create_database_data_lines = File.ReadAllLines(@"C:\FCM\FCM-ChangeManager\src\ScenarioTesting\create-db-fcm-changemanager-data.sql");

	using (SqlConnection mastercnx = new SqlConnection(mastercnxstr))
	{
		mastercnx.Open();
		//  Create Database
		StringBuilder sql_command = new StringBuilder();
		foreach (string line in create_database_lines)
		{
			if (string.Compare(line, "GO", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				SqlCommand cmd = new SqlCommand(sql_command.ToString(), mastercnx);
				cmd.ExecuteNonQuery();

				sql_command.Clear();
				continue;
			}

			sql_command.AppendLine(line);
		}

		if (sql_command.Length > 0)
		{
			SqlCommand cmd = new SqlCommand(sql_command.ToString(), mastercnx);
			cmd.ExecuteNonQuery();

			sql_command.Clear();
		}

		// 	List databases
		SqlCommand qry_databases = new SqlCommand(list_databases, mastercnx);

		SqlDataReader databasesReader = qry_databases.ExecuteReader();

		Console.WriteLine($"{new string('=', 32)} Databases {new string('=', 32)}");
		if (databasesReader.HasRows)
		{
			while (databasesReader.Read())
			{
				Console.WriteLine($"\t{databasesReader.GetString(0).ToLowerInvariant()}");
			}
		}
		else
		{
			Console.WriteLine($"{new string('.', 31)}No Databases {new string('.', 31)}");
		}

		Console.WriteLine(new string('-', 72));
		databasesReader.Close();

		mastercnx.Close();
	}

	using (SqlConnection changemanagercnx = new SqlConnection(changemanagersqlcnxstr))
	{
		changemanagercnx.Open();
		foreach (string[] lines in new List<string[]>() {create_database_objects_lines, create_database_data_lines})
		{
			StringBuilder sql_command = new StringBuilder();
			foreach (string line in lines)
			{
				if (string.Compare(line, "GO", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					SqlCommand cmd = new SqlCommand(sql_command.ToString(), changemanagercnx);
					cmd.ExecuteNonQuery();

					sql_command.Clear();
					continue;
				}

				sql_command.AppendLine(line);
			}

			if (sql_command.Length > 0)
			{
				SqlCommand cmd = new SqlCommand(sql_command.ToString(), changemanagercnx);
				cmd.ExecuteNonQuery();

				sql_command.Clear();
			}
		}

		// 	List databases
		SqlCommand qry_tables = new SqlCommand(list_tables, changemanagercnx);

		SqlDataReader tablesReader = qry_tables.ExecuteReader();

		Console.WriteLine($"{new string('=', 32)} Tables {new string('=', 32)}");
		if (tablesReader.HasRows)
		{
			while (tablesReader.Read())
			{
				Console.WriteLine($"\t{tablesReader.GetString(0).ToLowerInvariant()}");
			}
		}
		else
		{
			Console.WriteLine($"{new string('.', 31)}No Tables {new string('.', 31)}");
		}

		Console.WriteLine(new string('-', 72));
		tablesReader.Close();

		changemanagercnx.Close();
	}

	return true;
}

