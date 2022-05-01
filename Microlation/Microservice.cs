namespace Microlation;

/// <summary>
///     Represents a Microservices with defined <see cref="Route{T}" />s.
/// </summary>
public class Microservice
{
	private readonly List<ICall> calls = new();

	private readonly CancellationTokenSource ctSource = new();

	/// <summary>
	///     Name for representation purposes, <see cref="ICall.CallUri" />.
	/// </summary>
	public string Name;

	/// <summary>
	///     <see cref="Route{T}" />s of the Microservice. Can be used by other Microservices to perform calls.
	/// </summary>
	public List<IRoute> Routes = new();

	public Microservice(string name)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
	}

	/// <summary>
	///     Creates a new Call.
	/// </summary>
	/// <param name="ms"><see cref="Microservice" /> that gets called.</param>
	/// <param name="callOptions">Options of the call, among others the <see cref="Route{T}"/.></param>
	/// <typeparam name="T">Type of the <see cref="Route{T}" /> that gets called.</typeparam>
	/// <returns></returns>
	/// <exception cref="ArgumentException">Given Route is not valid.</exception>
	public Call<T> Call<T>(Microservice ms, CallOptions<T> callOptions)
	{
		var route = ms.Routes.First(r => r.Url == callOptions.Route) ??
		            throw new ArgumentException(string.Format("Could not find Route: {0} on given Microservice",
			            callOptions.Route));

		var call = new Call<T>
		{
			CallOptions = callOptions,
			TypedRoute = (Route<T>)route,
			Destination = ms,
			Source = this
		};

		calls.Add(call);

		return call;
	}

	/// <summary>
	///     Simulates the Microservice by executing the calls for the given duration.
	/// </summary>
	/// <param name="duration">Duration of the simulation.</param>
	/// <returns>The results of each call (<see cref="CallResult" />) grouped by the <see cref="ICall" />.</returns>
	public async Task<Dictionary<ICall, List<CallResult>>> Simulate(TimeSpan duration)
	{
		ctSource.CancelAfter(duration);
		// var tasks = calls.Select(c => PerformCall(c, ctSource.Token));
		//
		// var results = await Task.WhenAll(tasks);


		var res = new Dictionary<ICall, List<CallResult>>();

		var ts = calls.Select(c => Task.Run(async () =>
		{
			var callResults = await PerformCall(c, ctSource.Token);
			res.Add(c, callResults);
		}));

		await Task.WhenAll(ts);

		return res;
	}

	private async Task<List<CallResult>> PerformCall(ICall call, CancellationToken cancellationToken)
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