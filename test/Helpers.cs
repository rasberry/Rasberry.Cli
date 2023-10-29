using System;
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