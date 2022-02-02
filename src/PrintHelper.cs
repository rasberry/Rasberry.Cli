using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Rasberry.Cli
{
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

		///<summary>Enumerates an enum</summary>
		///<param name="excludeZero">Exclude the zero value entry</param>
		public static IEnumerable<T> EnumAll<T>(bool excludeZero = false)
			where T : struct
		{
			foreach(T a in Enum.GetValues(typeof(T))) {
				int v = (int)(object)a;
				if (excludeZero && v == 0) { continue; }
				yield return a;
			};
		}

		///<summary>Prints an enum with optional descriptions</summary>
		///<param name="sb">The destination StringBuilder</param>
		///<param name="level">The indention level for the parameter name (defaults to 0)</param>
		///<param name="descriptionMap">A map of descriptions for each enum value</param>
		///<param name="nameMap">A map of replacement names for each enum value</param>
		///<param name="excludeZero">Exclude the zero value entry</param>
		///<returns>returns the StringBuilder for chaining</returns>
		public static StringBuilder PrintEnum<T>(this StringBuilder sb, int level = 0, Func<T,string> descriptionMap = null,
			Func<T,string> nameMap = null, bool excludeZero = false) where T : struct
		{
			var allEnums = EnumAll<T>(excludeZero).ToList();
			int numLen = 1 + (int)Math.Floor(Math.Log10(allEnums.Count));
			foreach(T e in allEnums) {
				int eValue = (int)(object)e;
				string sValue = eValue.ToString();
				int padLength = sValue.Length < numLen ? numLen - sValue.Length : 0;
				string pad = new string(' ',padLength);
				string eName = nameMap == null ? e.ToString() : nameMap(e);
				string eDesc = descriptionMap == null ? "" : descriptionMap(e);
				sb.ND(level,$"{pad}{sValue}. {eName}",eDesc);
			}
			return sb;
		}
	}
}