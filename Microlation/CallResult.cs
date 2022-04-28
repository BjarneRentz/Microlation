namespace Microlation;

public record CallResult<T>
{
	public T? Result;
	public bool Valid;
}