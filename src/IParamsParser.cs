using System;

namespace Rasberry.Cli;

///<summary>
/// Interface for parameter parsing class
/// <c>TryParse</c> should never throw an exception, however, you loose any error messages as a result.
/// Use <c>Parse</c> when you want to capture the error message contained in any resulting exception.
///</summary>
public interface IParamsParser
{
	/*
	///<summary>Attempts to parse a string into a value</summary>
	///<param name="inp">The input string</param>
	///<param name="val">The output value</param>
	///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
	///<returns>true if the parsing was successfull otherwise false</returns>
	bool TryParse<T>(string inp, out T val);

	///<summary>Attempts to parse a string into a value</summary>
	///<param name="type">The output Type</param>
	///<param name="sub">The input string</param>
	///<param name="val">The output value</param>
	///<returns>true if the parsing was successfull otherwise false</returns>
	bool TryParse(Type type, string inp, out object val);
	*/

	///<summary>Attempts to parse a string into a value</summary>
	///<param name="inp">The input string</param>
	///<typeparam name="T">The output type of the value attempting to be parsed</typeparam>
	///<returns>The output value</returns>
	T Parse<T>(string inp);

	///<summary>Attempts to parse a string into a value</summary>
	///<param name="type">The output Type</param>
	///<param name="sub">The input string</param>
	///<returns>The output value<</returns>
	object Parse(Type type, string inp);
}