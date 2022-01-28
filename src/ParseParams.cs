using System;
using System.Collections.Generic;
using System.Linq;

namespace Rasberry.Cli
{
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

		//private parser instance
		readonly IParamsParser ParserInst;

		//wrapper for calling the IParamsParser so we can use it as a delegate
		bool TryParse<T>(string inp, out T val)
		{
			return ParserInst.TryParse<T>(inp,out val);
		}

		///<summary>Template for a custom value parser</summary>
		public delegate bool Parser<T>(string inp, out T val);

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
		///<returns><c>Result</c> value</returns>
		public Result Has(params string[] @switch)
		{
			int ii = -1;
			foreach(string sw in @switch) {
				int i = Args.IndexOf(sw);
				if (i != -1) {
					Args.RemoveAt(i);
					ii = i;
				}
			}
			return ii == -1 ? Result.Missing : Result.Good;
		}

		///<summary>Tries to parse a value without an accompanying parameter</summary>
		///<param name="val">The output value</param>
		///<param name="def">An optional default value used if parsing fails</param>
		///<param name="par">An optional custom parser to be used on the value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Default<T>(out T val, T def = default,Parser<T> par = null)
		{
			val = def;
			if (Args.Count <= 0) {
				return Result.Missing;
			}
			string curr = Args[0];
			if (par == null) { par = TryParse; }
			if (!par(curr,out val)) {
				return Result.UnParsable;
			}
			Args.RemoveAt(0);
			return Result.Good;
		}

		///<summary>Tries to parse a parameter with a single value</summary>
		///<param name="switch">The parameter</param>
		///<param name="val">The output value</param>
		///<param name="def">An optional default value used if parsing fails</param>
		///<param name="par">An optional custom parser to be used on the value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Default<T>(string @switch,out T val,T def = default,Parser<T> par = null)
		{
			val = def;
			int i = Args.IndexOf(@switch);
			if (i == -1) {
				return Result.Missing;
			}
			if (i+1 >= Args.Count) {
				return Result.MissingArgument;
			}
			if (par == null) { par = TryParse; }
			if (!par(Args[i+1],out val)) {
				return Result.UnParsable;
			}
			Args.RemoveAt(i+1);
			Args.RemoveAt(i);
			return Result.Good;
		}

		///<summary>Tries to parse a multi-named parameter with a single value</summary>
		///<param name="switch">The parameter</param>
		///<param name="val">The output value</param>
		///<param name="def">An optional default value used if parsing fails</param>
		///<param name="par">An optional custom parser to be used on the value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Default<T>(string[] @switch,out T val,T def = default,Parser<T> par = null)
		{
			val = default;
			foreach(string sw in @switch) {
				var r = Default<T>(sw,out val,def,par);
				// Log.Debug($"Default sw={sw} r={r} val={val}");
				if (r == Result.MissingArgument) { return r; }
				if (r == Result.UnParsable) { return r; }
				if (r == Result.Good) { return r; }
			}
			return Result.Missing;
		}

		///<summary>Tries to parse a parameter with two values</summary>
		///<param name="switch">The parameter</param>
		///<param name="tval">The first output value</param>
		///<param name="uval">The second output value</param>
		///<param name="tdef">An optional default value used if parsing fails</param>
		///<param name="udef">An optional default value used if parsing fails</param>
		///<param name="cond">A condition function that determines when second argument is required (defaults to always true)</param>
		///<param name="tpar">An optional custom parser to be used on the first value</param>
		///<param name="upar">An optional custom parser to be used on the second value</param>
		///<typeparam name="T">The output type of the first value attempting to be parsed</typeparam>
		///<typeparam name="U">The output type of the second value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Default<T,U>(string @switch,out T tval, out U uval,
			T tdef = default, U udef = default, Func<T,bool> Cond = null,
			Parser<T> tpar = null, Parser<U> upar = null)
		{
			tval = tdef;
			uval = udef;
			int i = Args.IndexOf(@switch);
			if (i == -1) {
				return Result.Missing;
			}
			if (i+1 >= Args.Count) {
				return Result.MissingArgument;
			}
			if (tpar == null) { tpar = TryParse; }
			if (!tpar(Args[i+1],out tval)) {
				return Result.UnParsable;
			}

			//if condition function returns false - we don't look for a second arg
			if (Cond != null && !Cond(tval)) {
				Args.RemoveAt(i+1);
				Args.RemoveAt(i);
				return Result.Good;
			}

			if (i+2 >= Args.Count) {
				return Result.MissingArgument;
			}
			if (upar == null) { upar = TryParse; }
			if (!upar(Args[i+2],out uval)) {
				return Result.UnParsable;
			}
			Args.RemoveAt(i+2);
			Args.RemoveAt(i+1);
			Args.RemoveAt(i);
			return Result.Good;
		}

		///<summary>Expects to parse a value without an accompanying parameter</summary>
		///<param name="val">The output value</param>
		///<param name="def">An optional default value used if parsing fails</param>
		///<param name="par">An optional custom parser to be used on the value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Expect<T>(out T val)
		{
			if (Result.Good != Default(out val)) {
				//turn Missing into MissingArgument
				return Result.MissingArgument;
			}
			return Result.Good;
		}

		///<summary>Expects to find the given parameter with no value</summary>
		///<param name="switch">The name of the parameter</param>
		///<returns><c>Result</c> value</returns>
		public Result Expect(string @switch)
		{
			var has = Has(@switch);
			if (Result.Good != has) {
				//turn Missing into MissingArgument
				return Result.MissingArgument;
			}
			return Result.Good;
		}

		///<summary>Expects to parse a parameter with a single value</summary>
		///<param name="switch">The name fo the parameter</param>
		///<param name="val">The output value</param>
		///<param name="par">An optional custom parser to be used on the value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Expect<T>(string @switch, out T val,Parser<T> par = null)
		{
			var has = Default(@switch,out val, par:par);
			if (Result.Good != has) {
				//turn Missing into MissingArgument
				return Result.MissingArgument;
			}
			return Result.Good;
		}

		///<summary>Expects to parse a parameter with two values</summary>
		///<param name="switch">The name of the parameter</param>
		///<param name="tval">The first output value</param>
		///<param name="uval">The second output value</param>
		///<param name="tpar">An optional custom parser to be used on the first value</param>
		///<param name="upar">An optional custom parser to be used on the second value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<typeparam name="U">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>Result</c> value</returns>
		public Result Expect<T,U>(string @switch, out T tval, out U uval,Parser<T> tpar = null,Parser<U> upar = null)
		{
			var has = Default(@switch,out tval,out uval, tpar:tpar, upar:upar);
			if (Result.Good != has) {
				//turn Missing into MissingArgument
				return Result.MissingArgument;
			}
			return Result.Good;
		}
	}
}