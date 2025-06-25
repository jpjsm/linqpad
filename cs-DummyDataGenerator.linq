<Query Kind="Statements" />

const string baseguid = "a0000000-0000-0000-0000-000000000000";
Dictionary<int, string> romans = new Dictionary<int, string>() { {0, string.Empty}, {1, "I"}, {2, "II"}, {3, "III"}, {4, "IV"}, {5, "V"}, {6, "VI"}, {7, "VII"}, {8, "VIII"}, {9, "IX"}, {10, "X"} };
StringBuilder approverId = new StringBuilder(baseguid, 64);
//StringBuilder subscriptionId = new StringBuilder(serviceId.ToString(), 64);
//Console.WriteLine(baseguid);
for(int i = 1; i<=10;i++)
{
	approverId[1] = i.ToString("x")[0];
	string approverEmail = string.Format("cvp{0}@example.test",i.ToString("00"));
	string approverName = string.Format("Corporate Vice President {0}", romans[i]);
	
	//start writing
	Console.WriteLine("\t\tnew Approver(){");
	Console.WriteLine("\t\t\tApproverId = new Guid(\"{0}\"),", approverId);
	Console.WriteLine("\t\t\tApproverEmail = \"{0}\",", approverEmail);
	Console.WriteLine("\t\t\tApproverName = \"{0}\",", approverName);
	Console.WriteLine("\t\t\tServices = new Service[] {");
	StringBuilder serviceId = new StringBuilder(approverId.ToString(), 64);
	for (int j = 1; j <= 10; j++)
	{
		serviceId[9] = j.ToString("x")[0];
		string serviceName = string.Format("Service {0}.{1}", romans[i], romans[j]);

		//start writing, again
		Console.WriteLine("\t\t\t\tnew Service(){");
		Console.WriteLine("\t\t\t\t\tServiceId = new Guid(\"{0}\"),", serviceId);
		Console.WriteLine("\t\t\t\t\tServiceName = \"{0}\",", serviceName);
		Console.WriteLine("\t\t\t\t\tSubscriptions = new Subscription[] {");
		StringBuilder subscriptionId = new StringBuilder(serviceId.ToString(), 64);
		for (int k = 1; k <= 10; k++)
		{
			subscriptionId[14] = k.ToString("x")[0];
			string subscriptionName = string.Format("Subscription {0}.{1}.{2}", romans[i], romans[j], romans[k]);

			//start writing, again
			Console.WriteLine("\t\t\t\t\t\tnew Subscription(){");
			Console.WriteLine("\t\t\t\t\t\t\tSubscriptionId = new Guid(\"{0}\"),", subscriptionId);
			Console.WriteLine("\t\t\t\t\t\t\tSubscriptionName = \"{0}\"", subscriptionName);
			Console.WriteLine("\t\t\t\t\t\t},");
		}

		Console.WriteLine("\t\t\t\t}");
		Console.WriteLine("\t\t\t},");
	}

	Console.WriteLine("\t\t\t}");
	Console.WriteLine("\t\t},");
}
