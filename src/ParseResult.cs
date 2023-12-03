using System;

namespace Rasberry.Cli;

/// <summary>
/// Contains the result of a parameter check function
/// </summary>
/// <typeparam name="T">Type of the parameter</typeparam>
public readonly struct ParseResult<T>
{
	/// <summary>
	/// Constructor for ParseResult
	/// </summary>
	/// <param name="result">Result of the Operation</param>
	/// <param name="name">Name of the parameter</param>
	/// <param name="value">Value of the parameter</param>
	/// <param name="error">Error that may thrown by the parser</param>
	public ParseResult(ParseParams.Result result, string name, T value, Exception error = null)
	{
		Result = result;
		Name = name;
		Value = value;
		Error = error;
	}

	/// <summary>Result of the Operation</summary>
	public ParseParams.Result Result { get; init; }

	/// <summary>Name of the parameter</summary>
	public string Name { get; init; }

	/// <summary>Value of the parameter</summary>
	public T Value { get; init; }

	/// <summary>Captures any error thrown by the parser</summary>
	public Exception Error { get; init; }
}
