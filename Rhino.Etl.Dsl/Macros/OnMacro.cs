namespace Rhino.Etl.Dsl.Macros
{
    using System.Collections.Generic;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using Core;

    /// <summary>
    /// The on part of a join, split into two parts, the first is the condition, the second
    /// is the merge
    /// </summary>
    public class OnMacro : AbstractChildMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnMacro"/> class.
        /// </summary>
        public OnMacro()
            : base("join")
        {
        }


        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected override Statement DoExpand(MacroStatement macro)
        {
            if (macro.Arguments.Count != 1)
            {
                Errors.Add(
                    CompilerErrorFactory.CustomError(macro.LexicalInfo,
                                                     "Only a single argument allowed for on statement"));
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
            mergeRowsMethod.Body.Add(macro.Block);
            mergeRowsMethod.Body.Add(new ReturnStatement(new ReferenceExpression("row")));

            ParentMethods.Add(mergeRowsMethod);


            Method matchJoinConditionMethod = new Method("MatchJoinCondition");
            matchJoinConditionMethod.ReturnType = new SimpleTypeReference(typeof (bool).FullName);
            matchJoinConditionMethod.Parameters.Add(new ParameterDeclaration("left", new SimpleTypeReference(typeof(Row).FullName)));
            matchJoinConditionMethod.Parameters.Add(new ParameterDeclaration("right", new SimpleTypeReference(typeof(Row).FullName)));
            matchJoinConditionMethod.Body.Add(
                new ReturnStatement(macro.Arguments[0])
                );
            
            ParentMethods.Add(matchJoinConditionMethod);

            return null;
        }
    }
}