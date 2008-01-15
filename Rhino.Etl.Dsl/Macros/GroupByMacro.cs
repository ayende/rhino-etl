namespace Rhino.Etl.Dsl.Macros
{
    using System.Collections.Generic;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// Creates the <see cref="AbstractAggregationOperation.GetColumnsToGroupBy"/> method from the reference
    /// expressions
    /// </summary>
    public class GroupByMacro : AbstractChildMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupByMacro"/> class.
        /// </summary>
        public GroupByMacro() : base("aggregate")
        {
        }

        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected override Statement DoExpand(MacroStatement macro)
        {
            List<string> columns = new List<string>();

            if(macro.Block.HasStatements)
            {
                Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "GroupBy cannot contain statements"));
                return null;
            }
            foreach (Expression argument in macro.Arguments)
            {
                ReferenceExpression expr = argument as ReferenceExpression;
                if(expr==null)
                {
                    Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, "GroupBy arguments must be refernce expressions. Example: groupBy name, surname"));
                    return null;
                }
                columns.Add(expr.Name);
            }

            Method method = CreateGetColumnsToGroupByMethod(macro, columns);

            ParentMethods.Add(method);

            return null;
        }

        private Method CreateGetColumnsToGroupByMethod(MacroStatement macro, IEnumerable<string> columns)
        {
            Method method = new Method("GetColumnsToGroupBy");
            method.Modifiers = TypeMemberModifiers.Override;
            ArrayLiteralExpression ale = new ArrayLiteralExpression(macro.LexicalInfo);
            ale.Type = new ArrayTypeReference(CodeBuilder.CreateTypeReference(typeof(string)));
            
            foreach (string column in columns)
            {
                ale.Items.Add(new StringLiteralExpression(column));    
            }
            method.Body.Statements.Add(new ReturnStatement(ale));
            return method;
        }
    }
}