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
            : base("join")
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
           
            if (macro.Block.Statements.Count < 1)
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "Join"+name+" section must contain at least a single expression statement"));
                return null;
            }

            foreach (Statement statement in macro.Block.Statements)
            {
                ExpressionStatement exprStmt = statement as ExpressionStatement;
                if(exprStmt==null)
                {
                    Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "Join" + name + " section can only contain expressions"));
                    return null;
                }
                Expression expr = exprStmt.Expression;
                MethodInvocationExpression expression = new MethodInvocationExpression(new ReferenceExpression(name), expr);
                body.Add(new ExpressionStatement(expression));
            }
            return null;
            
        }
    }
}