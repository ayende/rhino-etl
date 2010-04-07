using System.Configuration;

namespace Rhino.Etl.Tests.Fibonacci.Output
{
    using Rhino.Etl.Core.ConventionOperations;
    using Rhino.Etl.Core.Operations;

    public class FibonacciOutput : ConventionOutputCommandOperation
    {
        public FibonacciOutput() : base("test")
        {
            Command = "INSERT INTO Fibonacci (Id) VALUES(@Id)";
        }

        public FibonacciOutput(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings)
        {
            Command = "INSERT INTO Fibonacci (Id) VALUES(@Id)";
        }
    }
}