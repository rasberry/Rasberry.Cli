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
		Assert.IsTrue(r1.Value);
		Assert.IsNull(r1.Error);

		var r2 = p.Has("-x");
		Assert.IsTrue(r2.IsGood());
		Assert.AreEqual("-x",r2.Name);
		Assert.IsTrue(r2.Value);
		Assert.IsNull(r2.Error);

		var rem = p.Remaining();
		Assert.IsEmpty(rem);
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
		Assert.IsFalse(r.Value);
		Assert.IsNull(r.Error);
	}
}