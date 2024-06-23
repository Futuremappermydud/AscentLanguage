namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = "function test(hello) { let x = 5 + hello; x; } test(4)";

			string rewriteExpression = "lerp(5, 5 + 5, 0.5)";
			try
			{
				float result = AscentEvaluator.Evaluate(rewriteExpression, null, true, true);
				Console.WriteLine($"Result: {result}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
