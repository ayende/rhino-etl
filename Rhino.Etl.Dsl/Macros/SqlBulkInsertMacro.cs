using System;
using Boo.Lang.Compiler.Ast;

namespace Rhino.Etl.Dsl.Macros
{
    using Core.ConventionOperations;

    /// <summary>
    /// Create a new <see cref="ConventionSqlBulkInsertOperation"/> and instantiate it in place of the 
    /// macro
    /// </summary>
    public class SqlBulkInsertMacro : AbstractClassGeneratorMacro<ConventionSqlBulkInsertOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInsertMacro"/> class.
        /// </summary>
        public SqlBulkInsertMacro() : base("PrepareSchema")
        {
        }

        /// <summary>Expands the macro and replaces calls to map with calls to MapColumn</summary>
        /// <param name="macro">The macro to expand</param>
        /// <returns>The expanded macro statement</returns>
        /// <remarks>This prohibits using the builtin map macro!</remarks>
        public override Boo.Lang.Compiler.Ast.Statement Expand(MacroStatement macro)
        {
            if (macro.Block != null)
            {
                foreach(Statement statement in macro.Block.Statements)
                {
                    if (IsMappingStatement(statement))
                    {
                        MethodInvocationExpression mappingCall = GetMappingStatement(statement);
                        mappingCall.Target = new ReferenceExpression("MapColumn");
                    }
                }
            }
            return base.Expand(macro);
        }

        private static MethodInvocationExpression GetMappingStatement(Statement statement)
        {
            return (MethodInvocationExpression)((ExpressionStatement) statement).Expression;
        }

        private static bool IsMappingStatement(Statement statement)
        {
            ExpressionStatement expressionStatement = statement as ExpressionStatement;
            if (expressionStatement != null)
            {
                MethodInvocationExpression invocationExpression = expressionStatement.Expression as MethodInvocationExpression;
                if (invocationExpression != null)
                {
                    return invocationExpression.Target.ToString().Equals("map", StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return false;
        }
    }



}