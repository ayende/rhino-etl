using System.Configuration;

namespace Rhino.Etl.Tests.Fibonacci.Output
{
    using Core;
    using Errors;

    public class OutputFibonacciToDatabaseFromConnectionStringSettings : EtlProcess
    {
        private readonly int max;
        private readonly Should should;
        public readonly ThrowingOperation ThrowingOperation = new ThrowingOperation();
        public readonly FibonacciOutput OutputOperation = new FibonacciOutput(ConfigurationManager.ConnectionStrings["Test"]);

        public OutputFibonacciToDatabaseFromConnectionStringSettings(int max, Should should)
        {
            this.max = max;
            this.should = should;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            Register(new FibonacciOperation(max));
            if (should == Should.Throw)
                Register(ThrowingOperation);
            Register(OutputOperation);
        }
    }
}
