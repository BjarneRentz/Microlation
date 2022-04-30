namespace Microlation;

public interface ICall
{
	public Task<CallResult> Execute(CancellationToken cancellationToken, int iteration = 0);
}