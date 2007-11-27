using System;

namespace Rhino.ETL.Exceptions
{
	[global::System.Serializable]
	public class InvalidWebServiceException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public InvalidWebServiceException() { }
		public InvalidWebServiceException(string message) : base(message) { }
		public InvalidWebServiceException(string message, Exception inner) : base(message, inner) { }
		protected InvalidWebServiceException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}