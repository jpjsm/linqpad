<Query Kind="Statements">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

string cnxstr = "Server=tcp:fcm-changemanagersql.database.windows.net,1433;Initial Catalog=fcm-changemanagersql;Persist Security Info=False;User ID=changemanageradmin;Password=Ch1ng2M1n1g@r1dm3n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

int skip = 0; // last failure value
string nonWordChars = "[^A-Za-z0-9]";
Regex rgx = new Regex(nonWordChars);

Dictionary<Guid, HashSet<string>> SvcWords = new Dictionary<Guid, HashSet<string>>();
Dictionary<Guid, HashSet<string>> SubWords = new Dictionary<Guid, HashSet<string>>();

DataSet serviceSubscriptionTable = new DataSet();

DateTime start = DateTime.Now;
using (SqlConnection cnxReader = new SqlConnection(cnxstr))
{
	cnxReader.Open();
	SqlDataAdapter qry = new SqlDataAdapter("SELECT [ServiceId], [SubscriptionId], [ServiceName], [SubscriptionName] FROM [ServicesInfo].[ServiceTree_OrganizationHierarchy]", cnxReader);
	qry.Fill(serviceSubscriptionTable, "ServiceTree_OrganizationHierarchy");
}

Console.WriteLine($"Total rows read: {serviceSubscriptionTable.Tables[0].Rows.Count,18:N0}, time to process: {DateTime.Now - start}");

int row = 0;
start = DateTime.Now;
DateTime begin = start;
foreach (DataRow  serviceSubscriptionRow in serviceSubscriptionTable.Tables[0].Rows)
{
	using (SqlConnection cnxInsert = new SqlConnection(cnxstr))
	{
		cnxInsert.Open();

		SqlCommand insertWord2ServiceId = new SqlCommand("INSERT INTO [ServicesInfo].[Word2ServiceId](word, serviceId) VALUES (@word, @serviceId)", cnxInsert);
		insertWord2ServiceId.Parameters.Add("@serviceId", SqlDbType.UniqueIdentifier);
		insertWord2ServiceId.Parameters.Add("@word", SqlDbType.NVarChar, 128);

		SqlCommand insertWord2SubscriptionId = new SqlCommand("INSERT INTO [ServicesInfo].[Word2SubscriptionId](word, subscriptionId) VALUES (@word, @subscriptionId)", cnxInsert);
		insertWord2SubscriptionId.Parameters.Add("@subscriptionId", SqlDbType.UniqueIdentifier);
		insertWord2SubscriptionId.Parameters.Add("@word", SqlDbType.NVarChar, 128);

		Guid serviceId = new Guid(serviceSubscriptionRow["ServiceId"].ToString());
		if (!SvcWords.ContainsKey(serviceId))
		{
			SvcWords.Add(serviceId, new HashSet<string>());
		}

		Guid subscriptionId = new Guid(serviceSubscriptionRow["SubscriptionId"].ToString());
		if (!SubWords.ContainsKey(subscriptionId))
		{
			SubWords.Add(subscriptionId, new HashSet<string>());
		}

		string serviceName = serviceSubscriptionRow["ServiceName"].ToString().ToLowerInvariant();
		string subscriptionName = serviceSubscriptionRow["SubscriptionName"].ToString().ToLowerInvariant();
		HashSet<string> subscriptionWords = rgx.Replace(subscriptionName, " ").Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2).ToHashSet();
		HashSet<string> serviceWords = rgx.Replace(serviceName, " ").Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2).ToHashSet();

		insertWord2SubscriptionId.Parameters["@subscriptionId"].SqlValue = subscriptionId;
		foreach (string subscriptionWord in subscriptionWords)
		{
			if (SubWords[subscriptionId].Add(subscriptionWord))
			{
				insertWord2SubscriptionId.Parameters["@word"].SqlValue = subscriptionWord;
				insertWord2SubscriptionId.ExecuteNonQuery();
			}
		}

		insertWord2ServiceId.Parameters["@serviceId"].SqlValue = serviceId;
		foreach (string serviceWord in serviceWords)
		{
			if (SvcWords[serviceId].Add(serviceWord))
			{
				insertWord2ServiceId.Parameters["@word"].SqlValue = serviceWord;
				insertWord2ServiceId.ExecuteNonQuery();
			}
		}

	}

	if ((++row%1000)== 1)
	{
		int remaining = serviceSubscriptionTable.Tables[0].Rows.Count - row;
		TimeSpan elapsed = DateTime.Now - begin;
		TimeSpan elapsedUnit = elapsed / row;
		DateTime EstimatedEnd = DateTime.Now + (elapsedUnit * remaining);
		Console.WriteLine($"Processing row : {row,18:N0}, time to process: {DateTime.Now - start}, Estimated End {EstimatedEnd:s}");
		start = DateTime.Now;
		
	}
}