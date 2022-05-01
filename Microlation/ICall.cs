namespace Microlation;

public interface ICall
{
	public Microservice Source { get; init; }
	public Microservice Destination { get; init; }
	
	public IRoute Route { get; }
	
	public Task<CallResult> Execute(CancellationToken cancellationToken, int iteration = 0);

	public string CallUri => Source.Name + "-->" +  Destination.Name + "/" + Route.Url;
}