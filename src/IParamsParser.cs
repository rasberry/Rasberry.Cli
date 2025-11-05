using System;

namespace Rasberry.Cli;

///<summary>
/// Interface for parameter parsing class
/// <c>TryParse</c> should never throw an exception, however, you loose any error messages as a result.
/// Use <c>Parse</c> when you want to capture the error message contained in any resulting exception.
///</summary>
public interface IParamsParser
{
	///<summary>Attempts to parse a string into a value</summary>
	///<param name="inp">The input string</param>
	///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
	///<returns>The output value</returns>
	T Parse<T>(string inp);

	///<summary>Attempts to parse a string into a value</summary>
	///<param name="type">The output Type</param>
	///<param name="inp">The input string</param>
	///<returns>The output value</returns>
	object Parse(Type type, string inp);
}
