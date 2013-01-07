using Rhino.Etl.Core;
using Rhino.Etl.Tests.Errors;
using Rhino.Etl.Tests.Fibonacci.Output;

namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    public class SlowBatchFibonacci : EtlProcess
    {
        private readonly int max;
        protected int Max
        {
            get
            {
                return max;
            }
        }

        private readonly Should should;
        protected Should Should
        {
            get
            {
                return should;
            }
        }

        public SlowBatchFibonacci(int max, Should should)
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
            Register(new SlowBatchFibonacciToDatabase());
        }
    }
}