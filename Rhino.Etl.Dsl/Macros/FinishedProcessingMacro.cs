
using System;

namespace Rhino.Etl.Dsl.Macros
{
    using Boo.Lang.Compiler.Ast;
    using Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Generate the <see cref="EtlProcess.OnFinishedProcessing"/> method
    /// </summary>
    public class FinishedProcessingMacro : AbstractChildMacro
    {
        ///<summary>
        /// Initializes a new instance of the <see cref="FinishedProcessingMacro"/> class.
        ///</summary>
        public FinishedProcessingMacro()
            : base("process")
        {
            
        }

        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected override Statement DoExpand(MacroStatement macro)
        {
            Method rowProcessed = new Method("OnFinishedProcessing");
            rowProcessed.Modifiers = TypeMemberModifiers.Override;
            rowProcessed.Parameters.Add(new ParameterDeclaration("op", CodeBuilder.CreateTypeReference(typeof(AbstractOperation))));
            
            rowProcessed.Body = macro.Block;

            ParentMethods.Add(rowProcessed);

            return null;
        }
    }
}
