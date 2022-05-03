using Polly;

namespace Microlation;

/// <summary>
///     Defines various options for the call of a Route.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CallOptions<T>
{
	/// <summary>
	///     Policies that are used for the call.
	/// </summary>
	public ISyncPolicy<T>? Policies { get; set; }

	/// <summary>
	///     Route that will be called.
	/// </summary>
	public string Route { get; set; }

	/// <summary>
	///     Function that returns the interval based on the iteration.
	/// </summary>
	public Func<int, TimeSpan> Interval { get; set; }
}