namespace Rhino.Etl.Tests.Errors
{
    using Core;

    public class ErrorsProcess : EtlProcess
    {
        public readonly ThrowingOperation ThrowOperation = new ThrowingOperation();

        protected override void Initialize()
        {
            Register(ThrowOperation);
        }
    }
}
