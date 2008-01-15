namespace Rhino.Etl.Tests.Errors
{
    using Core;

    public class ErrorsProcess : EtlProcess
    {
        protected override void Initialize()
        {
            Register(new ThrowingOperation());
        }
    }
}