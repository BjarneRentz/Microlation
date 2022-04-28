namespace Microlation;

public class Microservice
{
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