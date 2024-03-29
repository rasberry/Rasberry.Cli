using System;
using System.IO;
using System.Text;
using System.Threading;

// This is a slightly modified version of ProgressBar.cs by DanielSWolf
// https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54

namespace Rasberry.Cli;

/// <summary>
/// An ASCII progress bar
/// </summary>
public class ProgressBar : IDisposable, IProgress<double>
{
	/// <summary>Dispose method</summary>
	public void Dispose()
	{
		disposed = true;
		UpdateText(string.Empty);
		timer.Dispose();
		GC.SuppressFinalize(this); //https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1816
	}

	/// <summary>Default Constructor</summary>
	public ProgressBar()
	{
		timer = new Timer(TimerHandler);
		writer = Console.Out;

		// a progress bar is only for temporary display in a console window
		// if the console output is redirected to a file, draw nothing
		// otherwise, we'll end up with a lot of garbage in the target file
		if (!Console.IsOutputRedirected) {
			ResetTimer();
		}
	}

	///<summary>Sets the progess amount</summary>
	///<param name="value">The progress amount. This value should be between 0 and 1</param>
	public void Report(double value)
	{
		Interlocked.Exchange(ref currentProgress, value);
	}

	///<summary>Include a prefix string in front of the progress bar</summary>
	public string Prefix { get; set; } = null;

	///<summary>Provide a custom <c>TextWriter</c> instead of using Console.Out</summary>
	public TextWriter Writer {
		get {
			return writer;
		}
		set {
			writer = value;
			ResetTimer(); //assume that we want the output now
		}
	}

	void TimerHandler(object state)
	{
		if (disposed) { return; }

		// Make sure value is in [0..1] range
		double value = Math.Clamp(currentProgress,0,1);

		int progressBlockCount = (int) (value * blockCount);
		int percent = (int) (value * 100);
		string filled = new('#', progressBlockCount);
		string spacer = new('-', blockCount - progressBlockCount);
		char signal = animation[animationIndex++ % animation.Length];

		string text = $"{Prefix}[{filled}{spacer}] {percent,3}% {signal}";
		UpdateText(text);
		ResetTimer();
	}

	void UpdateText(string text)
	{
		// Get length of common portion
		int commonPrefixLength = 0;
		int commonLength = Math.Min(currentText.Length, text.Length);
		while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength]) {
			commonPrefixLength++;
		}

		// Backtrack to the first differing character
		StringBuilder outputBuilder = new();
		outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

		// Output new suffix
		outputBuilder.Append(text.AsSpan(commonPrefixLength));

		// If the new text is shorter than the old one: delete overlapping characters
		int overlapCount = currentText.Length - text.Length;
		if (overlapCount > 0) {
			outputBuilder.Append(' ', overlapCount);
			outputBuilder.Append('\b', overlapCount);
		}

		Writer.Write(outputBuilder);
		currentText = text;
	}

	void ResetTimer()
	{
		if (disposed) { return; }
		var dontRepeat = TimeSpan.FromMilliseconds(-1);
		timer.Change(animationInterval, dontRepeat);
	}

	const int blockCount = 10;
	const string animation = @"|/-\";
	readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
	readonly Timer timer;
	double currentProgress = 0;
	string currentText = string.Empty;
	bool disposed = false;
	int animationIndex = 0;
	TextWriter writer = null;
}