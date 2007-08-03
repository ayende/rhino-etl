using System;
using System.IO;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using log4net;
using Rhino.ETL.Exceptions;
using Rhino.ETL.Impl;

namespace Rhino.ETL
{
	public class EtlContextBuilder
	{
		private static ILog logger = LogManager.GetLogger(typeof (EtlContextBuilder));

		private static readonly string[] defaultImports = {
		                                                  	"System",
		                                                  	"System.Data",
		                                                  	"System.Configuration",
		                                                  	"System.Data.SqlClient",
		                                                  	"System.Data.OracleClient",
		                                                  	"System.Data.Odbc",
		                                                  	"System.Data.OleDb",
		                                                  	"Rhino.ETL",
		                                                  	"Rhino.ETL.Impl",
		                                                  	"Rhino.ETL.Commands"
		                                                  };

		public static EtlConfigurationContext FromFile(string filename)
		{
			string rootName = Path.GetFileNameWithoutExtension(filename);
			return BuildAndConfigure(rootName, new FileInput(filename));
		}

		private static EtlConfigurationContext BuildAndConfigure(string rootName, params ICompilerInput[] inputs)
		{
			EtlConfigurationContext etlConfigurationContext;
			try
			{
				etlConfigurationContext = Compile(rootName, inputs);
			}
			catch (Exception e)
			{
				logger.Error("Failed compilation", e);
				throw;
			}
			using (etlConfigurationContext.EnterContext())
			{
				try
				{
					etlConfigurationContext.BuildConfig();
				}
				catch (Exception e)
				{
					logger.Error("Failed to evaluate EtlConfigurationContext named: " + etlConfigurationContext.Name, e);
					throw new ScriptEvalException(
						etlConfigurationContext.Name + " has thrown an exception when evaluated: " + e.Message, e);
				}
			}
			return etlConfigurationContext;
		}

		private static EtlConfigurationContext Compile(string rootName, params ICompilerInput[] inputs)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Ducky = true;
			compiler.Parameters.Pipeline = new CompileToMemory();
			compiler.Parameters.Pipeline = new CompileToFile();
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			foreach (ICompilerInput compilerInput in inputs)
			{
				compiler.Parameters.Input.Add(compilerInput);
			}
			compiler.Parameters.References.Add(Assembly.GetExecutingAssembly());
			compiler.Parameters.Pipeline.Insert(2, new TransformModuleToContextClass(defaultImports));
			compiler.Parameters.Pipeline.Insert(10, new TransfromGeneratorExpressionToBlocks());
			CompilerContext run = compiler.Run();
			if (run.Errors.Count != 0)
			{
				throw new CompilerError(string.Format("Compilation error! {0}", run.Errors.ToString(true)));
			}
				Type type = run.GeneratedAssembly.GetType(rootName);
			return Activator.CreateInstance(type) as EtlConfigurationContext;
		}
	}
}