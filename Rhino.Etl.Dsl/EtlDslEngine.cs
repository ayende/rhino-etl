using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Steps;
using Rhino.DSL;

namespace Rhino.Etl.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using CompilerSteps;
    using Core;

    /// <summary>
    /// The Etl DSL engine
    /// </summary>
    public class EtlDslEngine : DslEngine
    {
        private static readonly DslFactory factory = CreateFactory();
        private readonly IDictionary<string, IList<string>> moduleNameToContainedTypes = new Dictionary<string, IList<string>>();
        private readonly string[] _namespaces;

        /// <summary>
        /// Create the ETL DSL engine
        /// </summary>
        public EtlDslEngine() :  this(new string[0])
        { }

        /// <summary>
        /// Create the ETL DSL engine, registering additional namespaces with the compiler pipeline
        /// </summary>
        /// <param name="additionalNamespaces">Additional namespaces to register</param>
        public EtlDslEngine(IEnumerable<string> additionalNamespaces)
        {
            var namespaces = new List<string> {"Rhino.Etl.Core", "Rhino.Etl.Dsl", "Rhino.Etl.Dsl.Macros"};
            namespaces.AddRange(additionalNamespaces);
            _namespaces = namespaces.ToArray();
        }

        /// <summary>
        /// Compile the DSL and return the resulting context
        /// </summary>
        /// <param name="urls">The files to compile</param>
        /// <returns>The resulting compiler context</returns>
        public override CompilerContext Compile(params string[] urls)
        {
            //disable caching, we always compile from scratch
            return ForceCompile(urls, GetFileName(urls));
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns></returns>
        private static string GetFileName(IEnumerable<string> urls)
        {
            string file = Path.GetTempFileName();
            foreach (string url in urls)
            {
                file = url;
                break;
            }
            return Path.GetFileNameWithoutExtension(file) + ".dll";
        }

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
            var types = new List<Type>();
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
            pipeline.Insert(3, new AutoImportCompilerStep(_namespaces));

            pipeline.InsertAfter(typeof(MacroAndAttributeExpansion), 
                new CorrelateTypesToModuleName(moduleNameToContainedTypes));
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