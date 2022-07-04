using Microlation;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace Examples;

public class RetryExample
{
	public async Task Run(string[] args)
	{
		int[] retryCounts = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

		var caller = new Microservice("Caller");
		var target = new Microservice("Target")
		{
			Routes =
			{
				new Route<int>
				{
					Url = "Target", Value = () => 1,
					Faults = MonkeyPolicy.InjectException(with => with.Fault(new Exception())
							.InjectionRate(0.1)
							.Enabled())
						.AsPolicy<int>()
				}
			}
		};

		var call = caller.Call(target, new CallOptions<int>
		{
			Route = "Target",
			Interval = _ => TimeSpan.FromMilliseconds(500)
		});

		var simulation = new Simulation
		{
			Microservices =
			{
				caller,
				target
			}
		};

		var results = new Dictionary<int, List<CallResult>>();
		foreach (var retryCount in retryCounts)
		{
			call.CallOptions.Policies = Policy<int>.Handle<Exception>().Retry(retryCount);
			var result = await simulation.Run(TimeSpan.FromSeconds(60));
			results.Add(retryCount, result.First().Value);
		}


		Console.WriteLine("{0,20}| {1,20}, {2,20}", "Retry Count", "Avg Connection Time", "# Errors");
		foreach (var keyValuePair in results)
			Console.WriteLine("{0,20}| {1,20}| {2,20}", keyValuePair.Key,
				keyValuePair.Value.Average(r => r.CallDuration.TotalMilliseconds),
				keyValuePair.Value.Count(r => r.Exception != null));
	}
}