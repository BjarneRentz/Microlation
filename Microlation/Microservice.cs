namespace Microlation;

public class Microservice
{
	public List<IRoute> Routes = new();


	public Call<T> Call<T>(Microservice ms, CallOptions<T> callOptions)
	{
		var route = ms.Routes.First(r => r.Url == callOptions.Route);

		return new Call<T>
		{
			Policy = callOptions.Policys
		};
	}
}