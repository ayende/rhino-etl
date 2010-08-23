namespace Rhino.Etl.Dsl.Macros
{
    using Rhino.Etl.Core.ConventionOperations;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Create a new <see cref="OutputCommandOperation"/> and instantiate it in place of the 
    /// macro
    /// </summary>
    public class OutputMacro : AbstractClassGeneratorMacro<ConventionOutputCommandOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputMacro"/> class.
        /// </summary>
        public OutputMacro() : base("PrepareRow")
        {

        }
    }
}