namespace Microlation;

public class Call<T>
{
	public List<Func<T, bool>> Validators { get; } = new();

	public IPolicy<T> Policy { private get; init; }

	public CallResult<T> Execute()
	{
		var result = Policy.Execute();
		var valid = false;
		if (Validators.Any()) valid = Validators.Aggregate(true, (curr, next) => curr && next(result));

		return new CallResult<T>
		{
			Result = result,
			Valid = valid
		};
	}
}