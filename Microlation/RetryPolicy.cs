namespace Microlation;

public class RetryPolicy<T> : IPolicy<T> where T : new()
{
	public T Execute()
	{
		return new T();
	}
}