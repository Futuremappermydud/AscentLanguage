namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = "function test(a, b) { a + b }; test(3, 2);";

			string rewriteExpression = "lerp(5, 5 + 5, 0.5)";
			try
			{
				float result = AscentEvaluator.Evaluate(functionExpression, null, true, true);
				Console.WriteLine($"Result: {result}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
