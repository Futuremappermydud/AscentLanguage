namespace AscentLanguage.Parser
{
	public class FunctionDefinition
	{
		public FunctionDefinition(string name)
		{
			this.name = name;
		}

		public string name { get; set; }
		public List<string> args { get; set; } = new List<string>();
		public Expression[] contents { get; set; }
		public bool defined { get; set; } //TODO: use this
	}
}
