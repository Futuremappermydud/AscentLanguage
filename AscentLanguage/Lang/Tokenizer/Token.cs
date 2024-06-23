namespace AscentLanguage.Tokenizer
{
	public enum TokenType
	{
		Query,
		Constant,
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Modulus,
		LeftParenthesis,
		RightParenthesis,
		LeftBracket,
		RightBracket,
		Pow,
		LesserThen,
		GreaterThan,
		TernaryConditional,
		Colon,
		Comma,
		Definition,
		Assignment,
		Variable,
		SemiColon,
		Function,
		FunctionDefinition,
		LeftScope,
		RightScope,
		FunctionArgument,
	}
	public struct Token
	{
		public TokenType type;
		public char[] tokenBuffer = new char[1];

		public Token(TokenType type, char[] tokenBuffer)
		{
			this.type = type;
			this.tokenBuffer = tokenBuffer;
		}

		public Token(TokenType type, char token)
		{
			this.type = type;
			this.tokenBuffer[0] = token;
		}
	}
}
