using System;

namespace Rasberry.Cli;

///<summary>Extensions for using <c>Result</> in boolean statements</summary>
public static class ResultExtensions
{
	///<summary>Returns true if the <c>Result</c> is Good</summary>
	public static bool IsGood<T>(this ParseResult<T> r)
	{
		return r.Result == ParseParams.Result.Good;
	}

	///<summary>Returns true if the <c>Result</c> is not Good</summary>
	public static bool IsBad<T>(this ParseResult<T> r)
	{
		return r.Result != ParseParams.Result.Good;
	}

	///<summary>Returns true if the <c>Result</c> is MissingArgument</summary>
	public static bool IsMissingArgument<T>(this ParseResult<T> r)
	{
		return r.Result == ParseParams.Result.MissingArgument;
	}

	///<summary>Returns true if the <c>Result</c> is UnParsable</summary>
	public static bool IsUnParsable<T>(this ParseResult<T> r)
	{
		return r.Result == ParseParams.Result.UnParsable;
	}

	///<summary>Returns true if the <c>Result</c> is Missing</summary>
	public static bool IsMissing<T>(this ParseResult<T> r)
	{
		return r.Result == ParseParams.Result.Missing;
	}

	///<summary>Returns true if the <c>Result</c> is MissingArgument or UnParsable</summary>
	public static bool IsInvalid<T>(this ParseResult<T> r)
	{
		return IsMissingArgument(r) || IsUnParsable(r);
	}

	///<summary>Runs given function if result <c>IsGood</c></summary>
	public static ParseResult<T> WhenGood<T>(this ParseResult<T> r, Func<ParseResult<T>,ParseResult<T>> action = null)
	{
		if (action != null && r.IsGood()) {
			return action(r);
		}
		return r;
	}

	///<summary>Runs given function if result <c>IsBad</c></summary>
	public static ParseResult<T> WhenBad<T>(this ParseResult<T> r, Func<ParseResult<T>,ParseResult<T>> action = null)
	{
		if (action != null && r.IsBad()) {
			return action(r);
		}
		return r;
	}

	///<summary>Runs given function if result <c>IsMissingArgument</c></summary>
	public static ParseResult<T> WhenMissingArgument<T>(this ParseResult<T> r, Func<ParseResult<T>,ParseResult<T>> action = null)
	{
		if (action != null && r.IsMissingArgument()) {
			return action(r);
		}
		return r;
	}

	///<summary>Runs given function if result <c>IsUnParsable</c></summary>
	public static ParseResult<T> WhenUnParsable<T>(this ParseResult<T> r, Func<ParseResult<T>,ParseResult<T>> action = null)
	{
		if (action != null && r.IsUnParsable()) {
			return action(r);
		}
		return r;
	}

	///<summary>Runs given function if result <c>IsMissing</c></summary>
	public static ParseResult<T> WhenMissing<T>(this ParseResult<T> r, Func<ParseResult<T>,ParseResult<T>> action = null)
	{
		if (action != null && r.IsMissing()) {
			return action(r);
		}
		return r;
	}

	///<summary>Runs given function if result <c>IsInvalid</c></summary>
	public static ParseResult<T> WhenInvalid<T>(this ParseResult<T> r, Func<ParseResult<T>,ParseResult<T>> action = null)
	{
		if (action != null && r.IsInvalid()) {
			return action(r);
		}
		return r;
	}
}