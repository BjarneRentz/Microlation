using Polly;

namespace Microlation;

public class CallOptions<T>
{
	public ISyncPolicy<T> Policys { get; set; }
	public string Route { get; set; }
	
	public Func<int, TimeSpan> Interval { get; set; }
}