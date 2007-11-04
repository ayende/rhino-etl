using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.ETL.Exceptions
{

	[global::System.Serializable]
	public class ScriptEvalException : Exception
	{
		public ScriptEvalException() { }
		public ScriptEvalException(string message) : base(message) { }
		public ScriptEvalException(string message, Exception inner) : base(message, inner) { }
		protected ScriptEvalException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
