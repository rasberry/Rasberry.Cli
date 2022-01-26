using System;
using System.Globalization;

namespace Rasberry.Cli
{
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
	///<para>double: Tries to parse to a 64 bit floating point Double</para>
	///<para>float: Tries to parse to a 32 bit floating point Single</para>
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
		public DefaultParser()
		{
			ifp = CultureInfo.InvariantCulture.NumberFormat;
		}

		public DefaultParser(IFormatProvider provider)
		{
			ifp = provider;
		}

		readonly IFormatProvider ifp;

		delegate bool TryParseWrapped<T>(string s, NumberStyles ns, IFormatProvider fp, out T val);

		///<summary>Attempts to parse a string into a value</summary>
		///<param name="sub">The input string</param>
		///<param name="val">The output value</param>
		///<typeparam name="V">The output type of the value attempting to be parsed</typeparam>
		///<returns><c>bool</c> true if the parsing was successfull otherwise false</returns>
		public bool TryParse<V>(string sub, out V val)
		{
			val = default;
			Type t = typeof(V);

			var nullType = Nullable.GetUnderlyingType(t);
			if (nullType != null) { t = nullType; }

			//check for enum first since it might match other types
			if (t.IsEnum) {
				if (Enum.TryParse(t,sub,true,out object o)) {
					val = (V)o;
					return Enum.IsDefined(t,o);
				}
			}

			//standard types have a TypeCode
			var typeCode = Type.GetTypeCode(t);
			switch(typeCode) {
				case TypeCode.Boolean: {
					if (bool.TryParse(sub,out bool b)) {
						val = (V)(object)b;
						return true;
					};
				} break;
				case TypeCode.Byte: {
					TryParseWrapped<byte> func = byte.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Char: {
					if (char.TryParse(sub,out char c)) {
						val = (V)(object)c;
						return true;
					}
				} break;
				case TypeCode.Decimal: {
					TryParseWrapped<decimal> func = decimal.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Double: {
					TryParseWrapped<double> func = double.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Int16: {
					TryParseWrapped<short> func = short.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Int32: {
					TryParseWrapped<int> func = int.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Int64: {
					TryParseWrapped<long> func = long.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.SByte: {
					TryParseWrapped<sbyte> func = sbyte.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Single: {
					TryParseWrapped<float> func = float.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.String: {
					if (!string.IsNullOrWhiteSpace(sub)) {
						val = (V)(object)sub;
						return true;
					}
				} break;
				case TypeCode.UInt16: {
					TryParseWrapped<ushort> func = ushort.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.UInt32: {
					TryParseWrapped<uint> func = uint.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.UInt64: {
					TryParseWrapped<ulong> func = ulong.TryParse;
					if (TryWrapped(func,sub,out val)) {
						return true;
					}
				} break;
			}

			return false;
		}

		//saving some typing for types with NumberStyles
		bool TryWrapped<T,V>(TryParseWrapped<T> func, string s, out V val)
		{
			bool worked = func(s,NumberStyles.Any,ifp,out T n);
			if (worked) {
				val = (V)(object)n;
				return true;
			}
			else {
				val = default;
				return false;
			}
		}
	}
}