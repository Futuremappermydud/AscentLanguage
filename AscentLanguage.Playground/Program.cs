namespace AscentLanguage.Playground
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string functionExpression = "function test(hello) { let x = 5 + hello; x; } test(4)";
			float result = AscentEvaluator.Evaluate(functionExpression, null, true, true);
		}
	}
}
