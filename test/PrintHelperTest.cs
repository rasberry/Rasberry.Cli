using Rasberry.Cli;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;

namespace Rasberry.Cli.Tests
{
	[TestClass]
	public class PrintHelperTest
	{
		[TestInitialize]
		public void Init()
		{
			PrintHelper.DescriptionColumn = 10;
			PrintHelper.OutputWidth = 20;
		}

		[TestMethod]
		public void TestNDZero()
		{
			var sb = new StringBuilder();
			PrintHelper.ND(sb,0,"-w","desc");
			Assert.AreEqual(1,CountNL(sb));
			Assert.AreEqual("-w        desc",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestNDZeroNoDesc()
		{
			var sb = new StringBuilder();
			PrintHelper.ND(sb,0,"-w");
			Assert.AreEqual(1,CountNL(sb));
			Assert.AreEqual("-w",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestNDZeroDescWS()
		{
			var sb = new StringBuilder();
			PrintHelper.ND(sb,0,"-w","   ");
			Assert.AreEqual(1,CountNL(sb));
			Assert.AreEqual("-w",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestNDOne()
		{
			var sb = new StringBuilder();
			PrintHelper.ND(sb,1,"-w","desc");
			Assert.AreEqual(1,CountNL(sb));
			Assert.AreEqual(" -w       desc",Fix(sb).ToString());
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void TestNDOneNullName()
		{
			var sb = new StringBuilder();
			PrintHelper.ND(sb,1,null,"desc");
		}

		[TestMethod]
		public void TestWTText()
		{
			var sb = new StringBuilder();
			PrintHelper.WT(sb,1,"test");
			Assert.AreEqual(1,CountNL(sb));
			Assert.AreEqual(" test",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestWTText2Lines()
		{
			var sb = new StringBuilder();
			PrintHelper.WT(sb,1,"test abcdefghijklmnopqrstuvwxyz");
			Assert.AreEqual(2,CountNL(sb));
			Assert.AreEqual(" test abcdefghijklm nopqrstuvwxyz",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestWTText4Lines()
		{
			var sb = new StringBuilder();
			PrintHelper.WT(sb,2,"test abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			Assert.AreEqual(4,CountNL(sb));
			Assert.AreEqual("  test abcdefghijkl  mnopqrstuvwxyzABC  DEFGHIJKLMNOPQRST  UVWXYZ",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestEnumAll()
		{
			var list = PrintHelper.EnumAll<FoodStuff>();
			var expected = new List<FoodStuff> {
				FoodStuff.Donut,
				FoodStuff.Milk,
				FoodStuff.Fish,
				FoodStuff.Bread,
				FoodStuff.BonBons,
				FoodStuff.Dumplings
			};
			var val = new List<FoodStuff>(list);
			CollectionAssert.AreEqual(expected,val);
		}

		[TestMethod]
		public void TestEnumAllNoZero()
		{
			var list = PrintHelper.EnumAll<FoodStuff>(true);
			var expected = new List<FoodStuff> {
				FoodStuff.Milk,
				FoodStuff.Fish,
				FoodStuff.Bread,
				FoodStuff.BonBons,
				FoodStuff.Dumplings
			};
			var val = new List<FoodStuff>(list);
			CollectionAssert.AreEqual(expected,val);
		}

		[TestMethod]
		public void TestPrintEnum()
		{
			var sb = new StringBuilder();
			PrintHelper.PrintEnum<FoodStuff>(sb,1);
			Assert.AreEqual(6,CountNL(sb));
			Assert.AreEqual(" 0. Donut 2. Milk 3. Fish 4. Bread 5. BonBons 6. Dumplings",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestPrintEnumNoZero()
		{
			var sb = new StringBuilder();
			PrintHelper.PrintEnum<FoodStuff>(sb,1,excludeZero:true);
			Assert.AreEqual(5,CountNL(sb));
			Assert.AreEqual(" 2. Milk 3. Fish 4. Bread 5. BonBons 6. Dumplings",Fix(sb).ToString());
		}

		[TestMethod]
		public void TestPrintEnumDMap()
		{
			var sb = new StringBuilder();
			PrintHelper.PrintEnum(sb,1,(FoodStuff f) => {
				if (f == FoodStuff.Milk) {
					return "nutrient-rich liquid";
				}
				return "";
			});

			string expected = ""
 				+" 0. Donut"
				+" 2. Milk  nutrient-"
				+"          rich liqu"
				+"          id"
				+" 3. Fish"
				+" 4. Bread"
				+" 5. BonBons"
				+" 6. Dumplings"
			;
			Assert.AreEqual(8,CountNL(sb));
			Assert.AreEqual(expected,Fix(sb).ToString());
		}

		[TestMethod]
		public void TestPrintEnumNMap()
		{
			var sb = new StringBuilder();
			PrintHelper.PrintEnum(sb,1,null,(FoodStuff f) => {
				if (f == FoodStuff.BonBons) {
					return "NumNums";
				}
				return f.ToString();
			},true);
			Assert.AreEqual(" 2. Milk 3. Fish 4. Bread 5. NumNums 6. Dumplings",Fix(sb).ToString());
		}

		static StringBuilder Fix(StringBuilder sb)
		{
			sb
				.Replace("\n","")
				.Replace("\r","")
			;
			return sb;
		}

		static int CountNL(StringBuilder sb)
		{
			int count = 0;
			for(int c=0; c<sb.Length; c++) {
				char cc = sb[c];
				if (cc == '\n' || cc == '\r') {
					count++;
					//check if there's a new line char immediately following it
					cc = sb[++c];
					if (cc == '\n' || cc == '\r') {
						c++; //eat the extra
					}
				}
			}
			return count;
		}
	}
}