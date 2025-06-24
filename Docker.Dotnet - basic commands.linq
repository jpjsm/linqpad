<Query Kind="Program">
  <NuGetReference>Docker.DotNet</NuGetReference>
  <Namespace>Docker.DotNet</Namespace>
  <Namespace>Docker.DotNet.Models</Namespace>
  <Namespace>Microsoft.Net.Http.Client</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{
	DockerClient client = new DockerClientConfiguration()
	                          .CreateClient();
							  
	List<ContainerListResponse> containers = client.Containers
	                                                .ListContainersAsync(new ContainersListParameters(){All = true})
													.Result
													.ToList();
	Console.WriteLine($"Number of containers: {containers.Count}");											
	foreach (ContainerListResponse container in containers)
	{
		Console.WriteLine($"\tImage: {container.Image}, Names: {string.Join("; ", container.Names)}, State: {container.State}, Status: {container.Status}");
	}

	// Make sure the image is available
	string imagerepo = "jpjofresm";
	string imageName = "flask-hostname";
	string imagetag = "2.0.2";

	var images = client.Images.ListImagesAsync(new ImagesListParameters() {All = true}).Result;

	bool imageExists = false;
	Console.WriteLine($"Number of images: {images.Count}");
	foreach (ImagesListResponse image in images)
	{
		var repoTags = image.RepoTags != null ? string.Join("; ", image.RepoTags) : string.Empty;
		if (repoTags.Contains(imageName, StringComparison.InvariantCultureIgnoreCase))
		{
			imageExists = true;
		}
		var labels = image.Labels != null ? image.Labels.Select(kvp => $"'{kvp.Key}': '{kvp.Value}'") : new List<string>();
		var containerNumber = image.Containers;
		Console.WriteLine($"\tImage ID: {image.ID}, Repo Tags: {repoTags}, Labels: {string.Join("; ", labels ?? new string[] {})}, Container number: {containerNumber}.");
	}

	if (!imageExists)
	{
		Console.WriteLine($"Getting {imagerepo}/{imageName}:{imagetag}");
		client.Images.CreateImageAsync(		
		   new ImagesCreateParameters() 
		   { 
		      FromImage = imageName,
			  Repo = imagerepo,
			  Tag = imagetag
		   }, 
		   new AuthConfig(), 
		   new Progress<JSONMessage>()
		).Wait();
	}

	// Create the container definitions
		
	
	string containername = "Echo-hostname";

	var config = new Config()
	{
		Hostname = "localhost"
	};

	var exposedPorts = new Dictionary<string, EmptyStruct>
	{
		{
			"5000", default(EmptyStruct)
		}
	};

	// Configure the ports to expose
	var hostConfig = new HostConfig()
        {
            PortBindings = new Dictionary<string, IList<PortBinding>>
			{
				{"5000", new List<PortBinding> {new PortBinding {HostPort = "5000"}}}
			},
		PublishAllPorts = true
	}
;

	// Create the container
	var response = client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
	{
		Image = $"{imagerepo}/{imageName}:{imagetag}",
				Name = containername,
				Tty = false,
				HostConfig = hostConfig,
			}).Result;

	Console.WriteLine($"Container ID: {response.ID}\n\tContainer warnings: '{string.Join(", ", response.Warnings)}'.");

}

// You can define other methods, fields, classes and namespaces here
