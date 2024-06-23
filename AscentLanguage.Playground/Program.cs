using System.Diagnostics;

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

function test2(add1, add2) {
    return add1 + add2
};

function test3(add1, add2) {
    return add1 + add2
};

function test4(add1, add2) {
    return add1 + add2
};

function test5(add1, add2) {
    return add1 + add2
};

return test(test2(test3(test4(test5(2, 2), test5(2, 2)), test4(test5(2, 2), test5(2, 2)))), test2(test3(test4(test5(2, 2), test5(2, 2)), test4(test5(2, 2), test5(2, 2))))) ^ 3 * 2";
			try
			{
				// run expression for 1000 iterations and then average the result.
				var stopWatch = new Stopwatch();
				stopWatch.Start();
				for (int i = 0; i < 1000; i++)
				{
					AscentEvaluator.Evaluate(functionExpression, null, true, false);
				}
				stopWatch.Stop();
				Console.WriteLine($"Average time: {stopWatch.ElapsedMilliseconds / 1000f}ms");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
