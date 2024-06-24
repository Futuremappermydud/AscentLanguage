using AscentLanguage.Functions;
using AscentLanguage.Parser;
using AscentLanguage.Util;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace AscentLanguage.Tokenizer
{
	public abstract class Tokenizer
	{
		public abstract Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope);
		public abstract bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null);
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

		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			if (stream.Position >= stream.Length && HasOperand) throw new FormatException("Missing Operand!");
			return new Token(Type, br.ReadChar());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			return peekChar == Token;
		}
	}

	public class SubtractionTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			if (stream.Position >= stream.Length) throw new FormatException("Missing Operand!");
			return new Token(TokenType.Subtraction, '-');
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
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
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (IsNumber(br.PeekChar()))
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			return new Token(TokenType.Constant, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
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

		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			char[] buffer = br.ReadChars(Word.Length);
			return new Token(Type, new string(buffer));
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
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

		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			var stringBuilder = new StringBuilder();
			while(ContinueFeedingQuery(br.PeekChar()))
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			return new Token(TokenType.Query, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			bool qMatch = qTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, ref functionDefs, scope, existingTokens);
			bool queryMatch = queryTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, ref functionDefs, scope, existingTokens);
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
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			br.ReadChars(3);
			var stringBuilder = new StringBuilder();
			while (br.PeekChar() != '=')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			variableDefs.Add(stringBuilder.ToString());
			return new Token(TokenType.Definition, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			if(letTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, ref functionDefs, scope, existingTokens))
			{
				//TODO: Should this be more robust?
				return true;
			}
			return false;
		}
	}

	public class AssignmentTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (br.PeekChar() != '=')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			return new Token(TokenType.Assignment, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
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
			return variableDefs.Any(x => x == match);
		}
	}

	public class VariableTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			var args = functionDefs?.FirstOrDefault(x => x.name == scope)?.args ?? new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			int check = 0;
			while (!variableDefs.Contains(stringBuilder.ToString()) && !args.Contains(stringBuilder.ToString()) && check < 25)
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
				check++;
			}
			return new Token(TokenType.Variable, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			var args = functionDefs?.FirstOrDefault(x => x.name == scope)?.args ?? new List<string>();
			if (!Utility.SearchForPotential((char)peekChar, variableDefs) && !Utility.SearchForPotential((char)peekChar, args)) return false;

			StringBuilder stringBuilder = new StringBuilder();
			int check = 0;
			while (!variableDefs.Contains(stringBuilder.ToString()) && !args.Contains(stringBuilder.ToString()) && check < 25)
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
				check++;
			}
			string match = stringBuilder.ToString();
			return variableDefs.Any(x => x == match) || args.Contains(match);
		}
	}

	public class FunctionTokenizer : Tokenizer
	{
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (!variableDefs.Contains(stringBuilder.ToString()) && br.PeekChar() != '(')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			string match = stringBuilder.ToString();
			return new Token(TokenType.Function, match);
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			if (!AscentFunctions.SearchAnyFunctions((char)peekChar) && !Utility.SearchForPotential((char)peekChar, functionDefs.Select(x=>x.name))) return false;
			StringBuilder stringBuilder = new StringBuilder();
			while (AscentFunctions.GetFunction(stringBuilder.ToString()) == null && br.PeekChar() != '(')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			string match = stringBuilder.ToString();
			return AscentFunctions.GetFunction(match) != null || functionDefs.Any(x=>x.name == match);
		}
	}

	public class FunctionDefinitionTokenizer : Tokenizer
	{
		private static readonly Tokenizer functionTokenizer = new WordMatchTokenizer("function", TokenType.FunctionDefinition);
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			functionTokenizer.GetToken(peekChar, br, stream, ref variableDefs, ref functionDefs, scope);
			StringBuilder stringBuilder = new StringBuilder();
			while (br.PeekChar() != '(' && br.PeekChar() != '{')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			functionDefs.Add(new FunctionDefinition(stringBuilder.ToString()));
			return new Token(TokenType.FunctionDefinition, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			return functionTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, ref functionDefs, scope, existingTokens);
		}
	}

	public class ForLoopTokenizer : Tokenizer
	{
		private static readonly Tokenizer forTokenizer = new WordMatchTokenizer("for", TokenType.ForLoop);
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			return forTokenizer.GetToken(peekChar, br, stream, ref variableDefs, ref functionDefs, scope);
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			return forTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, ref functionDefs, scope, existingTokens);
		}
	}

	public class WhileLoopTokenizer : Tokenizer
	{
		private static readonly Tokenizer whileTokenizer = new WordMatchTokenizer("while", TokenType.WhileLoop);
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			return whileTokenizer.GetToken(peekChar, br, stream, ref variableDefs, ref functionDefs, scope);
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			return whileTokenizer.IsMatch(peekChar, br, stream, ref variableDefs, ref functionDefs, scope, existingTokens);
		}
	}

	public class FunctionArgumentTokenizer : Tokenizer
	{
		private string name = "";
		public override Token GetToken(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (br.PeekChar() != ',' && br.PeekChar() != ')')
			{
				stringBuilder.Append(br.ReadChar());
				if (stream.Position >= stream.Length) break;
			}
			functionDefs.First(x => x.name == name).args.Add(stringBuilder.ToString());
			return new Token(TokenType.FunctionArgument, stringBuilder.ToString());
		}

		public override bool IsMatch(int peekChar, BinaryReader br, MemoryStream stream, ref List<string> variableDefs, ref List<FunctionDefinition> functionDefs, string scope, List<Token>? existingTokens = null)
		{
			TokenType[] allowedTokens = [TokenType.LeftParenthesis, TokenType.Comma, TokenType.FunctionArgument];
			int back = 0;
			while (existingTokens != null && existingTokens.Count > back && allowedTokens.Contains(existingTokens[existingTokens.Count - back - 1].type))
			{
				back++;
			}
			if(existingTokens == null || existingTokens.Count < back + 1) return false;
			var def = existingTokens[existingTokens.Count - back - 1];
			if (existingTokens[existingTokens.Count - back - 1].type == TokenType.FunctionDefinition)
			{
				name = def.tokenBuffer;
				return true;
			}
			return false;
		}
	}
}
