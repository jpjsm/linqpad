<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
</Query>

/*  =========== Writing Methods and Classes in LINQPad ====================
	https://albao.wordpress.com/2011/04/26/writing-methods-and-classes-in-linqpad/
*/

string url = "https://change-manager.fcm.test/changeassessment/ev2/892b3db1-6658-4b7c-a11e-2b598b642049";
using var client = new HttpClient();
var response = await client.GetAsync(url);
string status = response.StatusCode.ToString();
int status_code = (int)response.StatusCode;

string json = response.Content.ReadAsStringAsync().Result;

Console.WriteLine($"[{status_code}: {status}]:\n\t {json}");
Ev2DeploymentAssessment deploymentAssessment = Newtonsoft.Json.JsonConvert.DeserializeObject<Ev2DeploymentAssessment>(json);

} // Add here the closing curly bracket that's ommitted at the end

public class Ev2DeploymentAssessmentDetail
{
	public Guid ServiceId { get; internal set; }
	public Guid SubscriptionId { get; internal set; }
	public string Region { get; internal set; }
	public bool AllowedToDeploy { get; internal set; }
	public Guid[] ExceptionsReviewed { get; internal set; }
}

public class Ev2DeploymentAssessment
{
    public Guid RolloutId { get; internal set; }
    public Ev2DeploymentAssessmentDetail[] Ev2DeploymentAssessmentDetails { get; internal set; }
    public string ExceptionRequestUrl { get; internal set; }
//}  