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
		///<returns>true if the parsing was successfull otherwise false</returns>
		public bool TryParse<V>(string sub, out V val)
		{
			val = default;
			bool worked = TryParse(typeof(V),sub,out object o);
			if (worked) {
				val = (V)o;
				return true;
			}
			return false;
		}

		///<summary>Attempts to parse a string into a value</summary>
		///<param name="type">The output Type</param>
		///<param name="sub">The input string</param>
		///<param name="val">The output value</param>
		///<returns>true if the parsing was successfull otherwise false</returns>
		public bool TryParse(Type type, string sub, out object val)
		{
			val = null;
			var nullType = Nullable.GetUnderlyingType(type);
			if (nullType != null) { type = nullType; }

			//check for enum first since it might match other types
			if (type.IsEnum) {
				if (Enum.TryParse(type,sub,true,out val)) {
					return Enum.IsDefined(type,val);
				}
			}

			//standard types have a TypeCode
			var typeCode = Type.GetTypeCode(type);
			switch(typeCode) {
				case TypeCode.Boolean: {
					if (bool.TryParse(sub,out bool b)) {
						val = b;
						return true;
					};
				} break;
				case TypeCode.Byte: {
					TryParseWrapped<byte> func = byte.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Char: {
					if (char.TryParse(sub,out char c)) {
						val = c;
						return true;
					}
				} break;
				case TypeCode.Decimal: {
					TryParseWrapped<decimal> func = decimal.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Double: {
					TryParseWrapped<double> func = double.TryParse;
					if (TryNumber(func,sub,out val,false)) {
						if (!double.IsNaN((double)val)) {
							return true;
						}
					}
				} break;
				case TypeCode.Int16: {
					TryParseWrapped<short> func = short.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Int32: {
					TryParseWrapped<int> func = int.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Int64: {
					TryParseWrapped<long> func = long.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.SByte: {
					TryParseWrapped<sbyte> func = sbyte.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.Single: {
					TryParseWrapped<float> func = float.TryParse;
					if (TryNumber(func,sub,out val,false)) {
						if (!float.IsNaN((float)val)) {
							return true;
						}
					}
				} break;
				case TypeCode.String: {
					if (!string.IsNullOrWhiteSpace(sub)) {
						val = sub;
						return true;
					}
				} break;
				case TypeCode.UInt16: {
					TryParseWrapped<ushort> func = ushort.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.UInt32: {
					TryParseWrapped<uint> func = uint.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
				case TypeCode.UInt64: {
					TryParseWrapped<ulong> func = ulong.TryParse;
					if (TryNumber(func,sub,out val)) {
						return true;
					}
				} break;
			}

			return false;
		}

		//saving some typing for types with NumberStyles
		bool TryNumber<T>(TryParseWrapped<T> func, string s, out object val, bool allowHex = true)
		{
			val = default;
			//support hex numbers (with the 0x prefix)
			if (s != null && allowHex && s.StartsWith("0x",StringComparison.InvariantCultureIgnoreCase)) {
				bool worked = func(s[2..],NumberStyles.HexNumber,ifp,out T n);
				if (worked) {
					val = n;
					return true;
				}
				return false;
			}
			//otherwise try normal numbers
			else {
				bool worked = func(s,NumberStyles.Any,ifp,out T n);
				if (worked) {
					val = n;
					return true;
				}
				return false;
			}
		}
	}
}