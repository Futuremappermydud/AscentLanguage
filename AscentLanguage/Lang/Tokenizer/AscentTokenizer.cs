using AscentLanguage.Functions;
using AscentLanguage.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AscentLanguage.Tokenizer
{
	public static class AscentTokenizer
	{
		public static bool ContinueFeedingQuery(int chara)
		{
			return (chara >= 'a' && chara <= 'z') || (chara >= 'A' && chara <= 'Z');
		}
		public static bool IsNum(int c)
		{
			return c >= '0' && c <= '9' || c == '.' || c == '-';
		}

		public static Token[] Tokenize(string expression)
		{
			List<string> variableDefinitions = new List<string>();
			List<Token> tokens = new List<Token>();

			string trimmedExpression = expression.Replace(" ", "");

			int strLength = trimmedExpression.Length;
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(trimmedExpression));
			BinaryReader br = new BinaryReader(stream, Encoding.UTF8);
			while (stream.Position < strLength)
			{
				int peek = br.PeekChar();
				//Check if query
				if (peek == 'q' )
				{
					char[] buffer = new char[5];
					int check = 0;
					while(br.PeekChar() != '.' && check != 5) {
						buffer[check] = br.ReadChar();
						check++;
					}

					if ((check == 1 && buffer[0] == 'q') || String.Equals("query", new string(buffer), StringComparison.OrdinalIgnoreCase))
					{
						string queryNameBuffer = "";
						br.ReadChar(); // Skip the period
						while (ContinueFeedingQuery(br.PeekChar()) && stream.Position < strLength)
						{
							queryNameBuffer += br.ReadChar();
						}

						tokens.Add(new Token(TokenType.Query, queryNameBuffer.ToCharArray()));
						continue;
					}
					else
					{
						stream.Position -= check;
					}
				}
				if (peek == '+')
				{
					tokens.Add(new Token(TokenType.Addition, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '-')
				{
					if (tokens.Last().type == TokenType.Constant && tokens.Last().type == TokenType.Variable && tokens.Last().type == TokenType.Query)
					{
						tokens.Add(new Token(TokenType.Subtraction, br.ReadChar()));
						if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
						continue;
					}
				}
				if (peek == '*')
				{
					tokens.Add(new Token(TokenType.Multiplication, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '/')
				{
					tokens.Add(new Token(TokenType.Division, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '%')
				{
					tokens.Add(new Token(TokenType.Modulus, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '(')
				{
					tokens.Add(new Token(TokenType.LeftParenthesis, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == ')')
				{
					tokens.Add(new Token(TokenType.RightParenthesis, br.ReadChar()));
					continue;
				}
				if (peek == '[')
				{
					tokens.Add(new Token(TokenType.LeftBracket, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == ']')
				{
					tokens.Add(new Token(TokenType.RightBracket, br.ReadChar()));
					continue;
				}
				if (peek == '^')
				{
					tokens.Add(new Token(TokenType.Pow, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '%')
				{
					tokens.Add(new Token(TokenType.Modulus, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '>')
				{
					tokens.Add(new Token(TokenType.GreaterThan, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '<')
				{
					tokens.Add(new Token(TokenType.LesserThen, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == '?')
				{
					tokens.Add(new Token(TokenType.TernaryConditional, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == ':')
				{
					tokens.Add(new Token(TokenType.Colon, br.ReadChar()));
					if (stream.Position >= strLength) throw new FormatException("Missing Operand!");
					continue;
				}
				if (peek == ',')
				{
					tokens.Add(new Token(TokenType.Comma, br.ReadChar()));
					continue;
				}
				if (peek == ';')
				{
					tokens.Add(new Token(TokenType.SemiColon, br.ReadChar()));
					continue;
				}
				if (peek == 'l')
				{
					char[] numStack = new char[3];
					int check = 0;

					for (int i = 0; i < 3; i++)
					{
						numStack[check] = br.ReadChar();
						check++;
					}

					if(String.Equals("let", new string(numStack), StringComparison.OrdinalIgnoreCase))
					{
						string varStack = "";

						while (br.PeekChar() != '=' && stream.Position < strLength)
						{
							varStack += br.ReadChar();
						}
						if(variableDefinitions.Contains(varStack))
						{
							throw new FormatException($"Variable {varStack} already Exists! char {stream.Position}");
						}
						variableDefinitions.Add(varStack);
						tokens.Add(new Token(TokenType.Definition, varStack.ToCharArray()));
						continue;
					}
					else
					{
						stream.Position -= check;
					}
				}
				if (IsNum(peek))
				{
					string numStack = "";

					while (IsNum(br.PeekChar()))
					{
						numStack += br.ReadChar();
					}

					tokens.Add(new Token(TokenType.Constant, numStack.ToCharArray()));
					continue;
				}
                //Check Functions
                if (AscentFunctions.SearchAnyFunctions((char)peek))
                {
					string funcStack = "";
					int check = 0;

					while (br.PeekChar() != '(' && stream.Position < strLength)
					{
						funcStack += br.ReadChar();
						check++;
					}
					if (stream.Position >= strLength)
					{
						stream.Position -= check;
					}
					else
					{
						tokens.Add(new Token(TokenType.Function, funcStack.ToCharArray()));
						continue;
					}
				}
				if (Utility.SearchForPotential((char)peek, variableDefinitions))
				{
					string varStack = "";
					int check = 0;

					while (!variableDefinitions.Contains(varStack) && check < 25)
					{
						varStack += br.ReadChar();
						check++;
					}

					if(br.PeekChar() == '=')
					{
						tokens.Add(new Token(TokenType.Assignment, varStack.ToCharArray()));
						br.ReadChar();
						continue;
					}
					else
					{
						stream.Position -= check;
					}
				}
				if (Utility.SearchForPotential((char)peek, variableDefinitions))
				{
					string varStack = "";
					int check = 0;

					while (!variableDefinitions.Contains(varStack) && check < 25)
					{
						varStack += br.ReadChar();
						check++;
					}

					if(check == 24)
					{
						stream.Position -= check;
					}

					tokens.Add(new Token(TokenType.Variable, varStack.ToCharArray()));
					continue;
				}
				br.ReadChar();
			}

			return tokens.ToArray();
		}
	}
}
