using Polly;

namespace Microlation;

public class Route<T> : IRoute
{
	public Func<T> Value;

	public ISyncPolicy<T>? Faults;
	public string Url { get; set; }
}