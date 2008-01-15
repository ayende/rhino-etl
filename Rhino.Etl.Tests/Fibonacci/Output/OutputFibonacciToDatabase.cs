namespace Rhino.Etl.Tests.Fibonacci.Output
{
    using Core;
    using Errors;

    public class OutputFibonacciToDatabase : EtlProcess
    {
        private readonly int max;
        private readonly Should should;

        public OutputFibonacciToDatabase(int max, Should should)
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
                Register(new ThrowingOperation());
            Register(new FibonacciOutput());
        }
    }
}