using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AscentLanguage.Benchmarks
{
	[SimpleJob(RuntimeMoniker.HostProcess, baseline: true)]
	[RPlotExporter]
	public class Benchmarks
	{
		private const string nesting = @"
function aFunc(a) 
{
	function bFunc(b) 
	{
		function cFunc(c) 
		{
			function dFunc(d) 
			{
				function eFunc(e) 
				{
					return e + 1;
				}
				return eFunc(d) + 1;
			}
			return dFunc(c) + 1;
		}
		return cFunc(b) + 1;
	}
	return bFunc(a) + 1;
}
aFunc(3);
";
		private const string condtionals = @"
function aFunc(a)
{
	return a > 5;
}
let b = 5;
let c = 0;
for (let i = 0; i < 100; i++)
{
	c = c + aFunc(b);
}
return c;
";
		private const string queries = @"
return query.a + query.b + query.c + query.d + query.e + query.f + query.g + query.h + query.i + query.j + query.k;
";
		private AscentVariableMap variableMap = new AscentVariableMap(new Dictionary<string, float>() {
			{ "a", 10f },
			{ "b", 20f },
			{ "c", 30f },
			{ "d", 40f },
			{ "e", 50f },
			{ "f", 60f },
			{ "g", 70f },
			{ "h", 80f },
			{ "i", 90f },
			{ "j", 100f },
			{ "k", 110f }
		});
		private const string functions = @"
";

		[GlobalSetup]
		public void Setup()
		{
		}

		[Benchmark]
		public float Nesting()
		{
			return AscentEvaluator.Evaluate(nesting, null, true, true);
		}

		[Benchmark]
		public float Conditionals()
		{
			return AscentEvaluator.Evaluate(condtionals, null, true, true);
		}

		[Benchmark]
		public float Queries()
		{
			return AscentEvaluator.Evaluate(queries, variableMap, true, true);
		}

		[Benchmark]
		public float Functions()
		{
			return AscentEvaluator.Evaluate(functions, null, true, true);
		}
	}
}
