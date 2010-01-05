namespace Rhino.Etl.Tests.Errors
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Joins;
    using Xunit;

    
    public class ErrorsFixture : BaseFibonacciTest
    {
        [Fact]
        public void WillReportErrorsWhenThrown()
        {
            using (ErrorsProcess process = new ErrorsProcess())
            {
                ICollection<Row> results = new List<Row>();
                process.RegisterLast(new AddToResults(results));

                process.Execute();
                Assert.Equal(process.ThrowOperation.RowsAfterWhichToThrow, results.Count);
                List<Exception> errors = new List<Exception>(process.GetAllErrors());
                Assert.Equal(1, errors.Count);
                Assert.Equal("Failed to execute operation Rhino.Etl.Tests.Errors.ThrowingOperation: problem",
                                errors[0].Message);
            }
        }

        [Fact]
        public void OutputCommandWillRollbackTransactionOnError()
        {
            using (ErrorsProcess process = new ErrorsProcess())
            {
              
                
            }
        }
    }
}
