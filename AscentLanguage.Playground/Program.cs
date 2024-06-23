namespace AscentLanguage.Playground
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = @"
function test(add1, add2) {
    return add1 + add2
};

function k(a) {
    return a * 2
};

return k(test(test(2, 1), 2)) ^ 3 / 5";

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
