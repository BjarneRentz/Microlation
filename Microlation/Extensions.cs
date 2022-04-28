namespace Microlation;

public static class Extensions
{
	public static Call<T> Validate<T>(this Call<T> call, Func<T, bool> predicate)
	{
		call.Validators.Add(predicate);
		return call;
	}
}