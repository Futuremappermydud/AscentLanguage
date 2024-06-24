using AscentLanguage.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage.Splitter
{
	public class TokenContainer(TokenContainer? parentContainer)
	{
		public TokenContainer? parentContainer { get; set; } = parentContainer;
	}
	public class SingleTokenContainer(TokenContainer parentContainer, Token[] expression) : TokenContainer(parentContainer)
	{
		public Token[] Expression { get; set; } = expression;
	}

	public class MultipleTokenContainer(TokenContainer? parentContainer) : TokenContainer(parentContainer)
	{
		public List<TokenContainer> tokenContainers { get; set; } = new();
	}
}
