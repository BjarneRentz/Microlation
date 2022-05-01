namespace Microlation;

public interface ICall
{
	/// <summary>
	///     Calling Microservice.
	/// </summary>
	public Microservice Source { get; init; }

	/// <summary>
	///     Called Microserice.
	/// </summary>
	public Microservice Destination { get; init; }

	/// <summary>
	///     Route of the Call.
	/// </summary>
	public IRoute Route { get; }

	/// <summary>
	///     String representation of the source, destination and route of the call.
	/// </summary>
	public string CallUri => Source.Name + "-->" + Destination.Name + "/" + Route.Url;

	/// <summary>
	///     Performs the call.
	/// </summary>
	/// <param name="cancellationToken">Token to cancel the task.</param>
	/// <param name="iteration">Iteration of the simulation. Used to vary the duration between calls.</param>
	/// <returns>Result of the call.</returns>
	public Task<CallResult> Execute(CancellationToken cancellationToken, int iteration = 0);
}