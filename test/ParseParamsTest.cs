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
		public void TestHas()
		{
			string[] args = new[] { "-w", "-x" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Has("-w");
			Assert.IsTrue(r.IsGood());

			r = p.Has("-x");
			Assert.IsTrue(r.IsGood());

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 0);
		}

		[TestMethod]
		public void TestHasMissing()
		{
			string[] args = new[] { "-w", "-x" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Has("-q");
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissing());
		}

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

		[TestMethod]
		public void TestDefaultTwo()
		{
			string[] args = new[] { "-w", "1", "hi", "-x" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-w", out double num, out string s);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);
			Assert.AreEqual("hi",s);

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 1);
		}

		[TestMethod]
		public void TestDefaultMultiTwo()
		{
			string[] args = new[] { "-w", "1", "hi", "-x" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default(new[]{"--wide","-w"}, out double num, out string s);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);
			Assert.AreEqual("hi",s);

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 1);
		}

		[TestMethod]
		public void TestDefaultTwoMissingArgument()
		{
			string[] args = new[] { "-w", "1",};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default("-w", out double num, out string s);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}

		[TestMethod]
		public void TestDefaultTwoConditionSuccess()
		{
			string[] args = new[] { "-w", "1",};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default(@switch:"-w",
				leftVal:out double num, rightVal:out string s,
				condition: (double d) => {
					return false; //second argument optional
				}
			);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);
			Assert.AreEqual(null,s);
		}

		[TestMethod]
		public void TestDefaultTwoConditionFail()
		{
			string[] args = new[] { "-w", "1",};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Default(@switch:"-w",
				leftVal:out double num, rightVal:out string s,
				condition: (double d) => {
					return true; //second argument required
				}
			);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}

		[TestMethod]
		public void TestExpectNone()
		{
			string[] args = new[] { "1",};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect(out double num);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,num);
		}

		[TestMethod]
		public void TestExpectNoneMissingArgument()
		{
			string[] args = Array.Empty<string>();
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect(out double _);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}

		[TestMethod]
		public void TestExpectNoneUnParsable()
		{
			string[] args = new[] { "test" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect(out double _);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsUnParsable());
		}

		[TestMethod]
		public void TestExpectHas()
		{
			string[] args = new[] { "-x", "-w" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect("-w");
			Assert.IsTrue(r.IsGood());

			r = p.Expect("-x");
			Assert.IsTrue(r.IsGood());

			var rem = p.Remaining();
			Assert.IsTrue(rem.Length == 0);
		}

		[TestMethod]
		public void TestExpectHasMissingArgument()
		{
			string[] args = new[] { "-w", "-x" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect("-q");
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}

		[TestMethod]
		public void TestExpectOne()
		{
			string[] args = new[] { "-x", "2", "-w", "1"};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect("-w",out double one);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(1.0,one);

			r = p.Expect("-x",out double two);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(2.0,two);
		}

		[TestMethod]
		public void TestExpectOneMissingArgument()
		{
			string[] args = new[] { "-x", "2", "-w", "1"};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect("-q",out double one);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}

		[TestMethod]
		public void TestExpectTwo()
		{
			string[] args = new[] { "-x", "2", "1"};
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect("-x",out double two, out double one);
			Assert.IsTrue(r.IsGood());
			Assert.AreEqual(2.0,two);
			Assert.AreEqual(1.0,one);
		}

		[TestMethod]
		public void TestExpectMissingArgument()
		{
			string[] args = new[] { "-x", "2" };
			var p = new ParseParams(args);

			ParseParams.Result r;
			r = p.Expect("-x",out double two, out double one);
			Assert.IsTrue(r.IsBad());
			Assert.IsTrue(r.IsMissingArgument());
		}
	}
}