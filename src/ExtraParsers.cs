using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rasberry.Cli
{
	///<summary>Additional parsers</summary>
	public static class ExtraParsers
	{
		///<summary>Attempts to parse a number or number% into a value</summary>
		///<param name="num">The input string</param>
		///<param name="val">The output value</param>
		///<param name="provider">Optional <c>IFormatProvider</c> defaults to <c>CultureInfo.InvariantCulture.NumberFormat</c></param>
		///<returns>true if the parsing was successfull otherwise false</returns>
		public static bool TryParseNumberPercent(string num, out double val, IFormatProvider provider = null)
		{
			val = default;
			if (num == null) {
				return false;
			}

			bool isPercent = false;
			if (num.EndsWith('%')) {
				isPercent = true;
				num = num.Remove(num.Length - 1);
			}

			if (provider == null) {
				provider = CultureInfo.InvariantCulture.NumberFormat;
			}

			if (!double.TryParse(num, NumberStyles.Any, provider, out double d)) {
				return false;
			}
			if (!double.IsFinite(d)) {
				return false;
			}
			val = isPercent ? d/100.0 : d;
			return true;
		}

		///<summary>Attempts to parse a number or number% into a value</summary>
		///<param name="num">The input string</param>
		///<param name="val">The output value</param>
		///<param name="provider">Optional <c>IFormatProvider</c> defaults to <c>CultureInfo.InvariantCulture.NumberFormat</c></param>
		///<returns>true if the parsing was successfull otherwise false</returns>
		public static bool TryParseNumberPercent(string num, out double? val, IFormatProvider provider = null)
		{
			val = null;
			bool worked = TryParseNumberPercent(num,out double v,provider);
			if (worked) { val = v; }
			return worked;
		}

		///<summary>Attempts to parse an enum and will try to match the first letter</summary>
		///<param name="arg">The input string</param>
		///<param name="val">The output value</param>
		///<param name="ignoreZero">Ignore the enum item with value 0. Usefull for skipping a 'not-defined' placeholder item</param>
		///<returns>true if the parsing was successfull otherwise false</returns>
		public static bool TryParseEnumFirstLetter<T>(string arg, out T val, bool ignoreZero = false) where T : struct
		{
			bool worked = Enum.TryParse<T>(arg,true,out val);
			//try to match the first letter if normal enum parse fails
			if (!worked) {
				string fla = arg.Substring(0,1);
				foreach(T item in Enum.GetValues(typeof(T))) {
					string name = item.ToString();
					if (ignoreZero && (int)(object)item == 0) { continue; }
					string fln = name.Substring(0,1);
					if (fla.Equals(fln,StringComparison.OrdinalIgnoreCase)) {
						val = item;
						return true;
					}
				}
			}

			if (ignoreZero && (int)(object)val == 0) {
				return false;
			}

			return worked && Enum.IsDefined(typeof(T),val);
		}

		///<summary>Attempts to parse a sequence of values (for example a set of space seperated numbers)</summary>
		///<param name="arg">The input string</param>
		///<param name="delimiters">A set of delimiters</param>
		///<param name="seq">the output list of values</param>
		///<param name="parser">The parser to use on each piece of the input</param>
		///<returns>true if the parsing was successfull otherwise false</returns>
		public static bool TryParseSequence<T>(string arg, char[] delimiters,
			out IReadOnlyList<T> seq, ParseParams.Parser<T> parser)
		{
			seq = null;
			if (string.IsNullOrWhiteSpace(arg)) { return false; }

			var parts = arg.Split(delimiters,StringSplitOptions.RemoveEmptyEntries);
			var list = new List<T>();
			for(int p=0; p<parts.Length; p++) {
				if (!parser(parts[p], out T n)) {
					return false; //not able to parse as the underlying type
				}
				list.Add(n);
			}
			seq = list;
			return true;
		}
	}
}
