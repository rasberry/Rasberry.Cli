using System;
using System.Collections.Generic;
using System.Linq;

namespace Rasberry.Cli;

///<summary>Tools for parsing the command line</summary>
public sealed class ParseParams
{
	/// <summary>Possible results from an operation</summary>
	public enum Result {
		///<summary>Parameter is usable</summary>
		Good = 0,
		///<summary>Parameter was not found</summary>
		Missing = 1,
		///<summary>Parameter was not able to be parsed</summary>
		UnParsable = 2,
		///<summary>Parameter was missing a required argument</summary>
		MissingArgument = 3
	}

	///<summary>Template for a custom value parser</summary>
	public delegate T Parser<T>(string inp);

	///<summary>Constructor for Params class</summary>
	///<param name="args">String array of arguments that have already been seperated</param>
	public ParseParams(string[] args) : this (args,null)
	{
	}

	/// <summary>Constructor for Params using a custom parser</summary>
	///<param name="customParser">A custom parser instance</param>
	public ParseParams(string[] args, IParamsParser customParser)
	{
		Args = new List<string>(args);
		ParserInst = customParser ?? new DefaultParser();
	}

	// Internal storage for arguments
	readonly List<string> Args;

	///<summary>The list of remaining non-processed argument items</summary>
	///<returns>String array of remaining non-processed arguments</returns>
	public string[] Remaining()
	{
		return Args.ToArray();
	}

	///<summary>Checks for existence a single or multi-named parameter</summary>
	///<param name="switch">One or more arguments to check for existence</param>
	///<returns><c>ParseResult</c> value</returns>
	public ParseResult<bool> Has(params string[] @switch)
	{
		int ii = -1;
		string name = null;
		foreach(string sw in @switch) {
			int i = Args.IndexOf(sw);
			if (i != -1) {
				Args.RemoveAt(i);
				ii = i;
				name = sw;
			}
		}
		return new ParseResult<bool>(
			result: ii == -1 ? Result.Missing : Result.Good,
			name: name ?? (@switch.Length > 0 ? @switch[0] : null),
			value: ii != -1
		);
	}

	///<summary>
	/// Tries to parse a value without an accompanying parameter
	/// The result Error property may be set if parsing failed
	///</summary>
	///<param name="def">An optional default value used if parsing fails</param>
	///<param name="par">An optional custom parser to be used on the value</param>
	///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
	///<returns><c>ParseResult</c> value</returns>
	public ParseResult<T> Value<T>(T def = default,Parser<T> par = null)
	{
		T val = def;
		if (Args.Count <= 0) {
			return new ParseResult<T>(Result.Missing, null, val);
		}
		string curr = Args[0];
		if (par == null) { par = ParserInst.Parse<T>; }

		Exception err;
		(val, err) = ParseAndCapture(curr, par ?? ParserInst.Parse<T>, def);

		if (err != null) {
			return new ParseResult<T>(Result.UnParsable, null, val, err);
		}
		Args.RemoveAt(0);
		return new ParseResult<T>(Result.Good, null, val);
	}

	///<summary>
	/// Tries to parse a parameter with a single value
	/// The result Error property may be set if parsing failed
	///</summary>
	///<param name="switch">The parameter name</param>
	///<param name="def">An optional default value used if parsing fails</param>
	///<param name="par">An optional custom parser to be used on the value</param>
	///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
	///<returns><c>ParseResult</c> value</returns>
	public ParseResult<T> Scan<T>(string @switch,T def = default,Parser<T> par = null)
	{
		T val = def;
		int i = Args.IndexOf(@switch);
		if (i == -1) {
			return new ParseResult<T>(Result.Missing, @switch, val);
		}
		if (i+1 >= Args.Count) {
			return new ParseResult<T>(Result.MissingArgument, @switch, val);
		}

		Exception err;
		(val, err) = ParseAndCapture(Args[i+1], par ?? ParserInst.Parse<T>, def);

		if (err != null) {
			return new ParseResult<T>(Result.UnParsable, @switch, val, err);
		}

		Args.RemoveAt(i+1);
		Args.RemoveAt(i);
		return new ParseResult<T>(Result.Good, @switch, val);
	}

	///<summary>
	/// Tries to parse a multi-named parameter with a single value
	/// The result Error property may be set if parsing failed
	/// The result Name may be null if multiple parameter names were given
	///</summary>
	///<param name="switch">The parameter name(s)</param>
	///<param name="val">The output value</param>
	///<param name="def">An optional default value used if parsing fails</param>
	///<param name="par">An optional custom parser to be used on the value</param>
	///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
	///<returns><c>ParseResult</c> value</returns>
	public ParseResult<T> Scan<T>(string[] @switch,T def = default,Parser<T> par = null)
	{
		foreach(string sw in @switch) {
			var r = Scan<T>(sw,def,par);
			// Log.Debug($"Default sw={sw} r={r} val={val}");
			if (r.Result == Result.MissingArgument ||
				r.Result == Result.UnParsable ||
				r.Result == Result.Good
			) {
				return new ParseResult<T>(r.Result, sw, r.Value, r.Error);
			}
		}
		string name = @switch.Length > 0 ? @switch[0] : null;
		return new ParseResult<T>(Result.Missing, name, default);
	}

	///<summary>
	/// Tries to parse a parameter with two values
	/// The result Error property may be set if parsing failed
	///</summary>
	///<param name="switch">The parameter name</param>
	///<param name="leftVal">The first output value</param>
	///<param name="rightVal">The second output value</param>
	///<param name="leftDef">An optional default value used if parsing fails</param>
	///<param name="rightDef">An optional default value used if parsing fails</param>
	///<param name="condition">A condition function that determines when second argument is required (defaults to always true)</param>
	///<param name="leftPar">An optional custom parser to be used on the first value</param>
	///<param name="rightPar">An optional custom parser to be used on the second value</param>
	///<typeparam name="T">The output type of the first value attempting to be parsed</typeparam>
	///<typeparam name="U">The output type of the second value attempting to be parsed</typeparam>
	///<returns><c>ParseResult</c> value</returns>
	public ParseResult<(T,U)> Scan<T,U>(string @switch,
		T leftDef = default, U rightDef = default, Func<T,bool> condition = null,
		Parser<T> leftPar = null, Parser<U> rightPar = null)
	{
		T leftVal = leftDef;
		U rightVal = rightDef;
		int i = Args.IndexOf(@switch);
		if (i == -1) {
			return new ParseResult<(T,U)>(Result.Missing, @switch, (leftVal,rightVal));
		}
		if (i+1 >= Args.Count) {
			return new ParseResult<(T,U)>(Result.MissingArgument, @switch, (leftVal,rightVal));
		}

		Exception leftErr;
		(leftVal, leftErr) = ParseAndCapture(Args[i+1], leftPar ?? ParserInst.Parse<T>, leftDef);

		if (leftErr != null) {
			return new ParseResult<(T,U)>(Result.UnParsable, @switch, (leftVal,rightVal), leftErr);
		}

		//if condition function returns false - we don't look for a second arg
		if (condition != null && !condition(leftVal)) {
			Args.RemoveAt(i+1);
			Args.RemoveAt(i);
			return new ParseResult<(T,U)>(Result.Good, @switch, (leftVal,rightVal));
		}

		if (i+2 >= Args.Count) {
			return new ParseResult<(T,U)>(Result.MissingArgument, @switch, (leftVal,rightVal));
		}

		Exception rightErr;
		(rightVal, rightErr) = ParseAndCapture(Args[i+2], rightPar ?? ParserInst.Parse<U>, rightDef);

		if (rightErr != null) {
			return new ParseResult<(T,U)>(Result.UnParsable, @switch, (leftVal,rightVal), rightErr);
		}

		Args.RemoveAt(i+2);
		Args.RemoveAt(i+1);
		Args.RemoveAt(i);
		return new ParseResult<(T,U)>(Result.Good, @switch, (leftVal,rightVal));
	}

	///<summary>
	/// Tries to parse a parameter with two values
	/// The result Error property may be set if parsing failed
	/// The result Name may be null if multiple parameter names were given
	///</summary>
	///<param name="switch">The parameter name(s)</param>
	///<param name="leftVal">The first output value</param>
	///<param name="rightVal">The second output value</param>
	///<param name="leftDef">An optional default value used if parsing fails</param>
	///<param name="rightDef">An optional default value used if parsing fails</param>
	///<param name="condition">A condition function that determines when second argument is required (defaults to always true)</param>
	///<param name="leftPar">An optional custom parser to be used on the first value</param>
	///<param name="rightPar">An optional custom parser to be used on the second value</param>
	///<typeparam name="T">The output type of the first value attempting to be parsed</typeparam>
	///<typeparam name="U">The output type of the second value attempting to be parsed</typeparam>
	///<returns><c>ParseResult</c> value</returns>
	public ParseResult<(T,U)> Scan<T,U>(string[] @switch,
		T leftDef = default, U rightDef = default, Func<T,bool> condition = null,
		Parser<T> leftPar = null, Parser<U> rightPar = null)
	{
		foreach(string sw in @switch) {
			var r = Scan<T,U>(sw,leftDef,rightDef,condition,leftPar,rightPar);
			// Log.Debug($"Default sw={sw} r={r} val={val}");
			if (r.Result == Result.MissingArgument ||
				r.Result == Result.UnParsable ||
				r.Result == Result.Good
			) {
				return new ParseResult<(T,U)>(r.Result, sw, r.Value, r.Error);
			}
		}
		string name = @switch.Length > 0 ? @switch[0] : null;
		return new ParseResult<(T,U)>(Result.Missing, name, (default,default));
	}

	//private parser instance
	readonly IParamsParser ParserInst;

	//helper for running the parser and capturing any errors
	static (T,Exception) ParseAndCapture<T>(string sub, Parser<T> par, T def = default)
	{
		try {
			return (par(sub), null);
		}
		catch(Exception e) {
			return (def, e);
		}
	}
}