using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rasberry.Cli;

///<summary>Additional parsers</summary>
public static class ExtraParsers
{
	///<summary>Attempts to parse a number or number% into a value</summary>
	///<param name="num">The input string</param>
	///<param name="provider">Optional <c>IFormatProvider</c> defaults to <c>CultureInfo.InvariantCulture.NumberFormat</c></param>
	///<returns>The output value</returns>
	///<exception cref="ArgumentNullException"></exception>
	///<exception cref="ArgumentException"></exception>
	///<exception cref="ArgumentOutOfRangeException"></exception>
	public static double ParseNumberPercent(string num, IFormatProvider provider = null)
	{
		if(num == null) {
			throw new ArgumentNullException(nameof(num));
		}

		bool isPercent = false;
		if(num.EndsWith('%')) {
			isPercent = true;
			num = num.Remove(num.Length - 1);
		}

		if(provider == null) {
			provider = CultureInfo.InvariantCulture.NumberFormat;
		}

		double d = double.Parse(num, NumberStyles.Any, provider);
		if(!double.IsFinite(d)) {
			throw new ArgumentOutOfRangeException(nameof(num), "Only finite numbers are allowed");
		}
		return isPercent ? d / 100.0 : d;
	}

	///<summary>Attempts to parse an enum and will try to match the first letter</summary>
	///<param name="arg">The input string</param>
	///<param name="ignoreZero">Ignore the enum item with value 0. Usefull for skipping a 'not-defined' placeholder item</param>
	///<returns>The output value</returns>
	public static T ParseEnumFirstLetter<T>(string arg, bool ignoreZero = false) where T : struct
	{
		bool worked = Enum.TryParse<T>(arg, true, out T val);
		//try to match the first letter if normal enum parse fails
		if(!worked) {
			string fla = arg.Substring(0, 1);
			foreach(T item in Enum.GetValues(typeof(T))) {
				string name = item.ToString();
				if(ignoreZero && (int)(object)item == 0) { continue; }

				string fln = name.Substring(0, 1);
				if(fla.Equals(fln, StringComparison.OrdinalIgnoreCase)) {
					return item;
				}
			}
		}

		if(ignoreZero && (int)(object)val == 0) {
			throw new ArgumentOutOfRangeException(nameof(arg), "Ignoring item zero");
		}

		return val;
	}

	///<summary>Attempts to parse a sequence of values (for example a set of space seperated numbers)</summary>
	///<param name="arg">The input string</param>
	///<param name="delimiters">A set of delimiters</param>
	///<param name="parser">The optional parser to use on each piece of the input</param>
	///<returns>the output list of values</returns>
	public static IReadOnlyList<T> ParseSequence<T>(string arg, char[] delimiters, ParseParams.Parser<T> parser = null)
	{
		if(string.IsNullOrWhiteSpace(arg)) {
			throw new ArgumentException(nameof(arg));
		}

		if(parser == null) {
			var dp = new DefaultParser();
			parser = dp.Parse<T>;
		}

		var parts = arg.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
		var list = new List<T>();
		for(int p = 0; p < parts.Length; p++) {
			T n = parser(parts[p]);
			list.Add(n);
		}
		return list.AsReadOnly();
	}

	/// <summary>
	/// Attempts to parse a color from name for hex value. for example 'red' or '#FF0000'
	/// </summary>
	/// <param name="sub">the value to be interpreted as a color</param>
	/// <returns>The color value</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FormatException"></exception>
	public static System.Drawing.Color ParseColor(string sub)
	{
		var color = FromName(sub);
		if(color.HasValue) {
			return color.Value;
		}

		return FromHex(sub);
	}

	static System.Drawing.Color? FromName(string name)
	{
		var c = System.Drawing.Color.FromName(name);
		bool good = c.IsKnownColor && !c.IsSystemColor;
		return good ? c : null;
	}

	static System.Drawing.Color FromHex(string s)
	{
		if(s[0] == '#') {
			s = s.Substring(1);
		}
		int len = s.Length;
		if(len != 8 && len != 6 && len != 4 && len != 3) {
			throw new ArgumentException("Hex string length must be one of 3,4,6,8");
		}

		int scale = len == 8 || len == 6 ? 2 : 1;
		bool hasAlpha = len == 8 || len == 4;
		int scaleHex = scale == 1 ? 17 : 1; //scaling by 17 (0x11) 'doubles' the digit A -> AA, B -> BB

		string rr = s.Substring(0 * scale, scale);
		string gg = s.Substring(1 * scale, scale);
		string bb = s.Substring(2 * scale, scale);
		string aa = hasAlpha ? s.Substring(3 * scale, scale) : new String('F', scale);

		int cr = ParseHex(rr) * scaleHex;
		int cg = ParseHex(gg) * scaleHex;
		int cb = ParseHex(bb) * scaleHex;
		int ca = ParseHex(aa) * scaleHex;

		var rgba = System.Drawing.Color.FromArgb(ca, cr, cg, cb);
		return rgba;
	}

	static int ParseHex(string h)
	{
		int num = int.Parse(h, NumberStyles.HexNumber);
		return num;
	}
}
