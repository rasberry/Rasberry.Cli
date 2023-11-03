using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestParseParamsDefault
{
	[TestMethod]
	public void TestDefaultOne()
	{
		string[] args = new[] { "-w", "1", "-x", "hi" };
		var p = new ParseParams(args);

		var r1 = p.Default<double>("-w");
		Assert.IsTrue(r1.IsGood());
		Assert.AreEqual(1.0,r1.Value);
		Assert.AreEqual("-w",r1.Name);
		Assert.AreEqual(null, r1.Error);

		var r2 = p.Default<string>("-x");
		Assert.IsTrue(r2.IsGood());
		Assert.AreEqual("hi",r2.Value);
		Assert.AreEqual("-x",r2.Name);
		Assert.AreEqual(null, r2.Error);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 0);
	}

	[TestMethod]
	public void TestDefaultOneMissing()
	{
		string[] args = new[] { "-w", "1", "-x", "hi" };
		var p = new ParseParams(args);

		var r = p.Default<double>("-q");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissing());
		Assert.AreEqual("-q",r.Name);
		Assert.AreEqual(default,r.Value);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestDefaultOneUnParsable()
	{
		string[] args = new[] { "-w", "bad", "-x", "hi" };
		var p = new ParseParams(args);

		var r = p.Default<double>("-w");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsUnParsable());
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual(default,r.Value);
		Assert.AreEqual(typeof(FormatException), r.Error.GetType());
	}

	[TestMethod]
	public void TestDefaultOneRemaining()
	{
		string[] args = new[] { "-w", "1", "-x", "hi" };
		var p = new ParseParams(args);

		var r = p.Default<double>("-w");
		Assert.IsTrue(r.IsGood());
		Assert.AreEqual(1.0,r.Value);
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual(null, r.Error);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 2);
	}

	[TestMethod]
	public void TestDefaultOneMissingArgument()
	{
		string[] args = new[] { "-w" };
		var p = new ParseParams(args);

		var r = p.Default<double>("-w");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual(default,r.Value);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestDefaultMultiOne()
	{
		string[] args = new[] { "-w", "1", "-x", "2" };
		var p = new ParseParams(args);

		var r1 = p.Default<double>(new string[] { "-v", "-w" });
		Assert.IsTrue(r1.IsGood());
		Assert.AreEqual(1.0,r1.Value);
		Assert.AreEqual("-w",r1.Name);
		Assert.AreEqual(null, r1.Error);

		var r2 = p.Default<double>(new string[] { "-x", "-y" });
		Assert.IsTrue(r2.IsGood());
		Assert.AreEqual(2.0,r2.Value);
		Assert.AreEqual("-x",r2.Name);
		Assert.AreEqual(null, r2.Error);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 0);
	}

	[TestMethod]
	public void TestDefaultMultiOneMissing()
	{
		string[] args = new[] { "-w", "1", "-x", "2" };
		var p = new ParseParams(args);

		var r = p.Default<double>(new string[] { "-u", "-v" });
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissing());
		Assert.AreEqual("-u",r.Name);
		Assert.AreEqual(default,r.Value);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestDefaultTwo()
	{
		string[] args = new[] { "-w", "1", "hi", "-x" };
		var p = new ParseParams(args);

		var r = p.Default<double,string>("-w");
		Assert.IsTrue(r.IsGood());
		Assert.AreEqual(1.0,r.Value.Item1);
		Assert.AreEqual("hi",r.Value.Item2);
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual(null, r.Error);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 1);
	}

	[TestMethod]
	public void TestDefaultMultiTwo()
	{
		string[] args = new[] { "-w", "1", "hi", "-x" };
		var p = new ParseParams(args);

		var r = p.Default<double,string>(new[]{"--wide","-w"});
		Assert.IsTrue(r.IsGood());
		Assert.AreEqual(1.0,r.Value.Item1);
		Assert.AreEqual("hi",r.Value.Item2);
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual(null, r.Error);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 1);
	}

	[TestMethod]
	public void TestDefaultTwoMissingArgument()
	{
		string[] args = new[] { "-w", "1",};
		var p = new ParseParams(args);

		var r = p.Default<double,string>("-w");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual((1.0,null),r.Value);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestDefaultTwoConditionSuccess()
	{
		string[] args = new[] { "-w", "1"};
		var p = new ParseParams(args);

		var r = p.Default<double,string>(
			@switch:"-w",
			condition: (double d) => {
				return false; //second argument optional
			}
		);
		Assert.IsTrue(r.IsGood());
		Assert.AreEqual(1.0,r.Value.Item1);
		Assert.AreEqual(null,r.Value.Item2);
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestDefaultTwoConditionFail()
	{
		string[] args = new[] { "-w", "1",};
		var p = new ParseParams(args);

		var r = p.Default<double,string>(
			@switch:"-w",
			condition: (double d) => {
				return true; //second argument required
			}
		);
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual("-w",r.Name);
		Assert.AreEqual((1.0,null),r.Value);
		Assert.AreEqual(null, r.Error);
	}
}