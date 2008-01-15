namespace Rhino.Etl.Dsl.Macros
{
    using Boo.Lang.Compiler.Ast;
    using Core;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Generate the <see cref="AbstractAggregationOperation.Accumulate"/> method
    /// </summary>
    public class AccumulateMacro : AbstractChildMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccumulateMacro"/> class.
        /// </summary>
        public AccumulateMacro()
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
            Method accumulate = new Method("Accumulate");
            accumulate.Modifiers = TypeMemberModifiers.Override;
            accumulate.Parameters.Add(new ParameterDeclaration("row",CodeBuilder.CreateTypeReference(typeof(Row))));
            accumulate.Parameters.Add(new ParameterDeclaration("aggregate", CodeBuilder.CreateTypeReference(typeof(Row))));

            accumulate.Body = macro.Block;

            ParentMethods.Add(accumulate);

            return null;
        }
    }
}