using System;

namespace Rhino.ETL.Exceptions
{
    [global::System.Serializable]
    public class ParameterCountException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ParameterCountException() { }
        public ParameterCountException(string message) : base(message) { }
        public ParameterCountException(string message, Exception inner) : base(message, inner) { }
        protected ParameterCountException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}