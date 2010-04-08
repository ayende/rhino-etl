namespace Rhino.Etl.Tests.Integration
{
    using Core;

    public class UsersToPeople : EtlProcess
    {
        protected override void Initialize()
        {
            Register(new ReadUsers());
            Register(new SplitName());
            Register(new WritePeople());
        }
    }
}