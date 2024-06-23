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
		private static readonly List<Tokenizer> tokenizers =
		[
			new QueryTokenizer(),
			new SingleCharTokenizer('+', TokenType.Addition, true),
			new SubtractionTokenizer(),
			new SingleCharTokenizer('*', TokenType.Multiplication, true),
			new SingleCharTokenizer('/', TokenType.Division, true),
			new SingleCharTokenizer('^', TokenType.Pow, true),
			new SingleCharTokenizer('%', TokenType.Pow, true),
			new SingleCharTokenizer('(', TokenType.LeftParenthesis, true),
			new SingleCharTokenizer(')', TokenType.RightParenthesis, false),
			new SingleCharTokenizer('[', TokenType.LeftBracket, true),
			new SingleCharTokenizer(']', TokenType.RightBracket, false),
			new SingleCharTokenizer('<', TokenType.LesserThen, true),
			new SingleCharTokenizer('>', TokenType.GreaterThan, true),
			new SingleCharTokenizer('?', TokenType.TernaryConditional, true),
			new SingleCharTokenizer(':', TokenType.Colon, true),
			new SingleCharTokenizer(';', TokenType.SemiColon, false),
			new SingleCharTokenizer(',', TokenType.Comma, false),
			new FunctionTokenizer(),
			new DefinitionTokenizer(),
			new AssignmentTokenizer(),
			new VariableTokenizer(),
			new NumberTokenizer()
		];

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
				int peek = br.PeekChar(); //Store peek char for efficiency
				bool succeeded = false;
				foreach (var tokenizer in tokenizers)
				{
					long position = stream.Position;
					if (tokenizer.IsMatch(peek, br, stream, ref variableDefinitions))
					{
						stream.Position = position;
						tokens.Add(tokenizer.GetToken(peek, br, stream, ref variableDefinitions));
						succeeded = true;
						break;
					}
					stream.Position = position;
				}
				if (!succeeded)
					br.ReadChar(); // Prevent stack overflow
			}

			return tokens.ToArray();
		}
	}
}
