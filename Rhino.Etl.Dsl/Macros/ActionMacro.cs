using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Rhino.Etl.Core;

namespace Rhino.Etl.Dsl.Macros
{
    /// <summary>
    /// The on part of a join, split into two parts, the first is the condition, the second
    /// is the merge
    /// </summary>
    public class ActionMacro : AbstractChildMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnMacro"/> class.
        /// </summary>
        public ActionMacro()
            : base("leftjoin","rightjoin","innerjoin","fulljoin")
        {
        }


        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected override Statement DoExpand(MacroStatement macro)
        {
            if (macro.Arguments.Count != 0)
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo,"No arguments allowed for action statement"));
                return null;
            }

            Method mergeRowsMethod = new Method("MergeRows");
            mergeRowsMethod.Modifiers = TypeMemberModifiers.Override;
            mergeRowsMethod.Parameters.Add(new ParameterDeclaration("left", new SimpleTypeReference(typeof(Row).FullName)));
            mergeRowsMethod.Parameters.Add(new ParameterDeclaration("right", new SimpleTypeReference(typeof(Row).FullName)));
            CodeBuilder.DeclareLocal(mergeRowsMethod, "row", TypeSystemServices.Map(typeof(Row)));
            mergeRowsMethod.Body.Add(
                new BinaryExpression(BinaryOperatorType.Assign,
                                     new ReferenceExpression("row"),
                                     new MethodInvocationExpression(
                                         AstUtil.CreateReferenceExpression(typeof(Row).FullName))
                    )
                );
            mergeRowsMethod.Body.Add(macro.Body);
            mergeRowsMethod.Body.Add(new ReturnStatement(new ReferenceExpression("row")));

            ParentMethods.Add(mergeRowsMethod);
            return null;
        }
    }
}