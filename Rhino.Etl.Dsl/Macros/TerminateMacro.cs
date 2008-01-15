namespace Rhino.Etl.Dsl.Macros
{
    using Boo.Lang.Compiler.Ast;
    using Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Generate the <see cref="AbstractAggregationOperation.FinishAggregation"/> method
    /// </summary>
    public class TerminateMacro : AbstractChildMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminateMacro"/> class.
        /// </summary>
        public TerminateMacro()
            : base("aggregate")
        {
        }

        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected override Statement DoExpand(MacroStatement macro)
        {
            Method accumulate = new Method("FinishAggregation");
            accumulate.Modifiers = TypeMemberModifiers.Override;
            accumulate.Parameters.Add(new ParameterDeclaration("aggregate", CodeBuilder.CreateTypeReference(typeof(Row))));

            accumulate.Body = macro.Block;

            ParentMethods.Add(accumulate);

            return null;
        }
    }
}