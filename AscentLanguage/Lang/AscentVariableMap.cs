﻿using AscentLanguage.Parser;
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

		public Dictionary<string, float> QueryVariables { get; set; }
		public Dictionary<string, float> Variables { get; set; } = new Dictionary<string, float>();
		internal Dictionary<string, Expression[]> Functions { get; set; } = new Dictionary<string, Expression[]>();
	}
}
