using AscentLanguage.Functions;
using AscentLanguage.Tokenizer;
using AscentLanguage.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage.Tokenizer
{
	public abstract class Tokenizer
	{
		public abstract Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs);
		public abstract bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null);
	}

	public class SingleCharTokenizer : Tokenizer
	{
		public char Token { get; set; }
		public TokenType Type { get; set; }
		public bool HasOperand { get; set; }

		public SingleCharTokenizer(char token, TokenType type, bool hasOperand)
		{
			Token = token;
			Type = type;
			HasOperand = hasOperand;
		}

		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			if (stream.Position >= stream.Length && HasOperand) throw new FormatException("Missing Operand!");
			return new Token(Type, br.ReadChar());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			return peekChar == Token;
		}
	}

	public class SubtractionTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			if (stream.Position >= stream.Length) throw new FormatException("Missing Operand!");
			return new Token(TokenType.Subtraction, '-');
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			if(existingTokens != null && existingTokens.Count > 0)
			{
				Token lastToken = existingTokens[existingTokens.Count - 1];
				if (lastToken.type == TokenType.Constant || lastToken.type == TokenType.Variable || lastToken.type == TokenType.Query)
				{
					return peekChar == '-';
				}
			}
			return false;
		}
	}

	public class NumberTokenizer : Tokenizer
	{
		private bool IsNumber(int c)
		{
			return c >= '0' && c <= '9' || c == '.' || c == '-';
		}
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (IsNumber(br.PeekChar()))
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			return new Token(TokenType.Constant, stringBuilder.ToString().ToCharArray());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			return IsNumber(peekChar);
		}
	}

	public class WordMatchTokenizer : Tokenizer
	{
		public string Word { get; set; }
		public TokenType Type { get; set; }

		public WordMatchTokenizer(string word, TokenType type)
		{
			Word = word;
			Type = type;
		}

		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			char[] buffer = br.ReadChars(Word.Length);
			return new Token(Type, buffer);
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			if (peekChar == Word[0] && stream.Position < stream.Length)
			{
				var stringBuilder = new StringBuilder();

				for (int i = 0; i < Word.Length; i++)
				{
					if (Word[i] == br.PeekChar())
					{
						stringBuilder.Append(br.ReadChar());
						continue;
					}
					break;
				}

				if(stringBuilder.ToString() == Word)
				{
					return true;
				}
			}
			return false;
		}
	}

	public class QueryTokenizer : Tokenizer
	{
		private static readonly Tokenizer qTokenizer = new SingleCharTokenizer('q', TokenType.Query, false);
		private static readonly Tokenizer queryTokenizer = new WordMatchTokenizer("query", TokenType.Query);

		public static bool ContinueFeedingQuery(int chara)
		{
			return (chara >= 'a' && chara <= 'z') || (chara >= 'A' && chara <= 'Z');
		}

		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			var stringBuilder = new StringBuilder();
			while(ContinueFeedingQuery(br.PeekChar()))
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			return new Token(TokenType.Query, stringBuilder.ToString().ToCharArray());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			bool qMatch = qTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, existingTokens);
			bool queryMatch = queryTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, existingTokens);
			if ((queryMatch || qMatch) && br.ReadChar() == '.')
			{
				return true;
			}
			return false;
		}
	}

	public class DefinitionTokenizer : Tokenizer
	{
		private static readonly Tokenizer letTokenizer = new WordMatchTokenizer("let", TokenType.Definition);
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			br.ReadChars(3);
			var stringBuilder = new StringBuilder();
			while (br.PeekChar() != '=')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			variableDefs.Add(stringBuilder.ToString());
			return new Token(TokenType.Definition, stringBuilder.ToString().ToCharArray());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			if(letTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, existingTokens))
			{
				//TODO: Should this be more robust?
				return true;
			}
			return false;
		}
	}

	public class AssignmentTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (br.PeekChar() != '=')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			variableDefs.Add(stringBuilder.ToString());
			return new Token(TokenType.Assignment, stringBuilder.ToString().ToCharArray());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			if (!Utility.SearchForPotential((char)peekChar, variableDefs)) return false;

			StringBuilder stringBuilder = new StringBuilder();
			int check = 0;
			while (br.PeekChar() != '=')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
				check++;
			}
			string match = stringBuilder.ToString();
			Console.WriteLine(match);
			return variableDefs.Any(x => x == match);
		}
	}

	public class VariableTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int check = 0;
			while (!variableDefs.Contains(stringBuilder.ToString()) && check < 25)
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
				check++;
			}
			variableDefs.Add(stringBuilder.ToString());
			return new Token(TokenType.Variable, stringBuilder.ToString().ToCharArray());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			if (!Utility.SearchForPotential((char)peekChar, variableDefs)) return false;

			StringBuilder stringBuilder = new StringBuilder();
			int check = 0;
			while (!variableDefs.Contains(stringBuilder.ToString()) && check < 25)
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
				check++;
			}
			string match = stringBuilder.ToString();
			return variableDefs.Any(x => x == match);
		}
	}

	public class FunctionTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (!variableDefs.Contains(stringBuilder.ToString()) && br.PeekChar() != '(')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			string match = stringBuilder.ToString();
			return new Token(TokenType.Function, match.ToCharArray());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, List<Token>? existingTokens = null)
		{
			if (!AscentFunctions.SearchAnyFunctions((char)peekChar)) return false;
			StringBuilder stringBuilder = new StringBuilder();
			while (AscentFunctions.GetFunction(stringBuilder.ToString()) == null && br.PeekChar() != '(')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			string match = stringBuilder.ToString();
			return AscentFunctions.GetFunction(match) != null;
		}
	}
}
