namespace Microlation;

public class Simulation
{
	public List<Microservice> Microservices { get; private set; } = new();
	
	public async Task<Dictionary<ICall, List<CallResult>>> Run(TimeSpan duration)
	{
		
		var tasks = Microservices.Select(m => m.Simulate(duration));

		var result = await Task.WhenAll(tasks);

		return result
			.SelectMany(dict => dict)
			.ToDictionary(g => g.Key, g => g.Value);

	}
}