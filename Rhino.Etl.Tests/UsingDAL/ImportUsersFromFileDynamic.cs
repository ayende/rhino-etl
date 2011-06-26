namespace Rhino.Etl.Tests.UsingDAL
{
    using Core;

    public class ImportUsersFromFileDynamic : EtlProcess
    {
        protected override void Initialize()
        {
            Register(new ReadUsersFromFileDynamic());
            Register(new SaveToDal());
        }
    }
}