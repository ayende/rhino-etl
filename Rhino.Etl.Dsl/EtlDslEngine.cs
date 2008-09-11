namespace Rhino.Etl.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Pipelines;
    using Boo.Lang.Compiler.Steps;
    using Commons;
    using CompilerSteps;
    using Core;
    using DSL;

    /// <summary>
    /// The Etl DSL engine
    /// </summary>
    public class EtlDslEngine : DslEngine
    {
        private static readonly DslFactory factory = CreateFactory();
        private readonly IDictionary<string, IList<string>> moduleNameToContainedTypes = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Get a type from the assembly according to the URL.
        /// Here we are making the assumption that we will have only a single class
        /// inheriting from EtlProcess in the assembly
        /// </summary>
        public override Type GetTypeForUrl(Assembly assembly, string url)
        {
            string moduleName = Path.GetFileNameWithoutExtension(url);
            IList<string> typesInCurrentModule;
            if (moduleNameToContainedTypes.TryGetValue(moduleName, out typesInCurrentModule) == false)
                throw new InvalidOperationException("DSL Error: Module " + moduleName + " was not processed correctly");
            System.Collections.Generic.List<Type> types = new System.Collections.Generic.List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(EtlProcess).IsAssignableFrom(type) && typesInCurrentModule.Contains(type.Name))
                    types.Add(type);
            }

            Guard.Against(types.Count > 1, "Found more than a single EtlProcess type in this assembly");
            Guard.Against(types.Count == 0, "Could not find an EtlProcess in this assembly");

            return types[0];
        }

        /// <summary>
        /// Customise the compiler to fit the etl engine
        /// </summary>
        protected override void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, string[] urls)
        {
            compiler.Parameters.References.Add(typeof(EtlDslEngine).Assembly);
            compiler.Parameters.References.Add(typeof(EtlProcess).Assembly);
            pipeline.Insert(1, new AutoReferenceFilesCompilerStep());
            pipeline.Insert(2, new UseModuleNameAsNamespaceIfMissing());
            pipeline.Insert(3, new AutoImportCompilerStep(
                "Rhino.Etl.Core",
                "Rhino.Etl.Dsl",
                "Rhino.Etl.Dsl.Macros"));

            pipeline.InsertAfter(typeof(ExpandMacros), new CorrelateTypesToModuleName(moduleNameToContainedTypes));

            pipeline.Add(new SaveAssembly());
        }

        /// <summary>
        /// Creates the DSL facotry
        /// </summary>
        public static DslFactory Factory
        {
            get { return factory; }
        }

        private static DslFactory CreateFactory()
        {
            DslFactory dslFactory = new DslFactory();
            dslFactory.Register<EtlProcess>(new EtlDslEngine());
            return dslFactory;
        }
    }
}