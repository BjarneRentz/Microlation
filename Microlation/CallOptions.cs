namespace Microlation;

public class CallOptions<T>
{
	public IPolicy<T> Policys;
	public string Route { get; set; }
}