using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rasberry.Cli.Tests;

public enum FoodStuff
{
	Donut = 0,
	Milk = 2,
	Fish = 3,
	Bread = 4,
	BonBons = 5,
	Dumplings = 6
}

public static class Extensions
{
	public static void ThrowsExceptionType(this Assert assert, Type exceptionType, Action action)
	{
		try {
			action();
		}
		catch(Exception e) {
			Assert.AreEqual(exceptionType, e.GetType());
		}
	}
}

#if DEBUG
// This is here to help with the examples in readme.md
class Example
{
	bool ParseInputs(string[] args)
	{
		var p = new ParseParams(args);

		if (p.Has("-a").IsGood()) {
			HasOptionA = true;
		}

		//example using temporary variable
		var rb = p.Default<double>("-b");
		if (rb.IsBad()) { //use IsBad when the parameter is required
			string err = rb.Error == null ? "" : " - " + rb.Error.ToString();
			Console.WriteLine($"something is wrong with your {rb.Name} option{err}");
			return false;
		}
		OptionB = rb.Value;

		//example using 'When' extensions
		if (p.Default<double>("-c")
			.WhenGood(r => { OptionC = r.Value; return r; })
			.WhenMissing(r => { Console.WriteLine($"option {r.Name} is missing"); return r;})
			.WhenUnParsable(r => { Console.WriteLine($"could not parse {r.Name} option - {r.Error}"); return r; })
			.IsInvalid() //use IsInvalid when the parameter is optional
		) {
			return false;
		}

		return true;
	}

	bool HasOptionA;
	double OptionB;
	double OptionC;

	bool ParseExtras(string[] args)
	{
		var p = new ParseParams(args);

		{
		double OptionD;
		var parser = new ParseParams.Parser<double>((string s) => {
			return ExtraParsers.ParseNumberPercent(s);
		});

		if (p.Default("-d", par: parser)
			.WhenGood(r => { OptionD = r.Value; return r;})
			.WhenBad(r => {
				string err = r.Error == null ? "" : " - " + r.Error.ToString();
				Console.WriteLine($"something is wrong with your {r.Name} option{err}");
				return r;
			})
			.IsInvalid()
		) {
			return false;
		}
		}

		{
		System.Drawing.Color MyColor;
		var parser = new ParseParams.Parser<System.Drawing.Color>(ExtraParsers.ParseColor);
		if (p.Default("-c", par: parser)
			.WhenGood(r => { MyColor = r.Value; return r; })
			.IsInvalid()
		) {
			return false;
		}
		}

		{
		FoodStuff Food;
		var parser = new ParseParams.Parser<FoodStuff>((string s) => {
			return ExtraParsers.ParseEnumFirstLetter<FoodStuff>(s, ignoreZero: true);
		});
		if (p.Default("-c", par: parser)
			.WhenGood(r => { Food = r.Value; return r;})
			.IsInvalid()
		) {
			return false;
		}
		}

		{
		IReadOnlyList<int> Seq;
		var parser = new ParseParams.Parser<IReadOnlyList<int>>((string s) => {
			return ExtraParsers.ParseSequence<int>(s,new char[] {','});
		});

		if (p.Default("-s", par: parser)
			.WhenGood(r => { Seq = r.Value; return r; })
			.IsInvalid()
		) {
			return false;
		}

		}

		return true;
	}
}
#endif