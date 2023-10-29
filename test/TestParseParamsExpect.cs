using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestParseParamsExpect
{
	[TestMethod]
	public void TestExpectNone()
	{
		string[] args = new[] { "1",};
		var p = new ParseParams(args);

		var r = p.ExpectValue<double>();
		Assert.IsTrue(r.IsGood());
		Assert.AreEqual(1.0,r.Value);
		Assert.AreEqual(null,r.Name);
	}

	[TestMethod]
	public void TestExpectNoneMissingArgument()
	{
		string[] args = Array.Empty<string>();
		var p = new ParseParams(args);

		var r = p.ExpectValue<double>();
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual(null,r.Name);
	}

	[TestMethod]
	public void TestExpectNoneUnParsable()
	{
		string[] args = new[] { "test" };
		var p = new ParseParams(args);

		var r = p.ExpectValue<double>();
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsUnParsable());
		Assert.AreEqual(null,r.Name);
	}

	[TestMethod]
	public void TestExpectHas()
	{
		string[] args = new[] { "-x", "-w" };
		var p = new ParseParams(args);

		var r1 = p.Expect("-w");
		Assert.IsTrue(r1.IsGood());
		Assert.AreEqual("-w",r1.Name);
		Assert.AreEqual(true,r1.Value);

		var r2 = p.Expect("-x");
		Assert.IsTrue(r2.IsGood());
		Assert.AreEqual("-x",r2.Name);
		Assert.AreEqual(true,r2.Value);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 0);
	}

	[TestMethod]
	public void TestExpectHasMissingArgument()
	{
		string[] args = new[] { "-w", "-x" };
		var p = new ParseParams(args);

		var r = p.Expect("-q");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual("-q",r.Name);
		Assert.AreEqual(default,r.Value);
	}

	[TestMethod]
	public void TestExpectOne()
	{
		string[] args = new[] { "-x", "2", "-w", "1"};
		var p = new ParseParams(args);

		var r1 = p.Expect<double>("-w");
		Assert.IsTrue(r1.IsGood());
		Assert.AreEqual(1.0,r1.Value);
		Assert.AreEqual("-w",r1.Name);

		var r2 = p.Expect<double>("-x");
		Assert.IsTrue(r2.IsGood());
		Assert.AreEqual(2.0,r2.Value);
		Assert.AreEqual("-x",r2.Name);
	}

	[TestMethod]
	public void TestExpectOneMissingArgument()
	{
		string[] args = new[] { "-x", "2", "-w", "1"};
		var p = new ParseParams(args);

		var r = p.Expect<double>("-q");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual("-q",r.Name);
		Assert.AreEqual(default,r.Value);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestExpectTwo()
	{
		string[] args = new[] { "-x", "2", "1"};
		var p = new ParseParams(args);

		var r = p.Expect<double,double>("-x");
		Assert.IsTrue(r.IsGood());
		Assert.AreEqual(2.0,r.Value.Item1);
		Assert.AreEqual(1.0,r.Value.Item2);
		Assert.AreEqual("-x",r.Name);
		Assert.AreEqual(null, r.Error);
	}

	[TestMethod]
	public void TestExpectMissingArgument()
	{
		string[] args = new[] { "-x", "2" };
		var p = new ParseParams(args);

		var r = p.Expect<double,double>("-x");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissingArgument());
		Assert.AreEqual("-x",r.Name);
		Assert.AreEqual((2.0,default),r.Value);
		Assert.AreEqual(null, r.Error);
	}
}