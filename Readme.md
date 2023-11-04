# Rasberry.Cli
A library with some helpful functions for command line programs.

Features include:
* Functions for processing command line arguments.
* Tools to display a help message.
* A progress bar

## How to use
### Printing a help message
```csharp
void PrintHelp()
{
	var sb = new StringBuilder();
	sb.WT(0,$"{nameof(PrintHelp)} [options]");
	sb.WT(0,"Options:");
	sb.ND(1,"-a",           "an option that does something");
	sb.ND(1,"-b (number)",  "an option which requires a number");
	sb.ND(1,"-c (number)",  "another option which requires a number");
	sb.WT(1,"An explanation of some kind");
}
```

### Parsing inputs
```csharp
bool ParseInputs(string[] args)
{
	var p = new ParseParams(args);

	if (p.Has("-a").IsGood()) {
		HasOptionA = true;
	}

	//example using temporary variable
	var rb = p.Scan<double>("-b");
	if (rb.IsBad()) { //use IsBad when the parameter is required
		string err = rb.Error == null ? "" : " - " + rb.Error.ToString();
		Console.WriteLine($"something is wrong with your {rb.Name} option{err}");
		return false;
	}
	OptionB = rb.Value;

	//example using 'When' extensions
	if (p.Scan<double>("-c")
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
```

### Displaying progress
```csharp
Console.Write("Performing some task... ");
using (var progress = new ProgressBar()) {
	for (int i = 0; i <= 100; i++) {
		progress.Report((double) i / 100);
		Thread.Sleep(20);
	}
}
```

### Extra parsers
There are several included additional parsers which can be optionally used.

#### ExtraParsers.ParseColor
ParseColor is used to parse color names and hex-style colors. For example red and #F00 result in the Color.Red value

```csharp
System.Drawing.Color MyColor;
var parser = new ParseParams.Parser<System.Drawing.Color>(ExtraParsers.ParseColor);
if (p.Scan("-c", par: parser)
	.WhenGood(r => { MyColor = r.Value; return r; })
	.IsInvalid()
) {
	return false;
}
```

#### ExtraParsers.ParseEnumFirstLetter
ParseEnumFirstLetter can parse enums from either their numeric value, first letter, or name

```csharp
FoodStuff Food;
var parser = new ParseParams.Parser<FoodStuff>((string s) => {
	return ExtraParsers.ParseEnumFirstLetter<FoodStuff>(s, ignoreZero: true);
});
if (p.Scan("-c", par: parser)
	.WhenGood(r => { Food = r.Value; return r;})
	.IsInvalid()
) {
	return false;
}
```
#### ExtraParsers.ParseNumberPercent
ParseNumberPercent can parse numbers as percentages. For example 0.1 and 10% produce the same value

```csharp
double OptionD;
var parser = new ParseParams.Parser<double>((string s) => {
	return ExtraParsers.ParseNumberPercent(s);
});

if (p.Scan("-d", par: parser)
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
```
#### ExtraParsers.ParseSequence
ParseSequence can be used to parse a single input into a collection of items. For example 1,2,3,4

```csharp
IReadOnlyList<int> Seq;
var parser = new ParseParams.Parser<IReadOnlyList<int>>((string s) => {
	return ExtraParsers.ParseSequence<int>(s,new char[] {','});
});

if (p.Scan("-s", par: parser)
	.WhenGood(r => { Seq = r.Value; return r; })
	.IsInvalid()
) {
	return false;
}
```

## Additional Notes
### Parsing
* Parsing numbers to number types (int,float,etc..) uses NumberStyles.Any (see [details](https://docs.microsoft.com/en-us/dotnet/standard/base-types/parsing-numeric))
* The number parser also looks for '0x' prefixed strings and parses those as hexdecimal numbers.
* Parsing of binary numbers prefixed by '0b' is also supported.

### Progress bar
The progress bar is a modified version of [this gist](https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54) by [Daniel Wolf](https://github.com/DanielSWolf) - [MIT License](http://opensource.org/licenses/MIT)
