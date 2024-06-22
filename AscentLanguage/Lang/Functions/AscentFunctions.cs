using AscentLanguage.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage.Functions
{
	public static class AscentFunctions
	{
		private readonly static Dictionary<string, Function> functions = new Dictionary<string, Function>()
		{
			{ "sin", new SinFunction() },
			{ "cos", new CosFunction() },
			{ "tan", new TanFunction() },
			{ "clamp", new ClampFunction() },
			{ "sign", new SignFunction() },
			{ "sqrt", new SqrtFunction() },
			{ "int", new IntFunction() },
			{ "abs", new AbsFunction() },
			{ "pow", new PowFunction() },
			{ "exp", new ExpFunction() },
			{ "frac", new FracFunction() },
			{ "lerp", new LerpFunction() },
			{ "bez_curve_x", new BezierCurveXFunction() },
			{ "bez_curve_y", new BezierCurveYFunction() },
		};

		public static Function GetFunction(string name)
		{
			if(functions.ContainsKey(name)) return functions[name];
			throw new ArgumentException($"Function {name} does not exist!");
		}
		public static bool SearchAnyFunctions(char c)
		{
			return Utility.SearchForPotential(c, [.. functions.Keys]);
		}
		public abstract class Function
		{
			public abstract float Evaluate(float[] input);
		}

		public class SinFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if(input.Length < 1) return 0f;
				return (float)Math.Sin(input[0]);
			}
		}

		public class CosFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return (float)Math.Cos(input[0]);
			}
		}

		public class TanFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return (float)Math.Tan(input[0]);
			}
		}

		public class ClampFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 3) return 0f;
				return Math.Clamp(input[0], input[1], input[2]);
			}
		}

		public class SignFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return Math.Sign(input[0]);
			}
		}

		public class SqrtFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return (float)Math.Sqrt(input[0]);
			}
		}

		public class IntFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return (int)input[0];
			}
		}

		public class AbsFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return Math.Abs(input[0]);
			}
		}

		public class LerpFunction : Function
		{
			private float Lerp(float a, float b, float t)
			{
				return a + t * (b - a);
			}
			public override float Evaluate(float[] input)
			{
				if (input.Length < 3) return 0f;
				return Lerp(input[0], input[1], input[2]);
			}
		}

		public class PowFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 2) return 0f;
				return (float)Math.Pow(input[0], input[1]);
			}
		}

		public class ExpFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return (float)Math.Exp(input[0]);
			}
		}

		public class FracFunction : Function
		{
			public override float Evaluate(float[] input)
			{
				if (input.Length < 1) return 0f;
				return input[0] - (float)Math.Floor(input[0]);
			}
		}

		public class BezierCurveXFunction : Function
		{
			private static float BezierCurveX(float x0, float x1, float x2, float x3, float t)
			{
				double x = Math.Pow(1 - t, 3) * x0 +
						   3 * Math.Pow(1 - t, 2) * t * x1 +
						   3 * (1 - t) * Math.Pow(t, 2) * x2 +
						   Math.Pow(t, 3) * x3;
				return (float)x;
			}
			public override float Evaluate(float[] input)
			{
				if (input.Length < 5) return 0f;
				return BezierCurveX(input[0], input[1], input[2], input[3], input[4]);
			}
		}

		public class BezierCurveYFunction : Function
		{
			private static float BezierCurveY(float y0, float y1, float y2, float y3, float t)
			{
				double y = Math.Pow(1 - t, 3) * y0 +
						   3 * Math.Pow(1 - t, 2) * t * y1 +
						   3 * (1 - t) * Math.Pow(t, 2) * y2 +
						   Math.Pow(t, 3) * y3;
				return (float)y;
			}
			public override float Evaluate(float[] input)
			{
				if (input.Length < 5) return 0f;
				return BezierCurveY(input[0], input[1], input[2], input[3], input[4]);
			}
		}
	}
}
