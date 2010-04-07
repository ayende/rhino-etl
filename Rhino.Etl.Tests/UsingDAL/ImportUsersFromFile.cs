namespace Rhino.Etl.Tests.UsingDAL
{
	using Core;

	public class ImportUsersFromFile : EtlProcess
    {
        protected override void Initialize()
        {
            Register(new ReadUsersFromFile());
            Register(new SaveToDal());
        }
    }
}