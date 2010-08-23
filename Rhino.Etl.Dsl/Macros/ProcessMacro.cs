namespace Rhino.Etl.Dsl.Macros
{
    using System.Collections.Generic;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Generate a clsas derived from <see cref="EtlProcess"/> with the 
    /// required initialization
    /// </summary>
    public class ProcessMacro : AbstractClassGeneratorMacro<EtlProcess>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessMacro"/> class.
        /// </summary>
        public ProcessMacro() : base("Initialize")
        {
        }

        /// <summary>
        /// Expands the macro, create a new class and transform all the expression statements in the 
        /// macro block to Register() calls.
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        public override Statement Expand(MacroStatement macro)
        {
            Statement expand = base.Expand(macro);
            foreach (Statement statement in macro.Block.Statements)
            {
                ExpressionStatement expressionStatement = statement as ExpressionStatement;
                if(expressionStatement==null)
                    continue;
                expressionStatement.Expression =
                    new MethodInvocationExpression(new ReferenceExpression("Register"), expressionStatement.Expression);
            }
            return expand;
        }
    }
}