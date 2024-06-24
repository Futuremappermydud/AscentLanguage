using System.Diagnostics;

namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = @"
let g = 1;
while(g < 1024) {
	g = g * 2;
	debug(g);
}
return g;
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
