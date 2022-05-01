namespace Microlation;

public interface ICall
{
	public Microservice Source { get; init; }
	public Microservice Destination { get; init; }

	public IRoute Route { get; }

	public string CallUri => Source.Name + "-->" + Destination.Name + "/" + Route.Url;

	public Task<CallResult> Execute(CancellationToken cancellationToken, int iteration = 0);
}