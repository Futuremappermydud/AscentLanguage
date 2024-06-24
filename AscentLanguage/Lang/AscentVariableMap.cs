using AscentLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscentLanguage
{
	public class AscentVariableMap
	{
		public AscentVariableMap(Dictionary<string, float> queryVariables)
		{
			QueryVariables = queryVariables;
		}

		public AscentVariableMap Clone()
		{
			var clone = new AscentVariableMap(QueryVariables);
			clone.Variables = new Dictionary<string, float>(Variables);
			clone.Functions = new Dictionary<string, FunctionDefinition>(Functions);
			return clone;
		}

		public Dictionary<string, float> QueryVariables { get; set; }
		public Dictionary<string, float> Variables { get; set; } = new Dictionary<string, float>();
		internal Dictionary<string, FunctionDefinition> Functions { get; set; } = new Dictionary<string, FunctionDefinition>();
	}
}
