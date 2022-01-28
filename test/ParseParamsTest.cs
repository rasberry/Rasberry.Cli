using Rasberry.Cli;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Rasberry.Cli.Tests
{
	[TestClass]
	public class ParseParamsTest
	{
		[TestMethod]
		public void TestDefaultOne()
		{
			string[] args = new[] { "-w", "1", "-x", "hi" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-w", out double num);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);

			r = p.Default("-x", out string s);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual("hi",s);

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 0);
		}

		[TestMethod]
		public void TestDefaultOneMissing()
		{
			string[] args = new[] { "-w", "1", "-x", "hi" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-q", out double num);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissing());
		}

		[TestMethod]
		public void TestDefaultOneUnParsable()
		{
			string[] args = new[] { "-w", "bad", "-x", "hi" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-w", out double num);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsUnParsable());
		}

		[TestMethod]
		public void TestDefaultOneRemaining()
		{
			string[] args = new[] { "-w", "1", "-x", "hi" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-w", out double num);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 2);
		}

		[TestMethod]
		public void TestDefaultOneMissingArgument()
		{
			string[] args = new[] { "-w" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-w", out double num);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}

		[TestMethod]
		public void TestDefaultMultiOne()
		{
			string[] args = new[] { "-w", "1", "-x", "2" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default(new string[] { "-v", "-w" }, out double num);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);

			r = p.Default(new string[] { "-x", "-y" }, out double mun);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(2.0,mun);

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 0);
		}

		[TestMethod]
		public void TestDefaultMultiOneMissing()
		{
			string[] args = new[] { "-w", "1", "-x", "2" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default(new string[] { "-u", "-v" }, out double num);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissing());
		}
	}
}