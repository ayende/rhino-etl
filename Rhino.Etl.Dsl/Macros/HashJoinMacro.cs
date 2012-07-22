using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Dsl.Macros
{
    /// <summary>
    /// Create a class based on <see cref="JoinOperation"/> and tranform the code
    /// into a join condition
    /// </summary>
    public abstract class HashJoinMacro : AbstractClassGeneratorMacro<JoinOperation>
    {
        private readonly JoinType JoinType;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinMacro"/> class.
        /// </summary>
        protected HashJoinMacro(JoinType joinType)
            : base("Initialize")
        {
            JoinType = joinType;
        }

        /// <summary>
        /// Expands the specified macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        public override Statement Expand(MacroStatement macro)
        {
            ICollection<StringLiteralExpression> leftKeys = null;
            ICollection<StringLiteralExpression> rightKeys = null;

            foreach(var arg in macro.Arguments)
            {
                BinaryExpression assignment;
                if (!TryGetAssignment(arg, out assignment)) continue;

                var rf = assignment.Left as ReferenceExpression;
                if (rf == null) continue;

                var val = assignment.Right as ArrayLiteralExpression;
                if (val == null) continue;

                if (rf.Name != "RightKeyColumns" && rf.Name != "LeftKeyColumns") continue;

                var lst = Enumerable.OfType<StringLiteralExpression>(val.Items).ToList();
                if (rf.Name == "RightKeyColumns") 
                    rightKeys = lst;
                else
                    leftKeys = lst;
            }

            if (leftKeys==null || leftKeys.Count==0)
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "LeftKeys must be defined for a hash join"));
                return null;
            }

            if (rightKeys == null || rightKeys.Count==0)
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "RightKeys must be defined for a hash join"));
                return null;
            }

            if (rightKeys.Count != leftKeys.Count)
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "Number of key columns must be the same for both sides of the join"));
            }

            var method = SetupJoinMethodDefinition(leftKeys, rightKeys);
            AddMethodDefinitionToClassDefinition(macro, method);
            return base.Expand(macro);
        }

        private Method SetupJoinMethodDefinition(IEnumerable<StringLiteralExpression> leftKeys, IEnumerable<StringLiteralExpression> rightKeys)
        {
            var method = new Method("SetupJoinConditions");
            method.Modifiers = TypeMemberModifiers.Override;

            CodeBuilder.DeclareLocal(method, "join", TypeSystemServices.Map(typeof(JoinOperation.JoinBuilder)));
            ReferenceExpression joinConstructor;
            switch (JoinType)
            {
                case JoinType.Full:
                    joinConstructor = new ReferenceExpression("FullOuterJoin");
                    break;
                case JoinType.Left:
                    joinConstructor = new ReferenceExpression("LeftJoin");
                    break;
                case JoinType.Right:
                    joinConstructor = new ReferenceExpression("RightJoin");
                    break;
                case JoinType.Inner:
                    joinConstructor = new ReferenceExpression("InnerJoin");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            method.Body.Add(new BinaryExpression(BinaryOperatorType.Assign, new ReferenceExpression("join"), joinConstructor));
            method.Body.Add(new MethodInvocationExpression(new MemberReferenceExpression(new ReferenceExpression("join"), "Left"), leftKeys.OfType<Expression>().ToArray()));
            method.Body.Add(new MethodInvocationExpression(new MemberReferenceExpression(new ReferenceExpression("join"), "Right"), rightKeys.OfType<Expression>().ToArray()));
            return method;
        }
    }
}