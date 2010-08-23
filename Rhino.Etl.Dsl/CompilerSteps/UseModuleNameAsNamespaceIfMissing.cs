namespace Rhino.Etl.Dsl.CompilerSteps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;

    /// <summary>
    /// This adds the module name as a namespace for the module, if it doesn't have one already.
    /// </summary>
    public class UseModuleNameAsNamespaceIfMissing : AbstractVisitorCompilerStep
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            Visit(CompileUnit);
        }

        /// <summary>
        /// Inspect a module to check if we need to add a namespace
        /// </summary>
        /// <param name="node">The node.</param>
        public override void OnModule(Module node)
        {
            if (node.Namespace != null)
                return;

            node.Namespace = new NamespaceDeclaration(node.FullName);
        }
    }
}