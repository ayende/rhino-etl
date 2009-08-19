
using System;

namespace Rhino.Etl.Dsl.Macros
{
    using Boo.Lang.Compiler.Ast;
    using Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Generate the <see cref="EtlProcess.OnRowProcessed"/> method
    /// </summary>
    public class RowProcessedMacro : AbstractChildMacro
    {
        ///<summary>
        /// Initializes a new instance of the <see cref="RowProcessedMacro"/> class.
        ///</summary>
        public RowProcessedMacro()
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
            Method rowProcessed = new Method("OnRowProcessed");
            rowProcessed.Modifiers = TypeMemberModifiers.Override;
            rowProcessed.Parameters.Add(new ParameterDeclaration("op", CodeBuilder.CreateTypeReference(typeof(IOperation))));
            rowProcessed.Parameters.Add(new ParameterDeclaration("row", CodeBuilder.CreateTypeReference(typeof(Row))));

            rowProcessed.Body = macro.Block;

            ParentMethods.Add(rowProcessed);

            return null;
        }
    }
}
