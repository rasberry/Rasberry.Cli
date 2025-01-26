using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Rasberry.Cli;

///<summary>Tools for printing the help message</summary>
public static class PrintHelper
{
	///<summary>Sets the position for the description text</summary>
	public static int DescriptionColumn { get; set; } = 30;

	///<summary>Gets or Sets the character width of the shell</summary>
	public static int OutputWidth { get; set; } = InitOutputWidth();

	static int InitOutputWidth()
	{
		return Console.IsOutputRedirected ? int.MaxValue : Console.BufferWidth;
	}

	///<summary>Prints a parameter name and description. The description may wrap</summary>
	///<param name="sb">The destination StringBuilder</param>
	///<param name="level">The indention level for the parameter name</param>
	///<param name="name">The name of the parameter</param>
	///<param name="desc">The description of the parameter (optional)</param>
	///<returns>returns the StringBuilder for chaining</returns>
	public static StringBuilder ND(this StringBuilder sb, int level, string name, string desc = "")
	{
		if (level < 0) { level = 0; }
		sb
			.Append(' ',level)
			.Append(name)
		;
		if (!string.IsNullOrWhiteSpace(desc)) {
			int pad = DescriptionColumn - name.Length - level;
			sb
				.Append(' ',pad > 0 ? pad : 0)
				.AppendWrap(DescriptionColumn,desc)
			;
		}
		else {
			sb.AppendLine();
		}
		return sb;
	}

	///<summary>Prints a line of text with wrapping</summary>
	///<param name="sb">The destination StringBuilder</param>
	///<param name="level">The indention level the text (defaults to 0)</param>
	///<param name="text">The text to print. if null only a new line is printed</param>
	///<returns>returns the StringBuilder for chaining</returns>
	public static StringBuilder WT(this StringBuilder sb, int level = 0, string text = null)
	{
		if (text == null) {
			sb.AppendLine();
		}
		else {
			sb
				.Append(' ',level)
				.AppendWrap(level,text)
			;
		}
		return sb;
	}

	static StringBuilder AppendWrap(this StringBuilder self, int offset, string m)
	{
		int w = OutputWidth - 1 - offset;
		int c = 0;
		int l = m.Length;

		while(c < l) {
			//need spacing after first line
			string o = c > 0 ? new string(' ',offset) : "";
			//this is the last iteration
			if (c + w >= l) {
				string s = m.Substring(c);
				c += w;
				self.Append(o).AppendLine(s);
			}
			//we're in the middle
			else {
				string s = m.Substring(c,w);
				c += w;
				self.Append(o).AppendLine(s);
			}
		}

		//we always want a newline even if m is empty
		if (l < 1) {
			self.AppendLine();
		}

		//StringBuilder likes to chain
		return self;
	}


	/// <summary>Enumerates an enum</summary>
	/// <typeparam name="T">Type of the enum</typeparam>
	/// <param name="excludeZero">Exclude the zero value entry</param>
	/// <returns>An enumeration of the enum names</returns>
	public static IEnumerable<T> EnumAll<T>(bool excludeZero = false)
		where T : struct
	{
		return EnumAll(typeof(T), excludeZero).Cast<T>();
	}

	/// <summary>Enumerates an enum</summary>
	/// <param name="t">Type of the enum</param>
	/// <param name="excludeZero">Exclude the zero value entry</param>
	/// <returns>An enumeration of the enum names</returns>
	public static IEnumerable EnumAll(Type t, bool excludeZero = false)
	{
		foreach(object a in Enum.GetValues(t)) {
			int v = (int)a;
			if (excludeZero && v == 0) { continue; }
			yield return a;
		};
	}

	/// <summary>Prints an enum with optional descriptions</summary>
	/// <typeparam name="T">Type of the enum</typeparam>
	/// <param name="sb">The destination StringBuilder</param>
	/// <param name="level">The indention level for the parameter name (default 0)</param>
	/// <param name="descriptionMap">A map of descriptions for each enum value</param>
	/// <param name="nameMap">A map of replacement names for each enum value</param>
	/// <param name="excludeZero">Exclude the zero value entry</param>
	/// <returns>returns the StringBuilder for chaining (default false)</returns>
	public static StringBuilder PrintEnum<T>(this StringBuilder sb, int level = 0, Func<T,string> descriptionMap = null,
		Func<T,string> nameMap = null, bool excludeZero = false) where T : struct
	{
		Func<object, string> dWrap = null;
		Func<object, string> nWrap = null;

		if (descriptionMap != null) {
			dWrap = (object o) => descriptionMap((T)o);
		}
		if (nameMap != null) {
			nWrap = (object o) => nameMap((T)o);
		}

		return PrintEnum(sb,typeof(T),level,dWrap,nWrap,excludeZero);
	}

	/// <summary>Prints an enum with optional descriptions</summary>
	/// <param name="sb">The destination StringBuilder</param>
	/// <param name="t">Type of the enum</param>
	/// <param name="level">The indention level for the parameter name (default 0)</param>
	/// <param name="descriptionMap">A map of descriptions for each enum value</param>
	/// <param name="nameMap">A map of replacement names for each enum value</param>
	/// <param name="excludeZero">Exclude the zero value entry (default false)</param>
	/// <returns>returns the StringBuilder for chaining</returns>
	public static StringBuilder PrintEnum(this StringBuilder sb, Type t, int level = 0, Func<object,string> descriptionMap = null,
		Func<object,string> nameMap = null, bool excludeZero = false)
	{
		var allEnums = EnumAll(t, excludeZero).ToArrayList();
		int numLen = 1 + (int)Math.Floor(Math.Log10(allEnums.Count));
		foreach(object e in allEnums) {
			int eValue = (int)e;
			string sValue = eValue.ToString();
			int padLength = sValue.Length < numLen ? numLen - sValue.Length : 0;
			string pad = new(' ',padLength);
			string eName = nameMap == null ? e.ToString() : nameMap(e);
			string eDesc = descriptionMap == null ? "" : descriptionMap(e);
			sb.ND(level,$"{pad}{sValue}. {eName}",eDesc);
		}
		return sb;
	}

	static ArrayList ToArrayList(this IEnumerable source)
	{
		ArrayList list = new();
		foreach(object i in source) {
			list.Add(i);
		}
		return list;
	}
}