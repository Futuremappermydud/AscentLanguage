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

		public Dictionary<string, float> QueryVariables { get; set; } = new Dictionary<string, float>();
		public Dictionary<string, float> Variables { get; set; } = new Dictionary<string, float>();
	}
}
