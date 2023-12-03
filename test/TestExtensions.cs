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

		var check = p.Scan<double>("-x")
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

		var check = p.Scan<double>("-x")
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

		var check = p.Scan<double>("-q")
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

		var check = p.Scan<double,double>("-x")
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

		var check = p.Scan<double>("-z")
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

		var check = p.Scan<double>("-z")
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

	[TestMethod]
	public void TestWhenSetDefault()
	{
		string[] args = new[] { "-x", "2"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		double val = 0.0;
		var check = p.Scan<double>("-z", 3.0)
			.WhenBad(n => { isBad = true; return n; })
			.WhenGood(n => { isGood = true; val = n.Value; return n; })
			.WhenMissing(n => {isMissing = true; val = n.Value; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => { isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsTrue(isMissing);
		Assert.IsFalse(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsFalse(isMissingArgument);
		Assert.AreEqual(3.0, val);
	}

	[TestMethod]
	public void TestWhenSetValue()
	{
		string[] args = new[] { "-x", "2"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		double val = 0.0;
		var check = p.Scan<double>("-x", 3.0)
			.WhenBad(n => { isBad = true; return n; })
			.WhenGood(n => { isGood = true; val = n.Value; return n; })
			.WhenMissing(n => {isMissing = true; val = n.Value; return n;})
			.WhenInvalid(n => {isInvalid = true; return n;})
			.WhenUnParsable(n => {isUnParsable = true; return n;})
			.WhenMissingArgument(n => { isMissingArgument = true; return n;})
			.IsBad();

		Assert.IsFalse(check);
		Assert.IsFalse(isBad);
		Assert.IsTrue(isGood);
		Assert.IsFalse(isMissing);
		Assert.IsFalse(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsFalse(isMissingArgument);
		Assert.AreEqual(2.0, val);
	}

	[TestMethod]
	public void TestWhenCustom()
	{
		string[] args = new[] { "-x", "2"};
		var p = new ParseParams(args);

		bool isBad = false;
		bool isGood = false;
		bool isMissing = false;
		bool isInvalid = false;
		bool isUnParsable = false;
		bool isMissingArgument = false;

		double val = 0.0;
		var check = p.Scan<double>("-z", 3.0)
			.When(c => c.IsGood() || c.IsMissing(), n => {
				isBad = n.IsBad();
				isGood = n.IsGood();
				isMissing = n.IsMissing();
				isInvalid = n.IsInvalid();
				isUnParsable = n.IsUnParsable();
				isMissingArgument = n.IsMissingArgument();
				val = n.Value;
				return n;
			})
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsTrue(isMissing);
		Assert.IsFalse(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsFalse(isMissingArgument);
		Assert.AreEqual(3.0, val);
	}
}