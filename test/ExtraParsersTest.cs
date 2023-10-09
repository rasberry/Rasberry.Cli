using Rasberry.Cli;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Rasberry.Cli.Tests;

[TestClass]
public class ExtraParsersTest
{
	[DataTestMethod]
	[DataRow("1%",0.01)]
	[DataRow("100%",1.0)]
	[DataRow("125%",1.25)]
	[DataRow("0.1%",0.001)]
	[DataRow("1e10%",1e8)]
	[DataRow("-1%",-0.01)]
	[DataRow("0.1",0.1)]
	[DataRow("1e10",1e10)]
	public void TryParseNumberPercentSuccess(string raw, double expected)
	{
		bool w = ExtraParsers.TryParseNumberPercent(raw,out double parsed);
		Assert.IsTrue(w);
		Assert.AreEqual(expected,parsed);
	}

	[DataTestMethod]
	[DataRow("a1%")]
	[DataRow("a%")]
	[DataRow("%")]
	[DataRow("")]
	[DataRow(null)]
	[DataRow("10% ")]
	[DataRow("Infinity")]
	public void TryParseNumberPercentFail(string raw)
	{
		bool w = ExtraParsers.TryParseNumberPercent(raw,out double _);
		Assert.IsFalse(w);
	}

	[DataTestMethod]
	[DataRow(false,"b",FoodStuff.Bread)]
	[DataRow(false,"B",FoodStuff.Bread)]
	[DataRow(false,"Bread",FoodStuff.Bread)]
	[DataRow(false,"BonBons",FoodStuff.BonBons)]
	[DataRow(false,"d",FoodStuff.Donut)]
	[DataRow(false,"0",FoodStuff.Donut)]
	[DataRow(false,"4",FoodStuff.Bread)]
	[DataRow(true,"d",FoodStuff.Dumplings)]
	public void TryParseEnumFirstLetterSuccess(bool igz, string raw, FoodStuff expected)
	{
		bool w = ExtraParsers.TryParseEnumFirstLetter(raw,out FoodStuff parsed,igz);
		Assert.IsTrue(w);
		Assert.AreEqual(expected,parsed);
	}

	[DataTestMethod]
	[DataRow(false,"q")]
	[DataRow(true,"0")]
	[DataRow(true,"Donut")]
	public void TryParseEnumFirstLetterFail(bool igz, string raw)
	{
		bool w = ExtraParsers.TryParseEnumFirstLetter(raw,out FoodStuff _, igz);
		Assert.IsFalse(w);
	}

	[DataTestMethod]
	[DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
	public void TryParseSequenceSuccess(string raw, FoodStuff[] expected)
	{
		var parser = new ParseParams.Parser<FoodStuff>((string arg, out FoodStuff val) => {
			return ExtraParsers.TryParseEnumFirstLetter(arg,out val);
		});

		bool w = ExtraParsers.TryParseSequence(raw,new char[] { ' ' }, out var list,parser);
		Assert.IsTrue(w);
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

	[DataTestMethod]
	[DynamicData(nameof(GetFailData), DynamicDataSourceType.Method)]
	public void TryParseSequenceFail(string raw)
	{
		var parser = new ParseParams.Parser<FoodStuff>((string arg, out FoodStuff val) => {
			return ExtraParsers.TryParseEnumFirstLetter(arg,out val);
		});

		bool w = ExtraParsers.TryParseSequence(raw,new char[] { ' ' }, out var _, parser);
		Assert.IsFalse(w);
	}

	static IEnumerable<object[]> GetFailData()
	{
		yield return new object[] { "q" };
	}
}