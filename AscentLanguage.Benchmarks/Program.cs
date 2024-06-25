using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace AscentLanguage.Benchmarks
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		}
	}
}
