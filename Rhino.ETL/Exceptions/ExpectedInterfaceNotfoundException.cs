using System;

namespace Rhino.ETL.Exceptions
{
	[global::System.Serializable]
	public class ExpectedInterfaceNotfoundException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ExpectedInterfaceNotfoundException() { }
		public ExpectedInterfaceNotfoundException(string message) : base(message) { }
		public ExpectedInterfaceNotfoundException(string message, Exception inner) : base(message, inner) { }
		protected ExpectedInterfaceNotfoundException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}