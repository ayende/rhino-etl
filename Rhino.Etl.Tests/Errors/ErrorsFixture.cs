namespace Rhino.Etl.Tests.Errors
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Joins;
    using MbUnit.Framework;

    [TestFixture]
    public class ErrorsFixture : BaseFibonacciTest
    {
        [Test]
        public void WillReportErrorsWhenThrown()
        {
            using (ErrorsProcess process = new ErrorsProcess())
            {
                ICollection<Row> results = new List<Row>();
                process.RegisterLast(new AddToResults(results));

                process.Execute();
                Assert.AreEqual(process.ThrowOperation.RowsAfterWhichToThrow, results.Count);
                List<Exception> errors = new List<Exception>(process.GetAllErrors());
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("Failed to execute operation Rhino.Etl.Tests.Errors.ThrowingOperation: problem",
                                errors[0].Message);
            }
        }

        [Test]
        public void OutputCommandWillRollbackTransactionOnError()
        {
            using (ErrorsProcess process = new ErrorsProcess())
            {
              
                
            }
        }
    }
}
