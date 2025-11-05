using System;
using System.Globalization;

namespace Rasberry.Cli;

///<summary>
///<para>The default parser for converting strings into values</para>
///<para>Supported Types:</para>
///<para>nullable: Nullables are unwrapped to their parent type</para>
///<para>enum: Tries to parse a name or number to an enum. Case-insensitive names are allowed.</para>
///<para>bool: Tries to parse to a Boolean. accepts the strings "true" or "false" case-insensitive</para>
///<para>byte: Tries to parse to a 8 bit Byte</para>
///<para>sbyte: Tries to parse to a 8 bit signed SByte</para>
///<para>char: Tries to parse to a Char</para>
///<para>decimal: Tries to parse to 128 bit floating point Decimal</para>
///<para>double: Tries to parse to a 64 bit floating point Double. Fails on NaN</para>
///<para>float: Tries to parse to a 32 bit floating point Single. Fails on NaN</para>
///<para>short: Tries to parse to an 16 bit Int16</para>
///<para>ushort: Tries to parse to an unsigned UInt16</para>
///<para>int: Tries to parse to an 32 bit Int32</para>
///<para>uint: Tries to parse to an unsigned UInt32</para>
///<para>long: Tries to parse to an 64 bit Int64</para>
///<para>ulong: Tries to parse to an unsigned UInt64</para>
///<para>string: This is a passthrough but fails for null or whitespace-only strings</para>
///</summary>
public sealed class DefaultParser : IParamsParser
{
	/// <summary>
	/// Default Constructor
	/// </summary>
	public DefaultParser()
	{
		ifp = CultureInfo.InvariantCulture.NumberFormat;
	}

	/// <summary>
	/// Constructor with <c>IFormatProvider</c>
	/// </summary>
	/// <param name="provider">The format provider to use with parsing</param>
	public DefaultParser(IFormatProvider provider)
	{
		ifp = provider;
	}

	readonly IFormatProvider ifp;
	delegate T ParseWrapped<T>(string s, NumberStyles ns, IFormatProvider fp);

	/// <summary>
	/// Attempts to parse a string into a value
	/// </summary>
	/// <typeparam name="V">The output type of the value attempting to be parsed</typeparam>
	/// <param name="sub">The input string</param>
	/// <returns>The output value</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FormatException"></exception>
	/// <exception cref="InvalidCastException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="OverflowException"></exception>
	public V Parse<V>(string sub)
	{
		return (V)Parse(typeof(V), sub);
	}

	/// <summary>
	/// Attempts to parse a string into a value
	/// </summary>
	/// <param name="type">The output Type</param>
	/// <param name="sub">The input string</param>
	/// <returns>The output value</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FormatException"></exception>
	/// <exception cref="InvalidCastException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="OverflowException"></exception>
	public object Parse(Type type, string sub)
	{
		var nullType = Nullable.GetUnderlyingType(type);
		if(nullType != null) { type = nullType; }

		//check for enum first since it might match other types
		if(type.IsEnum) {
			return Enum.Parse(type, sub, true);
		}

		//standard types have a TypeCode
		var typeCode = Type.GetTypeCode(type);
		switch(typeCode) {
		case TypeCode.Boolean: return bool.Parse(sub);
		case TypeCode.Byte: return ParseNumber(byte.Parse, sub);
		case TypeCode.Char: return char.Parse(sub);
		case TypeCode.Decimal: return ParseNumber(decimal.Parse, sub);

		case TypeCode.Double:
			double d = (double)ParseNumber(double.Parse, sub);
			if(double.IsNaN(d)) {
				throw new ArgumentException("NaN is not allowed");
			}
			return d;

		case TypeCode.Int16: return ParseNumber(short.Parse, sub);
		case TypeCode.Int32: return ParseNumber(int.Parse, sub);
		case TypeCode.Int64: return ParseNumber(long.Parse, sub);
		case TypeCode.SByte: return ParseNumber(sbyte.Parse, sub);

		case TypeCode.Single:
			float f = (float)ParseNumber(float.Parse, sub);
			if(float.IsNaN(f)) {
				throw new ArgumentException("NaN is not allowed");
			}
			return f;

		case TypeCode.String:
			if(string.IsNullOrWhiteSpace(sub)) {
				throw new ArgumentException("Null, empty, or whitespace-only values are not allowed");
			}
			return sub;
		case TypeCode.UInt16: return ParseNumber(ushort.Parse, sub);
		case TypeCode.UInt32: return ParseNumber(uint.Parse, sub);
		case TypeCode.UInt64: return ParseNumber(ulong.Parse, sub);
		}

		throw new NotSupportedException($"Type {type} is not supported");
	}

	//saving some typing for types with NumberStyles
	const int PrefixLength = 2;
	const int BinaryRadix = 2;
	object ParseNumber<T>(ParseWrapped<T> func, string s, bool allowHexBin = true)
	{
		var comp = StringComparison.InvariantCultureIgnoreCase;
		//support hex numbers (with the 0x prefix)
		if(s != null && allowHexBin && s.StartsWith("0x", comp)) {
			var plain = s.Substring(PrefixLength);
			return func(plain, NumberStyles.HexNumber, ifp);
		}
		//support binary numbers (with the 0b prefix)
		else if(s != null && allowHexBin && s.StartsWith("0b", comp)) {
			var plain = s.Substring(PrefixLength);
			// sigh.. no NumberStyles.BinaryNumber :/
			//must pick a type here.. going with worst case
			long n = Convert.ToInt64(plain, BinaryRadix);
			return Convert.ChangeType(n, typeof(T));
		}
		//otherwise try normal numbers
		else {
			return func(s, NumberStyles.Any, ifp);
		}
	}
}
