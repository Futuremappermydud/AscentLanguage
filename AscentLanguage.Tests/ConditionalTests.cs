namespace AscentLanguage.Tests
{
	[TestClass]
	public class ConditionalTests
	{
		[TestMethod]
		public void LessThanFalse()
		{
			var result = AscentEvaluator.Evaluate("10 < 5");
			Assert.AreEqual(0f, result, float.Epsilon);
		}

		[TestMethod]
		public void LessThanTrue()
		{
			var result = AscentEvaluator.Evaluate("5 < 10");
			Assert.AreEqual(1f, result, float.Epsilon);
		}

		[TestMethod]
		public void GreaterThanFalse()
		{
			var result = AscentEvaluator.Evaluate("5 > 10");
			Assert.AreEqual(0f, result, float.Epsilon);
		}

		[TestMethod]
		public void GreaterThanTrue()
		{
			var result = AscentEvaluator.Evaluate("10 > 5");
			Assert.AreEqual(1f, result, float.Epsilon);
		}

		[TestMethod]
		public void TernaryTrue()
		{
			var result = AscentEvaluator.Evaluate("10 > 5 ? 7 : 8");
			Assert.AreEqual(7f, result, float.Epsilon);
		}

		[TestMethod]
		public void TernaryFalse()
		{
			var result = AscentEvaluator.Evaluate("10 < 5 ? 7 : 8");
			Assert.AreEqual(8f, result, float.Epsilon);
		}
	}
}