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

	public Call<T> Call<T>(Microservice ms, CallOptions<T> callOptions)
	{
		var route = ms.Routes.First(r => r.Url == callOptions.Route) ?? throw new ArgumentException(string.Format("Could not find Route: {0} on given Microservice", callOptions.Route));
		
		return new Call<T>
		{
			CallOptions = callOptions,
			Route = (Route<T>) route,
		};
	}
}