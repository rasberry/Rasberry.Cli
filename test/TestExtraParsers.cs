using Rasberry.Cli;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestExtraParsers
{
	[TestMethod]
	[DataRow("1%",    0.01)]
	[DataRow("100%",  1.0)]
	[DataRow("125%",  1.25)]
	[DataRow("0.1%",  0.001)]
	[DataRow("1e10%", 1e8)]
	[DataRow("-1%",   -0.01)]
	[DataRow("0.1",   0.1)]
	[DataRow("1e10",  1e10)]
	public void ParseNumberPercentSuccess(string raw, double expected)
	{
		double parsed = ExtraParsers.ParseNumberPercent(raw);
		Assert.AreEqual(expected,parsed);
	}

	[TestMethod]
	[DataRow("a1%",      typeof(FormatException))]
	[DataRow("a%",       typeof(FormatException))]
	[DataRow("%",        typeof(FormatException))]
	[DataRow("",         typeof(FormatException))]
	[DataRow(null,       typeof(ArgumentNullException))]
	[DataRow("10% ",     typeof(FormatException))]
	[DataRow("Infinity", typeof(ArgumentOutOfRangeException))]
	public void ParseNumberPercentFail(string raw, Type err)
	{
		Assert.That.ThrowsExceptionType(err, () => {
			double val = ExtraParsers.ParseNumberPercent(raw);
		});
	}

	[TestMethod]
	[DataRow(false,"b",      FoodStuff.Bread)]
	[DataRow(false,"B",      FoodStuff.Bread)]
	[DataRow(false,"Bread",  FoodStuff.Bread)]
	[DataRow(false,"BonBons",FoodStuff.BonBons)]
	[DataRow(false,"d",      FoodStuff.Donut)]
	[DataRow(false,"0",      FoodStuff.Donut)]
	[DataRow(false,"4",      FoodStuff.Bread)]
	[DataRow(true ,"d",      FoodStuff.Dumplings)]
	public void ParseEnumFirstLetterSuccess(bool igz, string raw, FoodStuff expected)
	{
		FoodStuff parsed = ExtraParsers.ParseEnumFirstLetter<FoodStuff>(raw,igz);
		Assert.AreEqual(expected,parsed);
	}

	[TestMethod]
	[DataRow(false,"q",     typeof(ArgumentOutOfRangeException))]
	[DataRow(true ,"0",     typeof(ArgumentOutOfRangeException))]
	[DataRow(true ,"Donut", typeof(ArgumentOutOfRangeException))]
	public void ParseEnumFirstLetterFail(bool igz, string raw, Type err)
	{
		Assert.That.ThrowsExceptionType(err, () => {
			FoodStuff parsed = ExtraParsers.ParseEnumFirstLetter<FoodStuff>(raw,igz);
		});
	}

	[TestMethod]
	[DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
	public void ParseSequenceSuccess(string raw, FoodStuff[] expected)
	{
		var parser = new ParseParams.Parser<FoodStuff>((string arg) => {
			return ExtraParsers.ParseEnumFirstLetter<FoodStuff>(arg);
		});

		var list = ExtraParsers.ParseSequence(raw,new char[] { ' ' }, parser);
		var wrapList = new List<FoodStuff>(list);
		CollectionAssert.AreEqual(expected,wrapList);
	}

	static IEnumerable<object[]> GetSuccessData()
	{
		yield return new object[] {
			"b d f m BonBons",
			new[] { FoodStuff.Bread, FoodStuff.Donut, FoodStuff.Fish, FoodStuff.Milk, FoodStuff.BonBons }
		};
	}

	[TestMethod]
	[DynamicData(nameof(GetFailData), DynamicDataSourceType.Method)]
	public void ParseSequenceFail(string raw, Type err)
	{
		var parser = new ParseParams.Parser<FoodStuff>((string arg) => {
			return ExtraParsers.ParseEnumFirstLetter<FoodStuff>(arg);
		});

		Assert.That.ThrowsExceptionType(err, () => {
			var list = ExtraParsers.ParseSequence(raw,new char[] { ' ' }, parser);
		});
	}

	static IEnumerable<object[]> GetFailData()
	{
		yield return new object[] { "q", typeof(ArgumentOutOfRangeException) };
	}

	[TestMethod]
	[DataRow("red", System.Drawing.KnownColor.Red)]
	[DataRow("F00", System.Drawing.KnownColor.Red)]
	[DataRow("F00F", System.Drawing.KnownColor.Red)]
	[DataRow("FF0000", System.Drawing.KnownColor.Red)]
	[DataRow("#FF0000", System.Drawing.KnownColor.Red)]
	[DataRow("#FF0000FF", System.Drawing.KnownColor.Red)]
	public void ParseColorSuccess(string raw, System.Drawing.KnownColor expected)
	{
		var color = ExtraParsers.ParseColor(raw);
		var ex = System.Drawing.Color.FromKnownColor(expected);

		//System.Drawing.Color.Equals does not compare strictly on color values
		// it treats FromName('red') different than FromArgb(255,255,0,0);
		// so comparing components instead
		Assert.AreEqual(ex.A, color.A);
		Assert.AreEqual(ex.R, color.R);
		Assert.AreEqual(ex.G, color.G);
		Assert.AreEqual(ex.B, color.B);
	}

	[TestMethod]
	[DataRow("badcolor", typeof(FormatException))]
	[DataRow("FF0000 ", typeof(ArgumentException))]
	[DataRow("#FF00Z0", typeof(FormatException))]
	[DataRow(null, typeof(ArgumentNullException))]
	[DataRow("-F0000", typeof(FormatException))]
	[DataRow("FF00-0", typeof(FormatException))]
	[DataRow("FF00.0", typeof(FormatException))]
	public void ParseColorFail(string raw, Type err)
	{
		Assert.That.ThrowsExceptionType(err, () => {
			var color = ExtraParsers.ParseColor(raw);
		});
	}
}