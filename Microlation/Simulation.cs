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
		ShowState(duration);
		
		var tasks = Microservices.Select(m => m.Simulate(duration));

		var result = await Task.WhenAll(tasks);

		return result
			.SelectMany(dict => dict)
			.ToDictionary(g => g.Key, g => g.Value);
	}

	private async Task ShowState(TimeSpan duration)
	{
		var remainingTime = duration;

		// Stop state output one Second before further output happens to ensure no useful output gets cleared.
		while (remainingTime > TimeSpan.FromSeconds(1))
		{
			Console.Write("\rSimulation finished in {0}", remainingTime);
			await Task.Delay(1000);
			remainingTime -= TimeSpan.FromSeconds(1);
		}
		Console.Clear();
		Console.WriteLine("Finished simulation!");
	}
}