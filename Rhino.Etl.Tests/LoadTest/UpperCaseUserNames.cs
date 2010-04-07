namespace Rhino.Etl.Tests.LoadTest
{
    using Core;
    using Rhino.Etl.Core.Operations;

    public class UpperCaseUserNames : EtlProcess
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            Register(new ReadUsers());
            Register(new UpperCaseColumn("Name"));
        }
    }
}