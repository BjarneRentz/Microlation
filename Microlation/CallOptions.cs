using Polly;

namespace Microlation;

public class CallOptions<T>
{
	public ISyncPolicy<T> Policys;
	public string Route { get; set; }
}