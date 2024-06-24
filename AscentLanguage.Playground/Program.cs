using System.Diagnostics;

namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = @"
for (let i = 0; i < 10; i++) {
	debug(i);
}
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
