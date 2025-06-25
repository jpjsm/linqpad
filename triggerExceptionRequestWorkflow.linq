<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

void Main()
{
	int sequence = 1;
	Guid exceptionRequestId = Guid.NewGuid();
	ApproversEmailData approversEmailData = new ApproversEmailData()
			{
				Approver = "JuanPablo Jofre, JuanPablo Jofre",
				ApproverEmail = "jujofre@microsoft.com; jujofre@microsoft.com",
				ExceptionRequestId = exceptionRequestId.ToString(),
				Requestor = "JuanPablo Jofre",
				RequestorEmail = "jujofre@microsoft.com",
				Title = $"Test {sequence}",
				BusinessJustification = $"Test {sequence}",
				ExceptionBeginsOn = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC"),
				ExceptionEndsOn = DateTime.UtcNow.AddDays(7.0).ToString("yyyy-MM-dd HH:mm UTC"),
				Services = string.Join(";", new Guid[] { Guid.NewGuid(), Guid.NewGuid() }.Select(s => s.ToString())),
				Subject = $"[Exception Request] Test {sequence}",
				Body = FillInBody(
				approvers: "JuanPablo Jofre, JuanPablo Jofre",
				requestor: "JuanPablo Jofre",
				exceptionRequestId: exceptionRequestId.ToString(),
				title: $"Test {sequence}",
				businessJustification: $"Test {sequence}",
				services: "Hello, World",
				startDate: DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC"),
				endDate: DateTime.UtcNow.AddDays(7.0).ToString("yyyy-MM-dd HH:mm UTC"),
				deploymentSystem: "Ev2",
				riskAssessment: "medium",
				sdp: "Y"
				),
			};
	
	string workflowEndpoint = "<add updated URL>";
	string data = JsonConvert.SerializeObject(approversEmailData);
	StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
	
	using var client = new HttpClient();
	var response = client.PostAsync(workflowEndpoint, content).Result;
	int status = (int)response.StatusCode;
	
	if (status != 200 && status != 201 && status != 202)
	{
	throw new Exception($"Application Internal Error: Unhandled status code ${status}");
	}

	Console.WriteLine($"StatusCode: {response.StatusCode} [{status}]");
}

string FillInBody(
			string approvers,
			string requestor,
			string exceptionRequestId,
			string title,
			string businessJustification,
			string services,
			string startDate,
			string endDate,
			string deploymentSystem,
			string riskAssessment,
			string sdp
			)
{
	string body =
		$"   <table style=\"width:90%;font-family: arial, sans-serif;padding-left: 5%;\" >" +
		$"       <tr style=\"font-size: 200%;background-color:#0663af;color:white\"><th colspan=\"2\">Approval Request Notification</th></tr>" +
		$"       <tr style=\"font-size: 150%;background-color:#0663af;color:white\"><th colspan=\"2\">You are receiving this email because you are identified as an approver</th></tr>" +
		$"       <tr><th colspan=\"2\">&nbsp;</th></tr>" +
		$"       <tr style=\"font-size: 100%;color:black\"><td colspan=\"2\"><strong>Hello {approvers}</strong></td></tr>" +
		$"       <tr style=\"font-size: 100%;color:black\"><td colspan=\"2\"><p>The following exception request submitted by {requestor}, is ready for your review and decision.</p><p>Please review the exception details below and click on the link below to record your decision of approve or reject.</p></td></tr>" +
		$"       <tr><td style=\"width: 30%;\"><strong>Exception Request ID</strong>:</td><td>{exceptionRequestId}</td></tr>" +
		$"       <tr><td style=\"width: 30%;\"><strong>Title</strong>:</td><td>{title}</td></tr>" +
		$"       <tr><td style=\"width: 30%;\"><strong>Submitter</strong>:</td><td>{requestor}</td></tr>" +
		$"       <tr><td style=\"width: 30%;\"><strong>Business Justification</strong>:</td><td>{businessJustification}</td></tr>" +
		$"       <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"       <tr><td  colspan=\"2\"><strong>This currently impacts ...</strong>:</td></tr>" +
		$"       <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"        <tr><td style=\"width: 30%;\"><strong>Services Involved</strong>:</td><td>{services}</td></tr>" +
		$"        <tr><td style=\"width: 30%;\"><strong>Exception Period</strong>:</td><td><strong>from:</strong> {startDate} <strong>to:</strong>> {endDate}</td></tr>" +
		$"        <tr><td style=\"width: 30%;\"><strong>Deployment system that will be used</strong>:</td><td>{deploymentSystem}</td></tr>" +
		$"        <tr><td style=\"width: 30%;\"><strong>Risk Assessment</strong>:</td><td>{riskAssessment}</td></tr>" +
		$"        <tr><td style=\"width: 30%;\"><strong>Does this deployment follow SDP guidelines</strong>:</td><td>{sdp}</td></tr>" +
		$"        <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"        <tr><td colspan=\"2\"><a href=\"https://portal.changemanager.fcm.azure.microsoft.com/home/Exception/{exceptionRequestId}\">Click here to register your decision</a></td></tr>" +
		$"        <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"        <tr><td colspan=\"2\"><strong>Support</strong></td></tr>" +
		$"        <tr><td colspan=\"2\">For any technical assistance or general feedback please email <a href=\"mailto:fcmsupport@microsoft.com\">FCM feedback</a>.</td></tr>" +
		$"    </table>";
	return body;
}
}

class ApproversEmailData
{
	public string Approver { get; set; }
	public string ApproverEmail { get; set; }
	public string Subject { get; set; }
	public string ExceptionRequestId { get; set; }
	public string Body { get; set; }
	public string Requestor { get; set; }
	public string RequestorEmail { get; set; }
	public string Title { get; set; }
	public string BusinessJustification { get; set; }
	public string Services { get; set; }
	public string ExceptionBeginsOn { get; set; }
	public string ExceptionEndsOn { get; set; }


//}
