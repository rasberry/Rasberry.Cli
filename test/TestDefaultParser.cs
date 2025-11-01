using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestDefaultParser
{
	static readonly DefaultParser parser = new();

	[TestMethod]
	[DynamicData(nameof(GetSuccessData))]
	public void ParseSuccess(Type dest, string raw, object expected)
	{
		object parsed = parser.Parse(dest, raw);
		Assert.AreEqual(expected, parsed);
	}

	static IEnumerable<object[]> GetSuccessData()
	{
		yield return new object[] { typeof(bool), "true", true };
		yield return new object[] { typeof(bool), "true", true };
		yield return new object[] { typeof(bool), "True", true };
		yield return new object[] { typeof(bool), "false", false };
		yield return new object[] { typeof(bool), "False", false };
		yield return new object[] { typeof(bool), " true ", true };
		yield return new object[] { typeof(bool), " True ", true };

		yield return new object[] { typeof(byte), "32", (byte)32 };
		yield return new object[] { typeof(byte), "255", byte.MaxValue };
		yield return new object[] { typeof(byte), "0", byte.MinValue };
		yield return new object[] { typeof(byte), "1e+1", (byte)10 };
		yield return new object[] { typeof(byte), "10.0", (byte)10 };
		yield return new object[] { typeof(byte), "0x0A", (byte)10 };
		yield return new object[] { typeof(byte), "0b11", (byte)3 };
		yield return new object[] { typeof(byte), "0b011", (byte)3 };

		yield return new object[] { typeof(sbyte), "-32", (sbyte)-32 };
		yield return new object[] { typeof(sbyte), "0", (sbyte)0 };
		yield return new object[] { typeof(sbyte), "-128", sbyte.MinValue };
		yield return new object[] { typeof(sbyte), "127", sbyte.MaxValue };

		yield return new object[] { typeof(char), "a", 'a' };
		yield return new object[] { typeof(char), " ", ' ' };

		yield return new object[] { typeof(decimal), "1.0e+25", 1e+25m };
		yield return new object[] { typeof(decimal), "0.0", 0.0m };
		yield return new object[] { typeof(decimal), "-79228162514264337593543950335", decimal.MinValue };
		yield return new object[] { typeof(decimal), "79228162514264337593543950335", decimal.MaxValue };

		yield return new object[] { typeof(double), "1.0e+25", 1e+25d };
		yield return new object[] { typeof(double), "0.0", 0.0d };
		yield return new object[] { typeof(double), "-1.7976931348623157E+308", double.MinValue };
		yield return new object[] { typeof(double), "1.7976931348623157E+308", double.MaxValue };
		yield return new object[] { typeof(double), "Infinity", double.PositiveInfinity };
		yield return new object[] { typeof(double), "-Infinity", double.NegativeInfinity };

		yield return new object[] { typeof(float), "1.0e+25", 1e+25f };
		yield return new object[] { typeof(float), "0.0", 0.0f };
		yield return new object[] { typeof(float), "-3.40282347E+38", float.MinValue };
		yield return new object[] { typeof(float), "3.40282347E+38", float.MaxValue };
		yield return new object[] { typeof(float), "Infinity", float.PositiveInfinity };
		yield return new object[] { typeof(float), "-Infinity", float.NegativeInfinity };

		yield return new object[] { typeof(short), "0xA", (short)10 };
		yield return new object[] { typeof(short), "-10", (short)-10 };
		yield return new object[] { typeof(short), "0", (short)0 };
		yield return new object[] { typeof(short), "-32768", short.MinValue };
		yield return new object[] { typeof(short), "32767", short.MaxValue };
		yield return new object[] { typeof(short), "0b11", (short)3 };

		yield return new object[] { typeof(ushort), "0xA", (ushort)10 };
		yield return new object[] { typeof(ushort), "0", ushort.MinValue };
		yield return new object[] { typeof(ushort), "65535", ushort.MaxValue };
		yield return new object[] { typeof(ushort), "0b11", (ushort)3 };

		yield return new object[] { typeof(int), "0xA", 10 };
		yield return new object[] { typeof(int), "-10", -10 };
		yield return new object[] { typeof(int), "0", 0 };
		yield return new object[] { typeof(int), "-2147483648", int.MinValue };
		yield return new object[] { typeof(int), "2147483647", int.MaxValue };
		yield return new object[] { typeof(int), "0b11", 3 };

		yield return new object[] { typeof(uint), "0xA", 10u };
		yield return new object[] { typeof(uint), "0", uint.MinValue };
		yield return new object[] { typeof(uint), "4294967295", uint.MaxValue };

		yield return new object[] { typeof(long), "0xA", 10L };
		yield return new object[] { typeof(long), "-10", -10L };
		yield return new object[] { typeof(long), "0", 0L };
		yield return new object[] { typeof(long), "-9223372036854775808", long.MinValue };
		yield return new object[] { typeof(long), "9223372036854775807", long.MaxValue };
		yield return new object[] { typeof(long), "0b11", 3L };

		yield return new object[] { typeof(ulong), "0xA", 10uL };
		yield return new object[] { typeof(ulong), "0", ulong.MinValue };
		yield return new object[] { typeof(ulong), "18446744073709551615", ulong.MaxValue };
		yield return new object[] { typeof(ulong), "0b11", 3uL };

		yield return new object[] { typeof(bool?), "true", true };
		yield return new object[] { typeof(int?), "0xA", 10 };
		yield return new object[] { typeof(int?), "0", 0 };
		yield return new object[] { typeof(double?), "1.0", 1.0d };
		yield return new object[] { typeof(float?), "1.0", 1.0f };
	}

	[TestMethod]
	[DynamicData(nameof(GetFailData))]
	public void ParseFail(Type dest, string raw, Type err)
	{
		Assert.That.ThrowsExceptionType(err, () => {
			object parsed = parser.Parse(dest, raw);
		});
	}

	static IEnumerable<object[]> GetFailData()
	{
		yield return new object[] { typeof(bool), "bad", typeof(FormatException) };
		yield return new object[] { typeof(bool), "", typeof(FormatException) };
		yield return new object[] { typeof(bool), null, typeof(ArgumentNullException) };
		yield return new object[] { typeof(bool), " ", typeof(FormatException) };
		yield return new object[] { typeof(bool), "0", typeof(FormatException) };
		yield return new object[] { typeof(bool), "1", typeof(FormatException) };

		yield return new object[] { typeof(byte), "-32", typeof(OverflowException) };
		yield return new object[] { typeof(byte), "256", typeof(OverflowException) };
		yield return new object[] { typeof(byte), "", typeof(FormatException) };
		yield return new object[] { typeof(byte), null, typeof(ArgumentNullException) };

		yield return new object[] { typeof(sbyte), "-129", typeof(OverflowException) };
		yield return new object[] { typeof(sbyte), "128", typeof(OverflowException) };
		yield return new object[] { typeof(sbyte), "-0x0A", typeof(FormatException) };
		yield return new object[] { typeof(sbyte), "", typeof(FormatException) };
		yield return new object[] { typeof(sbyte), null, typeof(ArgumentNullException) };

		yield return new object[] { typeof(char), "aa", typeof(FormatException) };
		yield return new object[] { typeof(char), "97", typeof(FormatException) };
		yield return new object[] { typeof(char), "", typeof(FormatException) };
		yield return new object[] { typeof(char), null, typeof(ArgumentNullException) };

		yield return new object[] { typeof(double), "NaN", typeof(ArgumentException) };
		yield return new object[] { typeof(double), "0xA.1f", typeof(ArgumentException) };
		yield return new object[] { typeof(float), "NaN", typeof(ArgumentException) };

		yield return new object[] { typeof(ushort), "-10", typeof(OverflowException) };
		yield return new object[] { typeof(uint), "-10", typeof(OverflowException) };
		yield return new object[] { typeof(ulong), "-10", typeof(OverflowException) };

		yield return new object[] { typeof(int), "0x0Q", typeof(FormatException) };
		yield return new object[] { typeof(int), "0b012", typeof(FormatException) };
	}

	[TestMethod]
	[DataRow("bread", FoodStuff.Bread)]
	[DataRow("Bread", FoodStuff.Bread)]
	[DataRow("0", FoodStuff.Donut)]
	[DataRow("3", FoodStuff.Fish)]
	public void ParseEnumSuccess(string raw, FoodStuff value)
	{
		FoodStuff food = parser.Parse<FoodStuff>(raw);
		Assert.AreEqual(value, food);
	}

	[TestMethod]
	[DataRow("b", typeof(ArgumentException))]
	[DataRow("-1", typeof(ArgumentException))]
	[DataRow("999", typeof(ArgumentException))]
	public void ParseEnumFail(string raw, Type err)
	{
		Assert.That.ThrowsExceptionType(err, () => {
			FoodStuff food = parser.Parse<FoodStuff>(raw);
		});
	}

	[TestMethod]
	public void ParseStringSuccess()
	{
		string val = parser.Parse<string>("test");
		Assert.AreEqual("test", val);
	}

	[TestMethod]
	[DataRow("", typeof(ArgumentException))]
	[DataRow(null, typeof(ArgumentException))]
	[DataRow(" ", typeof(ArgumentException))]
	public void ParseStringFail(string raw, Type err)
	{
		//parsing should fail
		Assert.That.ThrowsExceptionType(err, () => {
			string val = parser.Parse<string>(raw);
		});
	}
}
