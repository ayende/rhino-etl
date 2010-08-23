namespace Rhino.Etl.Dsl.Macros
{
    using System.Collections.Generic;
    using Boo.Lang.Compiler.Ast;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Create a class based on <see cref="JoinOperation"/> and tranform the code
    /// into a join condition
    /// </summary>
    public class JoinMacro : AbstractClassGeneratorMacro<NestedLoopsJoinOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinMacro"/> class.
        /// </summary>
        public JoinMacro()
            : base("Initialize")
        {
        }
    }
}