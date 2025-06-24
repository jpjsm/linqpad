<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
</Query>

void Main()
{
	/*  =========== Writing Methods and Classes in LINQPad ====================
		https://albao.wordpress.com/2011/04/26/writing-methods-and-classes-in-linqpad/
	*/
	
	string url = "https://changemanager.fcm.azure.microsoft.com/api/eventsretrieval/events?startDate=2020-10-10T18:11:36.835Z&endDate=2022-11-10T18:11:36.835Z";
	string certificateFilePath = "C:\\Users\\jujofre\\Downloads\\fcm-changemanager-kv-kv-changeguardclientcert-20210128.pfx";
	
	
	X509Certificate2 certificate = new X509Certificate2(certificateFilePath, string.Empty);
	HttpClientHandler handler = new HttpClientHandler();
	handler.ClientCertificateOptions = ClientCertificateOption.Manual;
	handler.ClientCertificates.Add(certificate);
	
	int i = 1;
	while (HttpGetWithClientCert(url, certificateFilePath, i))
	{		
		System.Threading.Thread.Sleep(1000);
		i++;
	}
}

// You can define other methods, fields, classes and namespaces here
public bool HttpGetWithClientCert(string url, string certLocation, int i)
{
	X509Certificate2 certificate = new X509Certificate2(certLocation, string.Empty);
	HttpClientHandler handler = new HttpClientHandler();
	handler.ClientCertificateOptions = ClientCertificateOption.Manual;
	handler.ClientCertificates.Add(certificate);

	using (var client = new HttpClient(handler))
	{
		var response = client.GetAsync(url).Result;
		string status = response.StatusCode.ToString();
		int status_code = (int)response.StatusCode;

		string json = response.Content.ReadAsStringAsync().Result;

		//Console.WriteLine($"{i,9:N0}[{status_code}: {status}]:\n");
		Console.Write('.');
		response.Dispose();
	}

	return true;
}