# Rasberry.Cli #
A library with some helpful functions for command line programs.

Features include:
* functions for processing command line arguments.
* tools to display a help message.
* A progress bar

## How to use ##
### Printing a help message ###
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

### Parsing inputs ###
```csharp
bool ParseInputs(string[] args)
{
	var p = new ParseParams(args);
	if (p.Has("-a").IsGood()) {
		HasOptionA = true;
	}
	if (p.Default("-b",out OptionB).IsInvalid()) {
		Console.WriteLine("someing is wrong with your -b option");
		return false;
	}
	if (p.Expect("-c",out OptionC).IsBad()) {
		Console.WriteLine("option -c is invalid or missing");
		return false;
	}
	return true; //parsing inputs worked
}

bool HasOptionA;
double OptionB;
double OptionC;
```

### Displaying progress ###
```csharp
Console.Write("Performing some task... ");
using (var progress = new ProgressBar()) {
	for (int i = 0; i <= 100; i++) {
		progress.Report((double) i / 100);
		Thread.Sleep(20);
	}
}
```

## Additional Notes ##
### Parsing numbers ###
* Parsing numbers to number types (int,float,etc..) uses NumberStyles.Any (see [details](https://docs.microsoft.com/en-us/dotnet/standard/base-types/parsing-numeric))
* The parser also looks for '0x' prefixed strings and parses those as hexdecimal numbers.
* Parsing of binary numbers prefixed by '0b' is also supported.

### Progress bar ###
The progress bar is a modified version of [this gist](https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54) by [Daniel Wolf](https://github.com/DanielSWolf) - [MIT License](http://opensource.org/licenses/MIT)

## TODO ##
