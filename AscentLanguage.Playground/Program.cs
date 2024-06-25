using System.Diagnostics;

namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = @"
let c = 5;
c -= 7;
return c;
";
			try
			{
				//Evaluate perfTest1 1000 times and print the average time in ms
				/*Stopwatch sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < 10000; i++)
				{
					AscentEvaluator.Evaluate(perfTest1, null, true, false);
				}
				sw.Stop();
				Console.WriteLine(sw.ElapsedMilliseconds / 10000.0);*/
				Console.WriteLine(AscentEvaluator.Evaluate(functionExpression, null, true, false));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
