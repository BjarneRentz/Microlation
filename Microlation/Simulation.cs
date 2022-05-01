namespace Microlation;

/// <summary>
///     Represents a complete model to simulate.
/// </summary>
public class Simulation
{
	/// <summary>
	///     <see cref="Microservice" />s that are part of the simulation.
	/// </summary>
	public List<Microservice> Microservices { get; } = new();


	/// <summary>
	///     Simulates the set of Microservices for the given duration.
	/// </summary>
	/// <param name="duration">Of the simulation.</param>
	/// <returns>
	///     The results of each call (<see cref="CallResult" />) grouped by the representing <see cref="ICall" />
	/// </returns>
	public async Task<Dictionary<ICall, List<CallResult>>> Run(TimeSpan duration)
	{
		var tasks = Microservices.Select(m => m.Simulate(duration));

		var result = await Task.WhenAll(tasks);

		return result
			.SelectMany(dict => dict)
			.ToDictionary(g => g.Key, g => g.Value);
	}
}