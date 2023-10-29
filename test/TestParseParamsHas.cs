using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rasberry.Cli.Tests;

[TestClass]
public class TestParseParamsHas
{
	[TestMethod]
	public void TestHas()
	{
		string[] args = new[] { "-w", "-x" };
		var p = new ParseParams(args);

		var r1 = p.Has("-w");
		Assert.IsTrue(r1.IsGood());
		Assert.AreEqual("-w",r1.Name);
		Assert.AreEqual(true,r1.Value);

		var r2 = p.Has("-x");
		Assert.IsTrue(r2.IsGood());
		Assert.AreEqual("-x",r2.Name);
		Assert.AreEqual(true,r2.Value);

		var rem = p.Remaining();
		Assert.IsTrue(rem.Length == 0);
	}

	[TestMethod]
	public void TestHasMissing()
	{
		string[] args = new[] { "-w", "-x" };
		var p = new ParseParams(args);

		var r = p.Has("-q");
		Assert.IsTrue(r.IsBad());
		Assert.IsTrue(r.IsMissing());
		Assert.AreEqual("-q",r.Name);
		Assert.AreEqual(false,r.Value);
	}
}