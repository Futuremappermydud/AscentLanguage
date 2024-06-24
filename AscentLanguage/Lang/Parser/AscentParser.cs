using AscentLanguage.Tokenizer;
using AscentLanguage.Util;
using AscentLanguage.Splitter;
using System;
using System.Collections.Generic;

namespace AscentLanguage.Parser
{
	public class AscentParser
	{
		private TokenContainer _currentContainer;
		private int _position;
		private List<TokenContainer> _containerStack;
		private Token[] _currentTokens;

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

		public AscentParser(TokenContainer rootContainer)
		{
			_currentContainer = rootContainer;
			_position = 0;
			_containerStack = new List<TokenContainer> { _currentContainer };
			_currentTokens = Array.Empty<Token>();
			LoadNextContainer();
		}

		private void LoadNextContainer()
		{
			while (_position >= _currentTokens.Length && _containerStack.Count > 0)
			{
				var currentContainer = _containerStack[0];
				_containerStack.RemoveAt(0);

				if (currentContainer is SingleTokenContainer single)
				{
					_currentTokens = single.Expression;
					_position = 0;
				}
				else if (currentContainer is MultipleTokenContainer multiple)
				{
					Console.WriteLine($"Loading multiple token container with {multiple.tokenContainers.Count} containers");
					_containerStack.InsertRange(0, multiple.tokenContainers);
				}
			}
			for (int i = 0; i < _currentTokens.Length; i++)
			{
				Console.Write($"{_currentTokens[i].type}, ");
			}
			Console.WriteLine("");
		}

		public List<Expression> Parse(AscentVariableMap variableMap)
		{
			var expressions = new List<Expression>();

			while (_containerStack.Count > 0)
			{
				LoadNextContainer();

				while (_position < _currentTokens.Length)
				{
					var expression = ParseExpression(variableMap);
					if (expression != null)
					{
						expressions.Add(expression);
					}
					_position++;
				}
			}

			return expressions;
		}

		public Expression ParseExpression(AscentVariableMap variableMap)
		{
			return ParseBinary(0, variableMap);
		}

		private Expression ParseBinary(int precedence, AscentVariableMap variableMap)
		{
			var left = ParsePrimary(variableMap);

			while (true)
			{
				if (_position >= _currentTokens.Length || !Precedence.ContainsKey(_currentTokens[_position].type))
				{
					break;
				}

				var operatorToken = _currentTokens[_position];
				var tokenPrecedence = Precedence[operatorToken.type];
				if (tokenPrecedence < precedence)
				{
					break;
				}

				_position++;
				var right = ParseBinary(tokenPrecedence + 1, variableMap);
				left = new BinaryExpression(left, operatorToken, right);
			}

			return left;
		}

		private Expression ParsePrimary(AscentVariableMap variableMap)
		{
			if (CurrentTokenIs(TokenType.Constant) || CurrentTokenIs(TokenType.Query))
			{
				var numberToken = _currentTokens[_position++];
				return new NumberExpression(numberToken);
			}

			if (CurrentTokenIs(TokenType.LeftParenthesis))
			{
				_position++; // consume '('
				var expression = ParseExpression(variableMap);
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
				var expression = ParseExpression(variableMap);
				if (!CurrentTokenIs(TokenType.RightBracket))
				{
					throw new FormatException("Missing closing Bracket");
				}
				_position++; // consume ']'
				return expression;
			}

			if (CurrentTokenIs(TokenType.Function))
			{
				var functionToken = _currentTokens[_position++]; // Get the function token

				if (!CurrentTokenIs(TokenType.LeftParenthesis))
				{
					throw new FormatException("Expected '(' after function");
				}
				_position++; // consume '('

				// Parse function arguments
				var arguments = ParseFunctionArguments(false, variableMap);

				if (!CurrentTokenIs(TokenType.RightParenthesis))
				{
					throw new FormatException("Missing closing parenthesis for function call");
				}
				_position++; // consume ')'

				return new FunctionExpression(functionToken, arguments);
			}

			if (CurrentTokenIs(TokenType.FunctionDefinition))
			{
				var functionToken = _currentTokens[_position++]; // Get the function token

				if (CurrentTokenIs(TokenType.LeftParenthesis))
				{
					_position++; // consume '('
					var arguments = ParseDefinitionArguments();
					var name = functionToken.tokenBuffer;
					var definition = new FunctionDefinition(name);
					variableMap.Functions.Add(name, definition);
					definition.args = arguments.ToList();
					_position++; // consume ')'
				}

				if (!CurrentTokenIs(TokenType.LeftScope))
				{
					throw new FormatException("Expected '{' after function");
				}
				_position++; // consume '{'

				// Parse function contents
				var contents = ParseFunctionArguments(true, variableMap);

				if (!CurrentTokenIs(TokenType.RightScope))
				{
					throw new FormatException("Missing closing scope for function call");
				}
				_position++; // consume '}'

				return new FunctionDefinitionExpression(functionToken, contents);
			}

			if (CurrentTokenIs(TokenType.ForLoop))
			{
				_position++; // consume 'for'
				Expression definition;
				Expression condition;
				Expression suffix;
				if (CurrentTokenIs(TokenType.LeftParenthesis))
				{
					_position++; // consume '('
					definition = ParseExpression(variableMap);
					if (CurrentTokenIs(TokenType.SemiColon))
					{
						_position++; // consume ';'
					}
					condition = ParseExpression(variableMap);
					if (CurrentTokenIs(TokenType.SemiColon))
					{
						_position++; // consume ';'
					}
					suffix = ParseExpression(variableMap);
					_position++; // consume ')'
				}
				else
				{
					throw new FormatException("Expected '(' after for loop. Missing definition, condition, and suffix!");
				}

				if (!CurrentTokenIs(TokenType.LeftScope))
				{
					throw new FormatException("Expected '{' after for loop");
				}
				_position++; // consume '{'

				// Parse function contents
				var contents = ParseFunctionArguments(true, variableMap);

				if (!CurrentTokenIs(TokenType.RightScope))
				{
					throw new FormatException("Missing closing scope for loop");
				}
				_position++; // consume '}'

				return new ForLoopExpression(definition, condition, suffix, contents);
			}

			if (CurrentTokenIs(TokenType.WhileLoop))
			{
				_position++; // consume 'while'
				Expression condition;
				if (CurrentTokenIs(TokenType.LeftParenthesis))
				{
					_position++; // consume '('
					condition = ParseExpression(variableMap);
					_position++; // consume ')'
				}
				else
				{
					throw new FormatException("Expected '(' after while loop. Missing condition!");
				}

				if (!CurrentTokenIs(TokenType.LeftScope))
				{
					throw new FormatException("Expected '{' after while loop");
				}
				_position++; // consume '{'

				// Parse function contents
				var contents = ParseFunctionArguments(true, variableMap);

				if (!CurrentTokenIs(TokenType.RightScope))
				{
					throw new FormatException("Missing closing scope for loop");
				}
				_position++; // consume '}'

				return new WhileLoopExpression(condition, contents);
			}

			if (NextTokenIs(TokenType.TernaryConditional))
			{

				var condition = ParseExpression(variableMap);

				if (CurrentTokenIs(TokenType.TernaryConditional))
				{
					_position++; // consume '?'
					var trueExpression = ParseExpression(variableMap);

					if (!CurrentTokenIs(TokenType.Colon))
					{
						throw new FormatException("Expected ':' in ternary expression");
					}
					_position++; // consume ':'

					var falseExpression = ParseExpression(variableMap);

					return new TernaryExpression(condition, trueExpression, falseExpression);
				}
			}

			if (CurrentTokenIs(TokenType.Definition) || CurrentTokenIs(TokenType.Assignment))
			{
				var definitionToken = _currentTokens[_position];
				_position++;
				var assignment = ParseExpression(variableMap);
				return new AssignmentExpression(definitionToken, assignment);
			}

			if (CurrentTokenIs(TokenType.Variable))
			{
				if(NextTokenIs(TokenType.Increment))
				{
					var variableToken = _currentTokens[_position];
					_position++;
					_position++;
					return new IncrementVariableExpression(variableToken);
				}
				else
				{
					if (NextTokenIs(TokenType.Decrement))
					{
						var variableToken = _currentTokens[_position];
						_position++;
						_position++;
						return new DecrementVariableExpression(variableToken);
					}
					else
					{
						var variableToken = _currentTokens[_position];
						_position++;
						return new VariableExpression(variableToken);
					}
				}
			}

			if (CurrentTokenIs(TokenType.FunctionArgument))
			{
				var token = _currentTokens[_position];
				_position++;
				return new NilExpression(token);
			}

			if (CurrentTokenIs(TokenType.Return))
			{
				_position++;
				var ret = ParseExpression(variableMap);
				return new ReturnExpression(ret);
			}

			throw new FormatException($"Unexpected token {_currentTokens[_position].type}");
		}

		private string[] ParseDefinitionArguments()
		{
			var arguments = new List<string>();

			int checks = 0;

			// Parse comma-separated list of arguments
			while (_position < _currentTokens.Length && !CurrentTokenIs(TokenType.RightParenthesis) && checks < 30)
			{
				checks++;
				var argument = _currentTokens[_position++];
				arguments.Add(argument.tokenBuffer);

				if (CurrentTokenIs(TokenType.Comma))
				{
					_position++; // consume ','
				}
			}

			return arguments.ToArray();
		}

		private Expression[] ParseFunctionArguments(bool scoped, AscentVariableMap variableMap)
		{
			var arguments = new List<Expression>();

			int checks = 0;

			// Parse comma-separated list of arguments
			while (!CurrentTokenIs(scoped ? TokenType.RightScope : TokenType.RightParenthesis) && checks < 30)
			{
				LoadNextContainer();
				if (CurrentTokenIs(scoped ? TokenType.RightScope : TokenType.RightParenthesis)) break;
				checks++;
				var argument = ParseExpression(variableMap);
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
			return _position < _currentTokens.Length && _currentTokens[_position].type == type;
		}

		private bool NextTokenIs(TokenType type)
		{
			return _position + 1 < _currentTokens.Length && _currentTokens[_position + 1].type == type;
		}
	}
}