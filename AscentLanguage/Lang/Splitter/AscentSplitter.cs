using AscentLanguage.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage.Splitter
{
	public static class AscentSplitter
	{
		public static TokenContainer SplitTokens(List<Token> tokens)
		{
			var rootContainer = new MultipleTokenContainer(null);
			int _position = 0;
			List<Token> buffer = new List<Token>();
			MultipleTokenContainer? currentScope = rootContainer;
			while (_position < tokens.Count)
			{
				var token = tokens[_position];
				if (token.type == TokenType.LeftScope)
				{
					if (buffer.Count > 0)
					{
						buffer.Add(token);
						_position++;
						token = tokens[_position];
						currentScope?.tokenContainers.Add(new SingleTokenContainer(currentScope, buffer.ToArray()));
						buffer.Clear();
					}
				}
				if (token.type == TokenType.RightScope)
				{
					if (buffer.Count > 0)
					{
						currentScope?.tokenContainers.Add(new SingleTokenContainer(currentScope, buffer.ToArray()));
						buffer.Clear();

					}
					currentScope?.tokenContainers.Add(new SingleTokenContainer(currentScope, [token]));
					currentScope = currentScope?.parentContainer as MultipleTokenContainer;
				}
				if (token.type == TokenType.FunctionDefinition)
				{
					var newScope = new MultipleTokenContainer(currentScope);
					currentScope?.tokenContainers.Add(newScope);
					currentScope = newScope;
				}
				if (token.type != TokenType.SemiColon)
				{
					if (token.type != TokenType.RightScope)
						buffer.Add(token);
				}
				else
				{
					currentScope?.tokenContainers.Add(new SingleTokenContainer(currentScope, buffer.ToArray()));
					buffer.Clear();
				}
				_position++;
			}
			return rootContainer;
		}
	}
}
