namespace Microlation;

public static class Extensions
{
	/// <summary>
	/// Adds a new validator to the call to validate the result
	/// </summary>
	/// <param name="call">Call to which the validator will be added.</param>
	/// <param name="predicate">Determines weather the result is valid (true) or invalid (false).</param>
	/// <typeparam name="T">Return type of the <see cref="Route{T}"/></typeparam>
	/// <returns>The Call with added predicate.</returns>
	public static Call<T> Validate<T>(this Call<T> call, Func<T, bool> predicate)
	{
		call.Validators.Add(predicate);
		return call;
	}
}