namespace Rhino.Etl.Dsl.Macros
{
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Generate a class dervied from <see cref="AbstractOperation"/> with the 
    /// required initialization
    /// </summary>
    public class OperationMacro : AbstractClassGeneratorMacro<AbstractOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMacro"/> class.
        /// </summary>
        public OperationMacro() : base("Execute")
        {
        }
    }
}