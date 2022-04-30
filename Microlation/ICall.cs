namespace Microlation;

public interface ICall
{
	public Microservice Microservice { get; init; }
	
	public IRoute Route { get; }
	
	public Task<CallResult> Execute(CancellationToken cancellationToken, int iteration = 0);

	public string CallUri => Microservice.Name + "/" + Route.Url;
}