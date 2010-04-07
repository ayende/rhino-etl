namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class ExportUsersToFile : EtlProcess
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            Register(new GetAllUsers());
            Register(new WriteUsersToFile());
        }
    }
}