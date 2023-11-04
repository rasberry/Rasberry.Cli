using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestExtensions
{
	[TestMethod]
	public void TestWhenSuccess()
	{
		string[] args = new[] { "-x", "2", "-w", "1"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		var check = p.Default<double>("-x")
			.WhenBad(n => {isBad = true; return n;})
			.WhenGood(n => {isGood = true; return n;})
			.WhenMissing(n => {isMissing = true; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => {isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsFalse(check);
		Assert.IsFalse(isBad);
		Assert.IsTrue(isGood);
		Assert.IsFalse(isMissing);
		Assert.IsFalse(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsFalse(isMissingArgument);
	}

	[TestMethod]
	public void TestWhenFail()
	{
		string[] args = new[] { "-x", "a", "-w", "1"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		var check = p.Default<double>("-x")
			.WhenBad(n => {isBad = true; return n;})
			.WhenGood(n => {isGood = true; return n;})
			.WhenMissing(n => {isMissing = true; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => {isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsFalse(isMissing);
		Assert.IsTrue(isInvalid);
		Assert.IsTrue(isUnParsable);
		Assert.IsFalse(isMissingArgument);
	}

	[TestMethod]
	public void TestWhenMissing()
	{
		string[] args = new[] { "-x", "a", "-w", "1"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		var check = p.Default<double>("-q")
			.WhenBad(n => {isBad = true; return n;})
			.WhenGood(n => {isGood = true; return n;})
			.WhenMissing(n => {isMissing = true; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => {isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsTrue(isMissing);
		Assert.IsFalse(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsFalse(isMissingArgument);
	}

	[TestMethod]
	public void TestWhenMissingArgument()
	{
		string[] args = new[] { "-x", "2"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		var check = p.Default<double,double>("-x")
			.WhenBad(n => {isBad = true; return n;})
			.WhenGood(n => {isGood = true; return n;})
			.WhenMissing(n => {isMissing = true; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => {isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsFalse(isMissing);
		Assert.IsTrue(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsTrue(isMissingArgument);
	}

	[TestMethod]
	public void TestWhenExpectFail()
	{
		string[] args = new[] { "-x", "2"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		var check = p.Default<double>("-z")
			.WhenBad(n => {isBad = true; return n;})
			.WhenGood(n => {isGood = true; return n;})
			.WhenMissing(n => {isMissing = true; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => {isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsTrue(isMissing);
		Assert.IsFalse(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsFalse(isMissingArgument);
	}

	[TestMethod]
	public void TestWhenChangeResult()
	{
		string[] args = new[] { "-x", "2"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		var check = p.Default<double>("-z")
			.WhenBad(n => {
				isBad = true; return n with { Result = ParseParams.Result.Good };
			})
			.WhenGood(n => {
				isGood = true; return n with { Result = ParseParams.Result.UnParsable };
			})
			.WhenMissing(n => {isMissing = true; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => { isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsTrue(check); //the second update is back to bad
		Assert.IsTrue(isBad); //this was set before the first update
		Assert.IsTrue(isGood); //this was set because the result was Good at that point
		Assert.IsFalse(isMissing);
		Assert.IsTrue(isInvalid);
		Assert.IsTrue(isUnParsable);
		Assert.IsFalse(isMissingArgument);
	}
}