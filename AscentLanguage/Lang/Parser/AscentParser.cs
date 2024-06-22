using AscentLanguage.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage.Parser
{
	public class AscentParser
	{
		private readonly Token[] _tokens;
		private int _position;

		// Define operator precedence levels
		private static readonly Dictionary<TokenType, int> Precedence = new Dictionary<TokenType, int>
		{
			{ TokenType.Pow, 9 },
			{ TokenType.TernaryConditional, 5 },
			{ TokenType.GreaterThan, 6 },
			{ TokenType.LesserThen, 6 },
			{ TokenType.Modulus, 3 },
			{ TokenType.Multiplication, 3 },
			{ TokenType.Division, 3 },
			{ TokenType.Addition, 2 },
			{ TokenType.Subtraction, 2 },
		};

		public AscentParser(Token[] tokens)
		{
			_tokens = tokens;
			_position = 0;
		}

		public Expression ParseExpression()
		{
			return ParseBinary(Precedence[TokenType.Addition]);
		}

		private Expression ParseBinary(int precedence)
		{
			var left = ParsePrimary();

			while (_position < _tokens.Length && Precedence.ContainsKey(_tokens[_position].type) && Precedence[_tokens[_position].type] >= precedence)
			{
				var operatorToken = _tokens[_position++];

				if (operatorToken.type == TokenType.TernaryConditional)
				{
					// Ternary operator found, parse ternary expression
					var trueBranch = ParseExpression();

					if (!CurrentTokenIs(TokenType.Colon))
					{
						throw new Exception("Expected ':' in ternary expression");
					}
					_position++; // consume ':'

					var falseBranch = ParseExpression();

					left = new TernaryExpression(left, trueBranch, falseBranch);
				}
				else
				{
					// Normal binary operation
					var right = ParseBinary(Precedence[operatorToken.type] + 1);
					left = new BinaryExpression(left, operatorToken, right);
				}
			}

			return left;
		}

		private Expression ParsePrimary()
		{
			if (CurrentTokenIs(TokenType.Constant) || CurrentTokenIs(TokenType.Query))
			{
				var numberToken = _tokens[_position++];
				return new NumberExpression(numberToken);
			}

			if (CurrentTokenIs(TokenType.LeftParenthesis))
			{
				_position++; // consume '('
				var expression = ParseExpression();
				if (!CurrentTokenIs(TokenType.RightParenthesis))
				{
					throw new FormatException("Missing closing parenthesis");
				}
				_position++; // consume ')'
				return expression;
			}

			if (CurrentTokenIs(TokenType.LeftBracket))
			{
				_position++; // consume '['
				var expression = ParseExpression();
				if (!CurrentTokenIs(TokenType.RightBracket))
				{
					throw new FormatException("Missing closing Bracket");
				}
				_position++; // consume ']'
				return expression;
			}

			if (CurrentTokenIs(TokenType.Function))
			{
				var functionToken = _tokens[_position++]; // Get the function token

				if (!CurrentTokenIs(TokenType.LeftParenthesis))
				{
					throw new FormatException("Expected '(' after function");
				}
				_position++; // consume '('

				// Parse function arguments
				var arguments = ParseFunctionArguments();

				if (!CurrentTokenIs(TokenType.RightParenthesis))
				{
					throw new FormatException("Missing closing parenthesis for function call");
				}
				_position++; // consume ')'

				return new FunctionExpression(functionToken, arguments);
			}

			if (NextTokenIs(TokenType.TernaryConditional))
			{

				var condition = ParseExpression();

				if (CurrentTokenIs(TokenType.TernaryConditional))
				{
					_position++; // consume '?'
					var trueExpression = ParseExpression();

					if (!CurrentTokenIs(TokenType.Colon))
					{
						throw new FormatException("Expected ':' in ternary expression");
					}
					_position++; // consume ':'

					var falseExpression = ParseExpression();

					return new TernaryExpression(condition, trueExpression, falseExpression);
				}
			}

			if(CurrentTokenIs(TokenType.Definition) || CurrentTokenIs(TokenType.Assignment))
			{
				var definitionToken = _tokens[_position];
				_position++;
				var assignment = ParseExpression();
				return new AssignmentExpression(definitionToken, assignment);
			}

			if (CurrentTokenIs(TokenType.Variable))
			{
				var variableToken = _tokens[_position];
				return new VariableExpression(variableToken);
			}

			throw new FormatException("Unexpected token");
		}

		private Expression[] ParseFunctionArguments()
		{
			var arguments = new List<Expression>();

			// Parse comma-separated list of arguments
			while (_position < _tokens.Length && !CurrentTokenIs(TokenType.RightParenthesis))
			{
				var argument = ParseExpression();
				arguments.Add(argument);

				if (CurrentTokenIs(TokenType.Comma))
				{
					_position++; // consume ','
				}
			}

			return arguments.ToArray();
		}

		private bool CurrentTokenIs(TokenType type)
		{
			return _position < _tokens.Length && _tokens[_position].type == type;
		}

		private bool NextTokenIs(TokenType type)
		{
			return _position + 1 < _tokens.Length && _tokens[_position+1].type == type;
		}
	}
}
