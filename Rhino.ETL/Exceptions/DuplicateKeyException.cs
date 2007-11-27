using System;

namespace Rhino.ETL.Exceptions
{
	[global::System.Serializable]
	public class DuplicateKeyException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public DuplicateKeyException() { }
		public DuplicateKeyException(string message) : base(message) { }
		public DuplicateKeyException(string message, Exception inner) : base(message, inner) { }
		protected DuplicateKeyException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}