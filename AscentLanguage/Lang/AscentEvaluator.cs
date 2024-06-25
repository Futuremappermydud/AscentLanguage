using AscentLanguage.Parser;
using AscentLanguage.Splitter;
using AscentLanguage.Tokenizer;
using AscentLanguage.Util;

namespace AscentLanguage
{
	public static class AscentEvaluator
	{
		private class CacheData
		{
			internal Expression[] expressions;
			internal Dictionary<string, FunctionDefinition> functions;

			public CacheData(Expression[] expressions, Dictionary<string, FunctionDefinition> functions)
			{
				this.expressions = expressions;
				this.functions = functions;
			}
		}
		private static Dictionary<string, CacheData> cachedExpressions = new Dictionary<string, CacheData>();
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
				toEvaluate = cachedExpressions[expression].expressions.ToList();
				variableMap.Functions = cachedExpressions[expression].functions;
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

				var containers = AscentSplitter.SplitTokens(tokens.ToList());
				if (debug)
				{
					Utility.PrintTokenContainer(containers);
					Console.WriteLine("\n");
				}

				var parser = new AscentParser(containers as MultipleTokenContainer);

				var parsedExpressions = parser.Parse(variableMap);

				if (debug)
				{
					Console.WriteLine($"Parsed {parsedExpressions.Count} expressions");
				}

				for (int i = 0; i < parsedExpressions.Count; i++)
				{
					if (debug)
					{
						Utility.PrintExpression(parsedExpressions[i]);
					}

					toEvaluate.Add(parsedExpressions[i]);
				}
				if (cache)
					cachedExpressions[expression] = new CacheData(toEvaluate.ToArray(), variableMap.Functions);
			}
			float result = 0f;
			foreach (var evaluate in toEvaluate)
			{
				var eval = evaluate.Evaluate(variableMap);
				if (eval.HasValue) result = eval.Value;
			}
			return result;
		}
	}
}
