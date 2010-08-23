namespace Rhino.Etl.Dsl.Macros
{
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Create a class based on <see cref="AbstractAggregationOperation"/>
    /// </summary>
    public class AggregateMacro : AbstractClassGeneratorMacro<AbstractAggregationOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateMacro"/> class.
        /// </summary>
        public AggregateMacro() : base(null)
        {
        }
    }
}