using System;

namespace Rasberry.Cli
{
	///<summary>Interface for parameter parsing class</summary>
	public interface IParamsParser
	{
		///<summary>Attempts to parse a string into a value</summary>
		///<param name="inp">The input string</param>
		///<param name="val">The output value</param>
		///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>bool</c> true if the parsing was successfull otherwise false</returns>
		bool TryParse<T>(string inp, out T val);
	}
}