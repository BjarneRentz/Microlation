using System.Diagnostics;
using Polly;

namespace Microlation;

public class Call<T> : ICall
{
	private readonly Stopwatch watch = new();

	/// <summary>
	///     Generic version of the route.
	/// </summary>
	public Route<T> TypedRoute;

	/// <summary>
	///     Functions that allow custom result validation.
	/// </summary>
	/// <remarks>
	///     Can be added easily with the fluent <see cref="Extensions.Validate{T}" /> extension method.
	/// </remarks>
	public List<Func<T, bool>> Validators { get; } = new();

	/// <summary>
	///     Options of the call.
	/// </summary>
	public CallOptions<T> CallOptions { get; init; }

	/// <inheritdoc cref="ICall.Source" />
	public Microservice Source { get; init; }

	/// <inheritdoc cref="ICall.Destination" />
	public Microservice Destination { get; init; }

	/// <inheritdoc cref="ICall.Route" />
	public IRoute Route => TypedRoute;

	/// <inheritdoc cref="ICall.Execute" />
	public async Task<CallResult> Execute(CancellationToken token, int iteration = 0)
	{
		await Task.Delay(CallOptions.Interval(iteration), token);

		watch.Reset();
		watch.Start();

		// Build the callChain based on the provided policies and faults.
		ISyncPolicy<T>? callChain = null;
		if(CallOptions.Policies != null)
			callChain = CallOptions.Policies;

		if (TypedRoute.Faults != null)
			callChain = callChain == null ? TypedRoute.Faults : Policy.Wrap(callChain, TypedRoute.Faults);


		
		var result = new CallResult();

		try
		{
			var value = callChain != null ? callChain.Execute(() => TypedRoute.Value()) : TypedRoute.Value();
			watch.Stop();
			result.CallDuration = watch.Elapsed;

			result.Valid = Validators.Any() && Validators.Aggregate(true, (curr, next) => curr && next(value));
		}
		catch (Exception e)
		{
			watch.Stop();
			result.Exception = e;
			result.CallDuration = watch.Elapsed;
		}

		return result;
	}
}