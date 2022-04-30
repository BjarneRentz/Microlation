namespace Microlation;

/// <summary>
/// Represent the result of a single call.
/// </summary>
public record CallResult
{
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