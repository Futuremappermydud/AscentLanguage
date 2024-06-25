namespace AscentLanguage.Tokenizer
{
	public enum TokenType // All types of tokens.
	{
		Query,
		Constant,
		Increment,
		Decrement,
		AdditionAssignment,
		SubtractionAssignment,
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
		ForLoop,
		WhileLoop,
		Return
	}
	public struct Token
	{
		public TokenType type;
		public string tokenBuffer = ""; // Useful Buffer for token. For operators, it's a single char for the operation. For variables, it's the variable name. For numbers, it's the number. For Function Defs it's the function name. Etc.

		public Token(TokenType type, string tokenBuffer)
		{
			this.type = type;
			this.tokenBuffer = tokenBuffer;
		}

		public Token(TokenType type, char token)
		{
			this.type = type;
			this.tokenBuffer = new string([token]);
		}
	}
}
