namespace Microlation;

public interface IPolicy<T>
{
	public T Execute();
}