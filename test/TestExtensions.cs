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
			.WhenBad((n) => {isBad = true;})
			.WhenGood((n) => {isGood = true;})
			.WhenMissing((n) => {isMissing = true; })
			.WhenInvalid((n) => {isInvalid = true; })
			.WhenUnParsable((n) => {isUnParsable = true; })
			.WhenMissingArgument((n) => {isMissingArgument = true; })
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
			.WhenBad((n) => {isBad = true;})
			.WhenGood((n) => {isGood = true;})
			.WhenMissing((n) => {isMissing = true; })
			.WhenInvalid((n) => {isInvalid = true; })
			.WhenUnParsable((n) => {isUnParsable = true; })
			.WhenMissingArgument((n) => {isMissingArgument = true; })
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
			.WhenBad((n) => {isBad = true;})
			.WhenGood((n) => {isGood = true;})
			.WhenMissing((n) => {isMissing = true; })
			.WhenInvalid((n) => {isInvalid = true; })
			.WhenUnParsable((n) => {isUnParsable = true; })
			.WhenMissingArgument((n) => {isMissingArgument = true; })
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
			.WhenBad((n) => {isBad = true;})
			.WhenGood((n) => {isGood = true;})
			.WhenMissing((n) => {isMissing = true; })
			.WhenInvalid((n) => {isInvalid = true; })
			.WhenUnParsable((n) => {isUnParsable = true; })
			.WhenMissingArgument((n) => {isMissingArgument = true; })
			.IsBad();

		Assert.IsTrue(check);
		Assert.IsTrue(isBad);
		Assert.IsFalse(isGood);
		Assert.IsFalse(isMissing);
		Assert.IsTrue(isInvalid);
		Assert.IsFalse(isUnParsable);
		Assert.IsTrue(isMissingArgument);
	}
}