namespace Microlation;

/// <summary>
/// Represent the result of a single call.
/// </summary>
/// <typeparam name="T">Return type of the called <see cref="Route{T}"/>.</typeparam>
public record CallResult<T>
{
	/// <summary>
	/// Result / value of the call.
	/// </summary>
	public T? Result;
	
	/// <summary>
	/// Exception that was thrown during the call.
	/// </summary>
	public Exception? Exception;
	
	/// <summary>
	/// Weather the result is valid. If no Validators were defined, the value is false.
	/// </summary>
	public bool Valid;

	/// <summary>
	/// Duration the call took to complete.
	/// </summary>
	public TimeSpan CallDuration;
}