namespace Microlation;

/// <summary>
/// Represents a Microservices with defined <see cref="Route{T}"/>s.
/// </summary>
public class Microservice
{
	/// <summary>
	/// <see cref="Route{T}"/>s of the Microservice. Can be used by other Microservices to perform calls.
	/// </summary>
	public List<IRoute> Routes = new();

	private readonly List<ICall> calls = new();

	private readonly CancellationTokenSource ctSource = new();

	public Call<T> Call<T>(Microservice ms, CallOptions<T> callOptions)
	{
		var route = ms.Routes.First(r => r.Url == callOptions.Route) ?? throw new ArgumentException(string.Format("Could not find Route: {0} on given Microservice", callOptions.Route));
		
		var call =  new Call<T>
		{
			CallOptions = callOptions,
			Route = (Route<T>) route,
		};
		
		calls.Add(call);

		return call;
	}

	public async Task<List<CallResult>[]> Simulate(TimeSpan duration)
	{
		ctSource.CancelAfter(duration);
		var tasks = calls.Select(c => PerformCall(c, ctSource.Token));

		var results = await Task.WhenAll(tasks);

		return results;

	}

	private async Task<List<CallResult>> PerformCall(ICall call,  CancellationToken cancellationToken)
	{
		List<CallResult> callResults = new();
		
		var iteration = 0;
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				var result = await call.Execute(cancellationToken, iteration);
				callResults.Add(result);
			}
			catch (TaskCanceledException)
			{
				return callResults;
			}

			iteration++;
		}

		return callResults;
	}
}