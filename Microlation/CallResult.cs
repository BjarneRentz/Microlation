namespace Microlation;

public record CallResult<T>
{
	public T? Result;
	public Exception? Exception;
	public bool Valid;

	public TimeSpan CallDuration;
}