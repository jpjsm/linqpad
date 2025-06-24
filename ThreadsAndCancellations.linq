<Query Kind="Program">
  <Output>DataGrids</Output>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>



void Main()
{
	TaskStatus CompletionState = TaskStatus.RanToCompletion|TaskStatus.Faulted|TaskStatus.Canceled;
	TaskStatus ActiveState = ~CompletionState;
	
	Random rand = new Random();
	DoSomething doSomething = new DoSomething();
	CancellationTokenSource cts = new CancellationTokenSource();
	CancellationToken ct = cts.Token;
	Task doIt = new Task(doSomething.DoSomethingTask, ct, TaskCreationOptions.LongRunning|TaskCreationOptions.DenyChildAttach);
	doIt.Start();
	Thread.Sleep(2000);
	Console.WriteLine($"1.- Current sum value: {doSomething.Sum:N0}");
	
	cts.Cancel();
	Thread.Sleep(500);
	Console.WriteLine($"2.- Task status: {doIt.Status}");
	Console.WriteLine($"3.- Current sum value: {doSomething.Sum:N0}");

	//doSomething.KeepGoing =false;
	//doIt.Wait(100);
	
	if ((doIt.Status & CompletionState) != 0)
	{
		Console.WriteLine($"Disposing Task: {doIt.Exception.Message}");
		doIt.Dispose();
	}
	Thread.Sleep(500);
	Console.WriteLine($"Current sum value: {doSomething.Sum:N0}");

	if (doIt != null)
	{
		Console.WriteLine($"Task status: {doIt.Status}");
		Console.WriteLine($"Exception: {doIt.Exception}");
	}
	
	Console.WriteLine("Done!");
}

// You can define other methods, fields, classes and namespaces here
public class DoSomething
{
	private long sum = 0;
	
	public bool KeepGoing{get;set;} = true;
	public long Sum => sum;
	
	public void DoSomethingTask()
	{
		Random rand = new Random();
		
		while (KeepGoing)
		{
			sum += rand.Next(100);
			Thread.Sleep(50);
			
			if (sum > 1000)
			{
				throw new ApplicationException($"Time to stop. Sum: {sum}");
			}
		}	
	}
}