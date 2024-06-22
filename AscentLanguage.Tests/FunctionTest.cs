namespace AscentLanguage.Tests
{
	[TestClass]
	public class FunctionTest
	{
		[TestMethod]
		public void BezierCurve()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("bez_curve_x(0, 0, 1, 1, 0.5)");
			Assert.AreEqual(0.5f, result, float.Epsilon);

			var resultY = AscentLanguage.AscentEvaluator.Evaluate("bez_curve_y(0, 1, 1, 0, 0.5)");
			Assert.AreEqual(0.75f, resultY, float.Epsilon);
		}

		[TestMethod]
		public void Lerp()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("lerp(15, 20, 0.5)");
			Assert.AreEqual(17.5f, result, float.Epsilon);
		}

		[TestMethod]
		public void Frac()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("frac(15.654)");
			Assert.AreEqual(0.654f, result, 0.0000005);
		}

		[TestMethod]
		public void Abs()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("abs(-5)");
			Assert.AreEqual(5, result, 0.0000005);
		}

		[TestMethod]
		public void Exp()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("exp(5, 2)");
			Assert.AreEqual(25, result, 0.0000005);
		}

		[TestMethod]
		public void Sqrt()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("sqrt(25)");
			Assert.AreEqual(5, result, 0.0000005);
		}

		[TestMethod]
		public void Sign()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("sign(-5)");
			Assert.AreEqual(-1, result, 0.0000005);
		}

		[TestMethod]
		public void Int()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("int(5.31323481)");
			Assert.AreEqual(5, result, 0.0000005);
		}

		[TestMethod]
		public void Clamp()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("clamp(15, 2, 5)");
			Assert.AreEqual(5, result, 0.0000005);
		}

		[TestMethod]
		public void Sin()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("sin(1.5708)");
			Assert.AreEqual(1, result, 0.0000005);
		}

		[TestMethod]
		public void Cos()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("cos(1.5708)");
			Assert.AreEqual(0, result, 0.0000005);
		}

		[TestMethod]
		public void Tan()
		{
			var result = AscentLanguage.AscentEvaluator.Evaluate("tan(1)");
			Assert.AreEqual(1.5708, result, 0.0000005);
		}
	}
}