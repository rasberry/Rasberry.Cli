using System;

namespace Rasberry.Cli;

/// <summary>
/// Defines a provider for progress updates along with a label
/// </summary>
/// <typeparam name="T">The type of progress update value.</typeparam>
interface IProgressWithLabel<T> : IProgress<T>
{
	///<summary>Include a label for use with the progress bar</summary>
	string Label { get; set; }
}