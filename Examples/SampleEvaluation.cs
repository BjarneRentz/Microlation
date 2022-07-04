using System.Collections.Concurrent;
using Microlation;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;
using Polly.Timeout;

namespace Examples;

public class SampleEvaluation
{
	public static async Task Run()
	{
		int[] timeouts = { 1000, 1500, 2000, 2500, 3000 };
		int[] retries = { 0, 1, 2, 3, 4 };

		var simulations = new List<Simulation>();

		var dictionary = new ConcurrentDictionary<(int Timeout, int Retries), List<CallResult>>();
		var tasks = new List<Task>();

		foreach (var timeout in timeouts)
		{
			foreach (var retry in retries)
				tasks.Add(Task.Run(async () =>
				{
					var result = await CreateSimulation(timeout, retry).Run(TimeSpan.FromMinutes(1));
					dictionary.TryAdd((timeout, retry), result.Values.First());
				}));
		}

		//var tasks = simulations.Select(s => s.Run(TimeSpan.FromMinutes(1)));
		await Task.WhenAll(tasks);

		await Task.Delay(2000);

		EvaluateResults(dictionary);

		Console.ReadKey();
	}

	private static void EvaluateResults(IDictionary<(int Timeout, int Retry), List<CallResult>> results)
	{
		Console.WriteLine("{0,20} | {1,20} | {2,20} | {3,20} | {4,20} | {5,20}", "Timeout", "# Retries", "# Calls" ,"Avg Time",
			"# Errors", "# Valid");

		foreach (var kvp in results.OrderBy(k => k.Key.Timeout).ThenBy(k => k.Key.Retry))
			Console.WriteLine("{0,20} | {1,20} | {2,20} | {3,20} | {4,20} | {5,20}", 
				kvp.Key.Timeout, kvp.Key.Retry,
				kvp.Value.Count,
				kvp.Value.Average(r => r.CallDuration.TotalMilliseconds), 
				kvp.Value.Count(r => r.Exception != null),
				kvp.Value.Count(r => r.Valid));
	}

	private static Simulation CreateSimulation(int timeout, int retries)
	{
		var faultPolicy = MonkeyPolicy
			.InjectLatency(with => with.Latency(TimeSpan.FromSeconds(2)).InjectionRate(0.1).Enabled())
			.Wrap(MonkeyPolicy.InjectResult<int>(with => with.Result(4).InjectionRate(0.1).Enabled()));

		var caller = new Microservice("Caller");
		var target = new Microservice("Target")
		{
			Routes = new List<IRoute>
			{
				new Route<int>
				{
					Url = "Target",
					Faults = faultPolicy,
					Value = () => 3
				}
			}
		};

		caller.Call(target, new CallOptions<int>
		{
			Route = "Target",
			Interval = _ => TimeSpan.FromMilliseconds(500),
			Policies = Policy<int>.Handle<TimeoutRejectedException>().Retry(retries)
				.Wrap(Policy.Timeout<int>(TimeSpan.FromMilliseconds(timeout)))
		}).Validate(value => value == 3);

		return new Simulation
		{
			Microservices = { caller, target }
		};
	}
}