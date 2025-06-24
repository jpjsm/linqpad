<Query Kind="Statements">
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
</Query>

X509Certificate2 cert = null;

using (System.IO.StreamWriter file =
	new System.IO.StreamWriter(@"C:\tmp\certificates.txt"))
{

	foreach (StoreLocation loc in new StoreLocation[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine })
	{
		foreach (var sn in new[] { StoreName.AddressBook, StoreName.AuthRoot, StoreName.CertificateAuthority, StoreName.My, StoreName.Root, StoreName.TrustedPeople, StoreName.TrustedPublisher})
		{
			X509Store certStore = new X509Store(sn, loc);
			try
			{
				certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
				X509Certificate2Collection certificateCollection = certStore.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
				if (certificateCollection.Count > 0)
				{
					foreach (X509Certificate2 c in certificateCollection)
					{
						cert = c;
						if (!cert.Subject.Contains("fcm", StringComparison.InvariantCultureIgnoreCase)) continue;
						file.WriteLine("=====================================");
						file.WriteLine("Subject: {0}", cert.Subject);
						file.WriteLine("\tLocation        : {0}", loc);
						file.WriteLine("\tStore Name      : {0}", sn);
						file.WriteLine();
						file.WriteLine("\tFriendly Name   : {0}", cert.FriendlyName);
						file.WriteLine("\tIssuer          : {0}", cert.Issuer);
						//file.WriteLine("\tIssuer Name     : {0}", cert.IssuerName);
						file.WriteLine("\tHas Private Key : {0}", cert.HasPrivateKey);
						file.WriteLine("\tSerial Number   : {0}", cert.SerialNumber);
						if (cert.Extensions.Count > 0)
						{
							file.WriteLine("\tExtensions");
							foreach (var e in cert.Extensions)
							{
								file.WriteLine("\t\tOid   : {0} {1}", e.Oid.FriendlyName, e.Oid.Value);
							}
						}
						file.WriteLine();
						file.WriteLine("-------------------------------------");
						file.WriteLine();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(loc);
				Console.WriteLine(sn);

				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.Data);
				Console.WriteLine(ex.HResult);
				Console.WriteLine(ex.Source);
				if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
				//throw;
			}
		}
	}
}