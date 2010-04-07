namespace Rhino.Etl.Tests.LoadTest
{
    using Core;

    public class PushDataToDatabase : EtlProcess
    {
        public PushDataToDatabase(int expectedCount)
        {
            this.expectedCount = expectedCount;
        }

        private int expectedCount;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            Register(new GenerateUsers(expectedCount));
            Register(new BulkInsertUsers());
        }
    }
}