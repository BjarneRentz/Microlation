using System.Diagnostics;
using Polly;

namespace Microlation;

public class Call<T>
{

	private readonly Stopwatch watch = new();
	
	public List<Func<T, bool>> Validators { get; } = new();

	public ISyncPolicy<T> Policies { private get; init; }

	public Route<T> Route;

	public CallResult<T> Execute()
	{
		watch.Reset();
		watch.Start();
		var callChain = Policies;
		
		if (Route.Faults != null)
		{
			callChain = Policy.Wrap<T>(Policies, Route.Faults);
		}

		var result = new CallResult<T>();
		
		try
		{
			var value = callChain.Execute(() => Route.Value());
			watch.Stop();
			result.Result = value;
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