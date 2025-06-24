<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{	
	dynamic Ev2DeploymentAssessment = JsonConvert.DeserializeObject(Ev2DeploymentAssessment_str);
	List<(Guid subscriptionId, bool allowedToDeploy, dynamic exceptionsReviewed)> detailInfos = new List<(System.Guid subscriptionId, bool allowedToDeploy, dynamic exceptionsReviewed)>();
	foreach (dynamic ev2DeploymentAssessmentDetail in Ev2DeploymentAssessment.ev2DeploymentAssessmentDetails)
	{
		detailInfos.Add(
			(
				new Guid(ev2DeploymentAssessmentDetail.subscriptionId.ToString()), 
				Convert.ToBoolean(ev2DeploymentAssessmentDetail.allowedToDeploy.ToString()),
				ev2DeploymentAssessmentDetail.exceptionsReviewed
			));
	}
	
	foreach ((Guid subscriptionId, bool allowedToDeploy, dynamic exceptionsReviewed) detailInfo in detailInfos)
	{
		Console.WriteLine($"subscriptionId: {detailInfo.subscriptionId}, allowedToDeploy: {detailInfo.allowedToDeploy}, exceptionsReviewed: '{detailInfo.exceptionsReviewed}'");
	}
}

// You can define other methods, fields, classes and namespaces here
private static string Ev2DeploymentAssessment_str = "{" +
"  \"rolloutId\": \"faff8aab-f77f-4435-b96b-d9efb9a2d0ac\"," +
"  \"ev2DeploymentAssessmentDetails\": [" +
"    {" +
"      \"serviceId\": \"ffffffff-0101-0101-0100-000000000000\"," +
"      \"subscriptionId\": \"ffffffff-0101-0101-0101-000000000000\"," +
"      \"region\": \"*\"," +
"      \"allowedToDeploy\": true," +
"      \"exceptionsReviewed\": [" +
"        \"709f60d8-6a88-4dc4-aff7-bff656512f87\"" +
"      ]" +
"    }," +
"    {" +
"      \"serviceId\": \"ffffffff-0101-0101-0100-000000000000\"," +
"      \"subscriptionId\": \"ffffffff-0101-0101-0103-000000000000\"," +
"      \"region\": \"*\"," +
"      \"allowedToDeploy\": false," +
"      \"exceptionsReviewed\": []" +
"    }" +
"  ]," +
"  \"exceptionRequestUrl\": \"https://portal.changemanager.fcm.azure.microsoft.com/home/fromdeployment/q4r_-n_3NUS5a9nvuaLQrA\"" +
"}";
