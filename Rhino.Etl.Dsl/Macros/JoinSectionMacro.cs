using System.Collections.Generic;

namespace Rhino.Etl.Dsl.Macros
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;

    /// <summary>
    /// A part of a join
    /// </summary>
    public abstract class JoinSectionMacro : AbstractChildMacro
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinSectionMacro"/> class.
        /// </summary>
        public JoinSectionMacro(string name)
            : base("join", "leftjoin", "rightjoin", "innerjoin", "fulljoin")
        {
            this.name = name;
        }

        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected override Statement DoExpand(MacroStatement macro)
        {
            Block body = (Block)macro.GetAncestor(NodeType.Block);
            MacroStatement macroParent = (MacroStatement)macro.GetAncestor(NodeType.MacroStatement);
           
            if (macro.Body.Statements.Count < 1 && parent.Name=="join")
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "Join " + name + " section must contain at least a single expression statement"));
                return null;
            }

            if (macro.Arguments.Count == 0 && parent.Name!="join")
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "Join " + name + " section must contain at least one key column"));
                return null;
            }

            if (macro.Arguments.Count > 0)
            {
                ArrayLiteralExpression ale = new ArrayLiteralExpression(macro.LexicalInfo);
                ale.Type = new ArrayTypeReference(CodeBuilder.CreateTypeReference(typeof(string)));
                foreach (Expression argument in macro.Arguments)
                {
                    ReferenceExpression expr = argument as ReferenceExpression;
                    if (expr == null)
                    {
                        Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo,"Join " + name +" section arguments must be reference expressions. Example: " + name + " name, surname"));
                        return null;
                    }
                    ale.Items.Add(new StringLiteralExpression(expr.Name));
                }
                var keyExpr = new BinaryExpression(BinaryOperatorType.Assign, new ReferenceExpression(name+"KeyColumns"), ale);
                macroParent.Arguments.Add(keyExpr);
            }

            foreach (Statement statement in macro.Body.Statements)
            {
                ExpressionStatement exprStmt = statement as ExpressionStatement;
                if(exprStmt==null)
                {
                    Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "Join " + name + " section can only contain expressions"));
                    return null;
                }
                Expression expr = exprStmt.Expression;
                parent.Arguments.Add(new MethodInvocationExpression(new ReferenceExpression(name), expr));
            }

            return null;
            
        }
    }
}