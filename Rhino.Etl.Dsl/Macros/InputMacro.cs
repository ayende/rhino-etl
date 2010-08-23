namespace Rhino.Etl.Dsl.Macros
{
    using System;
    using Rhino.Etl.Core.ConventionOperations;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Create a new <see cref="InputCommandOperation"/> and instantiate it in place of the
    /// macro
    /// </summary>
    public class InputMacro : AbstractClassGeneratorMacro<ConventionInputCommandOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputMacro"/> class.
        /// </summary>
        public InputMacro() : base(null)
        {
        }
    }
}