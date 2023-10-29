using System;

namespace Rasberry.Cli;

/// <summary>
/// Contains the result of a parameter check function
/// </summary>
/// <typeparam name="T">Type of the parameter</typeparam>
public readonly struct ParseResult<T>
{
	public ParseResult(ParseParams.Result result, string name, T value, Exception error = null)
	{
		Result = result;
		Name = name;
		Value = value;
		Error = error;
	}

	/// <summary>Result of the Operation</summary>
	public ParseParams.Result Result { get; }

	/// <summary>Name of the parameter - may be null if multiple names are provided</summary>
	public string Name { get; }

	/// <summary>Value of the parameter - may be default if operation failed</summary>
	public T Value { get; }

	/// <summary>Captures any errors thrown by the parser</summary>
	public Exception Error { get; }
}
