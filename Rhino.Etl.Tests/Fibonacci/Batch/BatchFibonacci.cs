namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    using Core;
    using Errors;
    using Output;

    public class BatchFibonacci : EtlProcess
    {
        private readonly int max;
        private readonly Should should;

        public BatchFibonacci(int max, Should should)
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
            Register(new BatchFibonacciToDatabase());
        }
    }
}