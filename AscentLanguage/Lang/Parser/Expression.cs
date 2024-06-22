using AscentLanguage.Functions;
using AscentLanguage.Tokenizer;
using AscentLanguage.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage.Parser
{
	public abstract class Expression
	{
		public abstract float? Evaluate(AscentVariableMap? ascentVariableMap);
	}

	public class BinaryExpression : Expression
	{
		public Expression Left { get; set; }
		public Token Operator { get; set; }
		public Expression Right { get; set; }

		public BinaryExpression(Expression left, Token op, Expression right)
		{
			Left = left;
			Operator = op;
			Right = right;
		}
		public override float? Evaluate(AscentVariableMap? ascentVariableMap)
		{
			if(Operator.type == TokenType.Multiplication)
			{
				return Left.Evaluate(ascentVariableMap) * Right.Evaluate(ascentVariableMap);
			}

			if (Operator.type == TokenType.Division)
			{
				return Left.Evaluate(ascentVariableMap) / Right.Evaluate(ascentVariableMap);
			}

			if (Operator.type == TokenType.Addition)
			{
				return Left.Evaluate(ascentVariableMap) + Right.Evaluate(ascentVariableMap);
			}

			if (Operator.type == TokenType.Subtraction)
			{
				return Left.Evaluate(ascentVariableMap) - Right.Evaluate(ascentVariableMap);
			}

			if (Operator.type == TokenType.Pow)
			{
				return (float)Math.Pow(Left.Evaluate(ascentVariableMap) ?? 0f, Right.Evaluate(ascentVariableMap) ?? 0f);
			}

			if (Operator.type == TokenType.Modulus)
			{
				return Left.Evaluate(ascentVariableMap) % Right.Evaluate(ascentVariableMap);
			}

			if (Operator.type == TokenType.GreaterThan)
			{
				return (Left.Evaluate(ascentVariableMap) > Right.Evaluate(ascentVariableMap)) ? 1f : 0f;
			}

			if (Operator.type == TokenType.LesserThen)
			{
				return (Left.Evaluate(ascentVariableMap) < Right.Evaluate(ascentVariableMap)) ? 1f : 0f;
			}

			throw new InvalidOperationException($"Unsupported operator: {new string(Operator.tokenBuffer)}");
		}
	}

	public class NumberExpression : Expression
	{
		public Token Token { get; set; }

		public NumberExpression(Token token)
		{
			Token = token;
		}
		public override float? Evaluate(AscentVariableMap? ascentVariableMap)
		{
			if (Token.type == TokenType.Constant)
			{
				return Utility.ConvertToFloat(Token.tokenBuffer);
			}
			else if (Token.type == TokenType.Query)
			{
				var buffer = new string(Token.tokenBuffer, 0, Utility.FindLengthToUse(Token.tokenBuffer));
				if (ascentVariableMap != null && ascentVariableMap.QueryVariables.TryGetValue(buffer, out float value))
				{
					return value;
				}
				else
				{
					Console.WriteLine($"Variable {buffer} ({buffer.Length}) not found in variable map");
				}
			}

			return 0.0f;
		}
	}

	public class TernaryExpression : Expression
	{
		public Expression Condition { get; set; }
		public Expression TrueExpression { get; set; }
		public Expression FalseExpression { get; set; }

		public TernaryExpression(Expression condition, Expression trueExpr, Expression falseExpr)
		{
			Condition = condition;
			TrueExpression = trueExpr;
			FalseExpression = falseExpr;
		}

		public override float? Evaluate(AscentVariableMap? ascentVariableMap)
		{
			var conditionValue = Condition.Evaluate(ascentVariableMap);

			// Evaluate based on the condition value
			if (conditionValue == 1f)
			{
				return TrueExpression.Evaluate(ascentVariableMap);
			}
			else if (conditionValue == 0f)
			{
				return FalseExpression.Evaluate(ascentVariableMap);
			}
			else
			{
				throw new InvalidOperationException("Condition in ternary expression must evaluate to 0 or 1");
			}
		}
	}

	public class FunctionExpression : Expression
	{
		public Token FunctionToken { get; }
		public Expression[] Arguments { get; }

		public FunctionExpression(Token functionToken, Expression[] arguments)
		{
			FunctionToken = functionToken;
			Arguments = arguments;
		}

		public override float? Evaluate(AscentVariableMap? ascentVariableMap)
		{
			var function = AscentFunctions.GetFunction(new string(FunctionToken.tokenBuffer, 0, Utility.FindLengthToUse(FunctionToken.tokenBuffer)));
			if (function == null)
			{
				throw new ArgumentException($"Function {new string(FunctionToken.tokenBuffer)} does not exist!");
			}
			return function.Evaluate(Arguments.Select(x => x.Evaluate(ascentVariableMap) ?? 0f).ToArray());
		}
	}

	public class AssignmentExpression : Expression
	{
		public Token VariableToken { get; }
		public Expression Assignment { get; }

		public AssignmentExpression(Token variableToken, Expression assignment)
		{
			VariableToken = variableToken;
			Assignment = assignment;
		}

		public override float? Evaluate(AscentVariableMap? ascentVariableMap)
		{
			if(ascentVariableMap == null)
			{
				throw new InvalidOperationException("Variable map cannot be null");
			}
			if (Assignment == null)
			{
				throw new InvalidOperationException("Assignment Expression cannot be null");
			}
			ascentVariableMap.Variables[new string(VariableToken.tokenBuffer, 0, Utility.FindLengthToUse(VariableToken.tokenBuffer))] = Assignment?.Evaluate(ascentVariableMap) ?? 0f;
			return null;
		}
	}

	public class VariableExpression : Expression
	{
		public Token VariableToken { get; }

		public VariableExpression(Token variableToken)
		{
			VariableToken = variableToken;
		}

		public override float? Evaluate(AscentVariableMap? ascentVariableMap)
		{
			if (ascentVariableMap == null)
			{
				throw new InvalidOperationException("Variable map cannot be null");
			}
			if (ascentVariableMap.Variables.TryGetValue(new string(VariableToken.tokenBuffer, 0, Utility.FindLengthToUse(VariableToken.tokenBuffer)), out float value))
			{
				return value;
			}
			return null;
		}
	}
}
