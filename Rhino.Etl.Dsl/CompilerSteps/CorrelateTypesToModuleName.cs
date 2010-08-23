namespace Rhino.Etl.Dsl.CompilerSteps
{
    using System.Collections.Generic;
    using Boo.Lang.Compiler.Steps;

    /// <summary>
    /// Correlate between the types defined in a module and their module.
    /// This is required to get batch compilation working.
    /// </summary>
    public class CorrelateTypesToModuleName : AbstractVisitorCompilerStep
    {
        private readonly IDictionary<string, IList<string>> moduleNameToContainedTypes;
        private string currentModule;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelateTypesToModuleName"/> class.
        /// </summary>
        /// <param name="moduleNameToContainedTypes">The module name to contained types.</param>
        public CorrelateTypesToModuleName(IDictionary<string, IList<string>> moduleNameToContainedTypes)
        {
            this.moduleNameToContainedTypes = moduleNameToContainedTypes;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            Visit(CompileUnit);
        }

        /// <summary>
        /// Enters the module.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public override bool EnterModule(Boo.Lang.Compiler.Ast.Module node)
        {
            currentModule = node.Name;
            if (moduleNameToContainedTypes.ContainsKey(currentModule) == false)
                moduleNameToContainedTypes[currentModule] = new List<string>();
            return base.EnterModule(node);
        }

        /// <summary>
        /// Called when a class definition node is found
        /// </summary>
        /// <param name="node">The node.</param>
        public override void OnClassDefinition(Boo.Lang.Compiler.Ast.ClassDefinition node)
        {
            moduleNameToContainedTypes[currentModule].Add(node.Name);
        }
    }
}