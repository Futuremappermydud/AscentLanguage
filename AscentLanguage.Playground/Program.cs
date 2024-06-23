using System.Diagnostics;

namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = @"
function test(a, depth) {
    return depth < 10 ? test(a * 2, depth + 1) : a * 2
};

return test(1, 0)";
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
