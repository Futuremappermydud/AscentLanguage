namespace AscentLanguage.Tests
{
	[TestClass]
	public class FunctionTests
	{
		[TestMethod]
		public void BezierCurve()
		{
			var result = AscentEvaluator.Evaluate("bez_curve_x(0, 0, 1, 1, 0.5)");
			Assert.AreEqual(0.5f, result, 0.000001);

			var resultY = AscentEvaluator.Evaluate("bez_curve_y(0, 1, 1, 0, 0.5)");
			Assert.AreEqual(0.75f, resultY, 0.000001);
		}

		[TestMethod]
		public void Lerp()
		{
			var result = AscentEvaluator.Evaluate("lerp(15, 20, 0.5)");
			Assert.AreEqual(17.5f, result, 0.000001);
		}

		[TestMethod]
		public void Frac()
		{
			var result = AscentEvaluator.Evaluate("frac(15.654)");
			Assert.AreEqual(0.654f, result, 0.000001);
		}

		[TestMethod]
		public void Abs()
		{
			var result = AscentEvaluator.Evaluate("abs(-5)");
			Assert.AreEqual(5, result, 0.000001);
		}

		[TestMethod]
		public void Exp()
		{
			var result = AscentEvaluator.Evaluate("exp(5)");
			Assert.AreEqual(Math.Exp(5), result, 0.00001);
		}

		[TestMethod]
		public void Sqrt()
		{
			var result = AscentEvaluator.Evaluate("sqrt(25)");
			Assert.AreEqual(5, result, 0.000001);
		}

		[TestMethod]
		public void Sign()
		{
			var result = AscentEvaluator.Evaluate("sign(-5)");
			Assert.AreEqual(-1, result, 0.000001);
		}

		[TestMethod]
		public void Int()
		{
			var result = AscentEvaluator.Evaluate("int(5.31323481)");
			Assert.AreEqual(5, result, 0.000001);
		}

		[TestMethod]
		public void Clamp()
		{
			var result = AscentEvaluator.Evaluate("clamp(15, 2, 5)");
			Assert.AreEqual(5, result, 0.000001);
		}

		[TestMethod]
		public void Sin()
		{
			var result = AscentEvaluator.Evaluate("sin(1.5708)");
			Assert.AreEqual(1, result, 0.000001);
		}

		[TestMethod]
		public void Cos()
		{
			var result = AscentEvaluator.Evaluate("cos(6.28)");
			Assert.AreEqual(Math.Cos(6.28), result, 0.000001);
		}

		[TestMethod]
		public void Tan()
		{
			var result = AscentEvaluator.Evaluate("tan(0.785)");
			Assert.AreEqual(Math.Tan(0.785), result, 0.000001);
		}
	}
}