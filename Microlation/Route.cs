using Polly;

namespace Microlation;

/// <summary>
/// Defines a <see cref="Microservice"/> route.
/// </summary>
/// <typeparam name="T">Return type of value that is returned when the route is called.</typeparam>
public class Route<T> : IRoute
{
	/// <summary>
	/// Function that generates the return value of the route.
	/// </summary>
	public Func<T> Value;

	/// <summary>
	/// Possible Faults of the route, that will be executed on call.
	/// </summary>
	public ISyncPolicy<T>? Faults;
	
	///<inheritdoc cref="IRoute.Url"/>
	public string Url { get; set; }
}