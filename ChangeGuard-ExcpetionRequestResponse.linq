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
	ExceptionRequest existingexreq = new ExceptionRequest();
    ExceptionRequestResponse response = new ExceptionRequestResponse()
    {
		StatusCode = "200",
		Exceptions = new UserQuery.ExceptionRequest[] {existingexreq, existingexreq}
	};

	Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented));
}

// You can define other methods, fields, classes and namespaces here
public enum ExceptionRequestStatus
{
	PendingSubmission,
	PendingApproval,
	Delegated,
	Declined,
	Rejected = Declined,
	PartiallyDeclined,
	Approved,
}

public enum DeploymentSystemEnum
{
	Ev2,
	AzDeployer
}

public class Subscription
{
	public Guid SubscriptionId { get; set; }
	public string[] Regions { get; set; } // ["*"] => means all regions
}

public class Service
{
	public Guid ServiceId { get; set; }
	public string ServiceName { get; set; }
	public Subscription[] Subscriptions { get; set; }
}

public class StatusChange
{
	public string Status { get; set; }
	public DateTime ChangeDate { get; set; }
}

public class ApproverInfo
{
	public Guid? ApproverId { get; set; }
	public string ApproverName { get; set; }
	public string ApproverEmail { get; set; }
}

public class ApproverNotes
{
	public string ApproverEmail { get; set; }
	public string Notes { get; set; }
}

public class ExceptionRequest
{
	private ExceptionRequestStatus status;
	private DeploymentSystemEnum deploymentSystem = DeploymentSystemEnum.Ev2;

	public Guid ExceptionRequestId { get; set; }

	public DateTime RequestDate { get; set; }

	public string RequestorName { get; set; }
	public string RequestorEmail { get; set; }
	public string Title { get; set; }
	public string BusinessJustification { get; set; }
	public DateTime ExceptionBeginsOn { get; set; }
	public DateTime ExceptionEndsOn { get; set; }
	public Guid? DeploymentOfOrigin { get; set; }
	public Guid[] EventIds { get; set; }
	public Service[] Services { get; set; } // linked table ExceptionRequestServices

	public string ApprovedBy { get; set; }

	public string FollowsSDP { get; set; }

	public string DeploymentSystem { get; set; }

	public string RiskAssessment { get; set; }

	public string Status
	{
		get
		{
			return Enum.GetName(typeof(ExceptionRequestStatus), status);
		}
		set
		{
			// value gets set on the out variable
			if (!Enum.TryParse<ExceptionRequestStatus>(value, ignoreCase: true, out status))
			{
				throw new ArgumentOutOfRangeException($"Provided value '{value}' not defined");
			}
		}
	}

	public ApproverInfo[] Approvers { get; set; }

	public DateTime? LastUpdate { get; set; }

	public Guid[] FormerApproverIds { get; set; } // linked table ExceptionRequestFormerApproverIds

	public StatusChange[] ChangeStatus { get; set; } // linked table ExceptionRequestStatusChange

	public ApproverNotes[] ApproversNotes { get; set; } // linked table ExceptionRequestApproverNotes

}

public class ExceptionRequestResponse
{
	public string StatusCode { get; set; }
	public string ErrorMessage { get; set; }
	public ExceptionRequest[] Exceptions { get; set; }
	public string[] RejectedSubscriptions { get; set; }
}
