using AscentLanguage.Parser;
using AscentLanguage.Tokenizer;
using AscentLanguage.Util;

namespace AscentLanguage
{
	public static class AscentEvaluator
	{
		private static Dictionary<string, Expression[]> cachedExpressions = new Dictionary<string, Expression[]>();
		public static float Evaluate(string expression, AscentVariableMap? variableMap = null, bool cache = true, bool debug = false)
		{
			if(variableMap == null)
			{
				variableMap = new AscentVariableMap(new Dictionary<string, float>());
			}
			variableMap.Variables.Clear();
			variableMap.Functions.Clear();
			List<Expression> toEvaluate = new List<Expression>();
			if (cachedExpressions.ContainsKey(expression) && cache)
			{
				toEvaluate = cachedExpressions[expression].ToList();
			}
			else
			{
				var tokens = AscentTokenizer.Tokenize(expression);

				if (debug)
				{
					for (int i = 0; i < tokens.Length; i++)
					{
						Console.ForegroundColor = (ConsoleColor)((i % 6) + 1);
						Console.WriteLine($"Token {i}: {tokens[i].type} - {new string(tokens[i].tokenBuffer)}");
					}
					Console.ForegroundColor = ConsoleColor.White;
				}

				//TODO: optimize?
				var lines = tokens.Aggregate(new List<List<Token>> { new List<Token>() }, (acc, token) =>
				{
					if (token.type == TokenType.SemiColon)
					{
						acc.Add(new List<Token>());
					}
					else
					{
						acc.Last().Add(token);
					}
					return acc;
				}).Where(x=>x.Count > 0).ToList();

				for (int i = 0; i < lines.Count; i++)
				{
					var parser = new AscentParser(lines[i].ToArray());

					var parsedExpression = parser.ParseExpression();

					if (debug)
					{
						Utility.PrintExpression(parsedExpression);
					}

					toEvaluate.Add(parsedExpression);
				}
				if (cache)
					cachedExpressions[expression] = toEvaluate.ToArray();
			}
			float result = 0f;
			foreach (var evaluate in toEvaluate)
			{
				var eval = evaluate.Evaluate(variableMap);
				if(eval.HasValue) result = eval.Value;
			}
			return result;
		}
	}
}
