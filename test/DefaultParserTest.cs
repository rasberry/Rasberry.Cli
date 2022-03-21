using Rasberry.Cli;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Rasberry.Cli.Tests
{
	[TestClass]
	public class DefaultParserTest
	{
		static readonly DefaultParser parser = new();

		[DataTestMethod]
		[DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
		public void ParseSuccess(Type dest, string raw, object expected)
		{
			bool w = parser.TryParse(dest,raw,out object parsed);
			Assert.IsTrue(w);
			Assert.AreEqual(expected,parsed);
		}

		static IEnumerable<object[]> GetSuccessData()
		{
			yield return new object[] { typeof(bool),"true",true };
			yield return new object[] { typeof(bool),"true",true };
			yield return new object[] { typeof(bool),"True",true };
			yield return new object[] { typeof(bool),"false",false };
			yield return new object[] { typeof(bool),"False",false };
			yield return new object[] { typeof(bool)," true ",true };
			yield return new object[] { typeof(bool)," True ",true };

			yield return new object[] { typeof(byte),"32",(byte)32 };
			yield return new object[] { typeof(byte),"255",byte.MaxValue };
			yield return new object[] { typeof(byte),"0",byte.MinValue };
			yield return new object[] { typeof(byte),"1e+1",(byte)10 };
			yield return new object[] { typeof(byte),"10.0",(byte)10 };
			yield return new object[] { typeof(byte),"0x0A",(byte)10 };
			yield return new object[] { typeof(byte),"0b11",(byte)3 };
			yield return new object[] { typeof(byte),"0b011",(byte)3 };

			yield return new object[] { typeof(sbyte),"-32",(sbyte)-32 };
			yield return new object[] { typeof(sbyte),"0",(sbyte)0 };
			yield return new object[] { typeof(sbyte),"-128",sbyte.MinValue };
			yield return new object[] { typeof(sbyte),"127",sbyte.MaxValue };

			yield return new object[] { typeof(char),"a",'a' };
			yield return new object[] { typeof(char)," ",' ' };

			yield return new object[] { typeof(decimal),"1.0e+25",1e+25m };
			yield return new object[] { typeof(decimal),"0.0",0.0m };
			yield return new object[] { typeof(decimal),"-79228162514264337593543950335",decimal.MinValue };
			yield return new object[] { typeof(decimal),"79228162514264337593543950335",decimal.MaxValue };

			yield return new object[] { typeof(double),"1.0e+25",1e+25d };
			yield return new object[] { typeof(double),"0.0",0.0d };
			yield return new object[] { typeof(double),"-1.7976931348623157E+308",double.MinValue };
			yield return new object[] { typeof(double),"1.7976931348623157E+308",double.MaxValue };
			yield return new object[] { typeof(double),"Infinity", double.PositiveInfinity };
			yield return new object[] { typeof(double),"-Infinity", double.NegativeInfinity };

			yield return new object[] { typeof(float),"1.0e+25",1e+25f };
			yield return new object[] { typeof(float),"0.0",0.0f };
			yield return new object[] { typeof(float),"-3.40282347E+38",float.MinValue };
			yield return new object[] { typeof(float),"3.40282347E+38",float.MaxValue };
			yield return new object[] { typeof(float),"Infinity", float.PositiveInfinity };
			yield return new object[] { typeof(float),"-Infinity", float.NegativeInfinity };

			yield return new object[] { typeof(short),"0xA", (short)10 };
			yield return new object[] { typeof(short),"-10", (short)-10 };
			yield return new object[] { typeof(short),"0", (short)0 };
			yield return new object[] { typeof(short),"-32768",short.MinValue };
			yield return new object[] { typeof(short),"32767",short.MaxValue };
			yield return new object[] { typeof(short),"0b11", (short)3 };

			yield return new object[] { typeof(ushort),"0xA", (ushort)10 };
			yield return new object[] { typeof(ushort),"0",ushort.MinValue };
			yield return new object[] { typeof(ushort),"65535",ushort.MaxValue };
			yield return new object[] { typeof(ushort),"0b11", (ushort)3 };

			yield return new object[] { typeof(int),"0xA", 10 };
			yield return new object[] { typeof(int),"-10", -10 };
			yield return new object[] { typeof(int),"0", 0 };
			yield return new object[] { typeof(int),"-2147483648",int.MinValue };
			yield return new object[] { typeof(int),"2147483647",int.MaxValue };
			yield return new object[] { typeof(int),"0b11", 3 };

			yield return new object[] { typeof(uint),"0xA", 10u };
			yield return new object[] { typeof(uint),"0",uint.MinValue };
			yield return new object[] { typeof(uint),"4294967295",uint.MaxValue };

			yield return new object[] { typeof(long),"0xA", 10L };
			yield return new object[] { typeof(long),"-10", -10L };
			yield return new object[] { typeof(long),"0", 0L };
			yield return new object[] { typeof(long),"-9223372036854775808",long.MinValue };
			yield return new object[] { typeof(long),"9223372036854775807",long.MaxValue };
			yield return new object[] { typeof(long),"0b11", 3L };

			yield return new object[] { typeof(ulong),"0xA", 10uL };
			yield return new object[] { typeof(ulong),"0",ulong.MinValue };
			yield return new object[] { typeof(ulong),"18446744073709551615",ulong.MaxValue };
			yield return new object[] { typeof(ulong),"0b11", 3uL };

			yield return new object[] { typeof(bool?), "true", true };
			yield return new object[] { typeof(int?), "0xA", 10 };
			yield return new object[] { typeof(int?), "0", 0 };
			yield return new object[] { typeof(double?), "1.0", 1.0d };
			yield return new object[] { typeof(float?), "1.0", 1.0f };
		}

		[DataTestMethod]
		[DynamicData(nameof(GetFailData), DynamicDataSourceType.Method)]
		public void ParseFail(Type dest, string raw)
		{
			bool w = parser.TryParse(dest,raw,out object _);
			Assert.IsFalse(w);
		}

		static IEnumerable<object[]> GetFailData()
		{
			yield return new object[] { typeof(bool),"bad" };
			yield return new object[] { typeof(bool),"" };
			yield return new object[] { typeof(bool),null };
			yield return new object[] { typeof(bool)," " };

			yield return new object[] { typeof(byte),"-32" };
			yield return new object[] { typeof(byte),"256" };
			yield return new object[] { typeof(byte),"" };
			yield return new object[] { typeof(byte),null };

			yield return new object[] { typeof(sbyte),"-129" };
			yield return new object[] { typeof(sbyte),"128" };
			yield return new object[] { typeof(sbyte),"-0x0A" };
			yield return new object[] { typeof(sbyte),"" };
			yield return new object[] { typeof(sbyte),null };

			yield return new object[] { typeof(char),"aa" };
			yield return new object[] { typeof(char),"97" };
			yield return new object[] { typeof(char),"" };
			yield return new object[] { typeof(char),null };

			yield return new object[] { typeof(double),"NaN" };
			yield return new object[] { typeof(double),"0xA.1f" };
			yield return new object[] { typeof(float),"NaN" };

			yield return new object[] { typeof(ushort),"-10" };
			yield return new object[] { typeof(uint),"-10" };
			yield return new object[] { typeof(ulong),"-10" };

			yield return new object[] { typeof(int),"0x0Q" };
			yield return new object[] { typeof(int),"0b012" };
		}

		[DataTestMethod]
		[DataRow("bread",FoodStuff.Bread)]
		[DataRow("Bread",FoodStuff.Bread)]
		[DataRow("0",FoodStuff.Donut)]
		[DataRow("3",FoodStuff.Fish)]
		public void ParseEnumSuccess(string raw, FoodStuff value)
		{
			bool w = parser.TryParse(raw,out FoodStuff food);
			Assert.IsTrue(w);
			Assert.AreEqual(value,food);
		}

		[DataTestMethod]
		[DataRow("b")]
		[DataRow("-1")]
		[DataRow("999")]
		public void ParseEnumFail(string raw)
		{
			bool w = parser.TryParse(raw,out FoodStuff _);
			Assert.IsFalse(w);
		}

		[TestMethod]
		public void ParseStringSuccess()
		{
			bool w = parser.TryParse("test",out string val);
			Assert.IsTrue(w);
			Assert.AreEqual("test",val);
		}

		[DataTestMethod]
		[DataRow("")]
		[DataRow(null)]
		[DataRow(" ")]
		public void ParseStringFail(string raw)
		{
			//parsing should fail
			bool w = parser.TryParse(raw,out string val);
			Assert.IsFalse(w);
			Assert.AreEqual(null,val);
		}
	}
}
