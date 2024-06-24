using System.Diagnostics;

namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = @"
let a = 5;
function hello(c) {
	function hello2(f1, f2) {
		return f1 + f2;
	}
	let b = hello2(3, 2);
	return c + 10 + b;
}
hello(a);
";
			try
			{

				Console.WriteLine(AscentEvaluator.Evaluate(functionExpression, null, true, true));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
