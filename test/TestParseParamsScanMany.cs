using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestParseParamsScanMany
{
	[TestMethod]
	public void TestScanManyNormal()
	{
		string[] args = new[] { "-w", "1", "-w", "2", "-w", "3" };
		var p = new ParseParams(args);

		var res = p.ScanMany<int>("-w");
		Assert.AreEqual(ParseParams.Result.Good, res.Result);

		var items = res.Value.ToList();
		Assert.HasCount(3, items);
		Assert.AreEqual(1, items[0]);
		Assert.AreEqual(2, items[1]);
		Assert.AreEqual(3, items[2]);
		Assert.IsEmpty(p.Remaining());
	}

	[TestMethod]
	public void TestScanManySeveralNames()
	{
		string[] args = new[] { "-ww", "1", "-w", "2", "-w", "3", "-ww", "4" };
		var p = new ParseParams(args);

		var res = p.ScanMany<int>(new string[] { "-w", "-ww" });
		Assert.AreEqual(ParseParams.Result.Good, res.Result);

		var items = res.Value.ToList();
		Assert.HasCount(4, items);
		Assert.AreEqual(1, items[0]);
		Assert.AreEqual(2, items[1]);
		Assert.AreEqual(3, items[2]);
		Assert.AreEqual(4, items[3]);
		Assert.IsEmpty(p.Remaining());
	}

	[TestMethod]
	public void TestScanManyMissingArgument()
	{
		string[] args = new[] { "-ww", "1", "-w", "2", "-w", "3", "-ww" };
		var p = new ParseParams(args);

		var res = p.ScanMany<int>(new string[] { "-w", "-ww" });
		Assert.AreEqual(ParseParams.Result.MissingArgument, res.Result);
		Assert.IsNull(res.Value);
		Assert.IsNull(res.Error);
	}

	[TestMethod]
	public void TestScanManyBadNames()
	{
		string[] args = new[] { "-ww", "1", "-w", "2", "-w", "3", "-ww" };
		var p = new ParseParams(args);

		Assert.Throws<ArgumentOutOfRangeException>(() => {
			var res = p.ScanMany<int>(new string[0]);
		});

		string[] names = null;
		Assert.Throws<ArgumentNullException>(() => {
			var res = p.ScanMany<int>(names);
		});
	}

	[TestMethod]
	public void TestScanManyBadValues()
	{
		string[] args = new[] { "-ww", "bad1", "-w", "2", "-w", "3", "-ww", "bad4" };
		var p = new ParseParams(args);

		var res = p.ScanMany<int>(new string[] { "-w", "-ww" });
		Assert.AreEqual(ParseParams.Result.UnParsable, res.Result);
		Assert.IsNull(res.Value);
		Assert.IsInstanceOfType<FormatException>(res.Error);
	}

	[TestMethod]
	public void TestScanManyNoValues()
	{
		string[] args = new[] { "-x", "1", };
		var p = new ParseParams(args);

		var res = p.ScanMany<int>("-w");
		Assert.AreEqual(ParseParams.Result.Missing, res.Result);
		Assert.IsNull(res.Value);
		Assert.IsNull(res.Error);
	}
}
