<Query Kind="Program" />

void Main()
{
	string table = FillInApprovalRequestBody("Approver1-Firstname Approver1-Lastname, Approver2-Firstname Approver2-Lastname","Requestor-Name Requestor-Lastname",Guid.NewGuid().ToString(),"Here goes the Title", "This is the Business Justification","Services",DateTime.UtcNow.ToString("O"),DateTime.UtcNow.AddDays(30).ToString("O"),"Ev2", "Low","Y");
	string html = $"<!DOCTYPE html><html><body>{table}</body></html>";
	System.IO.File.WriteAllText(@"C:\tmp\exceptionrequest-FillInApprovalRequestBody.html", html, Encoding.UTF8);
	
	table = FillInRequestExceptionDecisionNotificationBody("Approver1-Firstname Approver1-Lastname","Requestor-Name Requestor-Lastname",Guid.NewGuid().ToString(),"Here goes the Title", "Approved|Declined");
	html = $"<!DOCTYPE html><html><body>{table}</body></html>";
	System.IO.File.WriteAllText(@"C:\tmp\exceptionrequest-FillInRequestExceptionDecisionNotificationBody.html", html, Encoding.UTF8);
}

// You can define other methods, fields, classes and namespaces here
private string FillInApprovalRequestBody(
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
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;\" >" +
		$"  <tr style=\"font-size: 200%;background-color:#0663af;color:white;\"><th colspan=\"2\" >Approval Request Notification</th></tr>" +
		$"  <tr style=\"font-size: 150%;background-color:#0663af;color:white;\"><th colspan=\"2\" >You are receiving this email because you are identified as an approver</th></tr>" +
		$"  <tr><th colspan=\"2\">&nbsp;</th></tr>" +
		$"  <tr style=\"font-size: 100%;color:black;\"><td colspan=\"2\" ><strong>Hello {approvers}</strong></td></tr>" +
		$"  <tr style=\"font-size: 100%;color:black;\"><td colspan=\"2\" ><p>The following exception request submitted by <strong>{requestor}</strong>, is ready for your review and decision.</p><p>Please review the exception details below and click on the link below to record your decision of approve or reject.</p></td></tr>" +
		$"</table>" +
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;border-collapse: collapse;\" >" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Exception Request ID</strong>:</td><td style=\"border: 1px solid black;\">{exceptionRequestId}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Title</strong>:</td><td style=\"border: 1px solid black;\">{title}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Submitter</strong>:</td><td style=\"border: 1px solid black;\">{requestor}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Business Justification</strong>:</td><td style=\"border: 1px solid black;\">{businessJustification}</td></tr>" +
		$"</table>" +
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;\" >" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"  <tr><td colspan=\"2\"><strong>This currently impacts</strong>:</td></tr>" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"</table>" +
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;border-collapse: collapse;\" >" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Services Involved</strong>:</td><td style=\"border: 1px solid black;\">{services}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Exception Period</strong>:</td><td style=\"border: 1px solid black;\"><p><strong>From:</strong> {startDate}</p><p><strong>To:</strong> {endDate}</p></td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Deployment system that will be used</strong>:</td><td style=\"border: 1px solid black;\">{deploymentSystem}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Risk Assessment</strong>:</td><td style=\"border: 1px solid black;\">{riskAssessment}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Does this deployment follows SDP guidelines</strong>:</td><td style=\"border: 1px solid black;\">{sdp}</td></tr>" +
		$"</table>" +
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;border-collapse: collapse;\" >" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"  <tr><td colspan=\"2\"><a href=\"https://portal.changemanager.fcm.azure.microsoft.com/home/Exception/{exceptionRequestId}\">Click here to register your decision</a></td></tr>" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"  <tr><td colspan=\"2\"><strong>Support</strong></td></tr>" +
		$"  <tr><td colspan=\"2\">For any technical assistance or general feedback please email <a href=\"mailto:fcmsupport@microsoft.com\">FCM feedback</a>.</td></tr>" +
	    $"</table>";
	return body;
}

private string FillInRequestExceptionDecisionNotificationBody(
	string approver,
	string requestor,
	string exceptionRequestId,
	string title,
	string status
	)
{
	string body =
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;\" >" +
		$"  <tr style=\"font-size: 200%;background-color:#0663af;color:white\"><th colspan=\"2\">Request Decision Notification</th></tr>" +
		$"  <tr style=\"font-size: 150%;background-color:#0663af;color:white\"><th colspan=\"2\">You are receiving this email because you are identified as the requestor for an exception</th></tr>" +
		$"  <tr><th colspan=\"2\">&nbsp;</th></tr>" +
		$"  <tr style=\"font-size: 100%;color:black\"><td colspan=\"2\"><strong>Hello {requestor}</strong></td></tr>" +
		$"  <tr><th colspan=\"2\">&nbsp;</th></tr>" +
		$"  <tr style=\"font-size: 100%;color:black\"><td colspan=\"2\">The exception '{exceptionRequestId}' was reviewed by <strong>{approver}</strong> and a decision was made.</td></tr>" +
		$"  <tr><td  colspan=\"2\">The request is <strong>{status}</strong>.</td></tr>" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"</table>" +
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;border-collapse: collapse;\" >" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Exception Request ID</strong>:</td><td style=\"border: 1px solid black;\">{exceptionRequestId}</td></tr>" +
		$"  <tr><td style=\"width: 30%;border: 1px solid black;\"><strong>Title</strong>:</td><td style=\"border: 1px solid black;\">{title}</td></tr>" +
		$"</table>" +
		$"<table style=\"margin-left: auto;margin-right: auto;width:90%;font-family: arial, sans-serif;border-collapse: collapse;\" >" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"  <tr><td colspan=\"2\"><a href=\"https://portal.changemanager.fcm.azure.microsoft.com/home/Exception/{exceptionRequestId}\">Click here for details.</a></td></tr>" +
		$"  <tr><td colspan=\"2\">&nbsp;</td></tr>" +
		$"  <tr><td colspan=\"2\"><strong>Support</strong></td></tr>" +
		$"  <tr><td colspan=\"2\">For any technical assistance or general feedback please email <a href=\"mailto:fcmsupport@microsoft.com\">FCM feedback</a>.</td></tr>" +
		$"</table>";
	return body;
}

