namespace Microlation;

/// <summary>
/// Interface of Routes primarily used for internal handling of <see cref="Route{T}"/>.
/// </summary>
public interface IRoute
{
	/// <summary>
	/// Url of the Route. Must be unique.
	/// </summary>
	string Url { get; set; }
}