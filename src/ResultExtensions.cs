using System;

namespace Rasberry.Cli;

///<summary>Extensions for using <c>Result</> in boolean statements</summary>
public static class ResultExtensions
{
	///<summary>Returns true if the <c>Result</c> is Good</summary>
	public static bool IsGood(this ParseParams.Result r)
	{
		return r == ParseParams.Result.Good;
	}

	///<summary>Returns true if the <c>Result</c> is not Good</summary>
	public static bool IsBad(this ParseParams.Result r)
	{
		return r != ParseParams.Result.Good;
	}

	///<summary>Returns true if the <c>Result</c> is MissingArgument</summary>
	public static bool IsMissingArgument(this ParseParams.Result r)
	{
		return r == ParseParams.Result.MissingArgument;
	}

	///<summary>Returns true if the <c>Result</c> is UnParsable</summary>
	public static bool IsUnParsable(this ParseParams.Result r)
	{
		return r == ParseParams.Result.UnParsable;
	}

	///<summary>Returns true if the <c>Result</c> is Missing</summary>
	public static bool IsMissing(this ParseParams.Result r)
	{
		return r == ParseParams.Result.Missing;
	}

	///<summary>Returns true if the <c>Result</c> is MissingArgument or UnParsable</summary>
	public static bool IsInvalid(this ParseParams.Result r)
	{
		return IsMissingArgument(r) || IsUnParsable(r);
	}
}