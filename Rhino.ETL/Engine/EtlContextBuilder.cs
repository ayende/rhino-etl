namespace Rhino.ETL
{
	using System;
	using System.Data;
	using System.IO;
	using System.Reflection;
	using System.Transactions;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Pipelines;
	using Engine;
	using Exceptions;
	using FileHelpers;
	using Impl;
	using log4net;
	using Rhino.Commons.Boo;

	public class EtlContextBuilder
	{
		private static readonly string[] defaultImports = {
		                                                  	"System",
		                                                  	"System.Configuration",
		                                                  	"System.Data.SqlClient",
		                                                  	"System.Data.OracleClient",
		                                                  	"System.Data.Odbc",
		                                                  	"System.Data.OleDb",
		                                                  	"Rhino.ETL",
		                                                  	"FileHelpers",
		                                                  	"Rhino.ETL.Impl",
		                                                  	"Rhino.ETL.Commands",
		                                                  	"Rhino.ETL.FileHelpersExtensions",
		                                                  	"System.Transactions",
		                                                  	"Rhino.ETL.Engine"
		                                                  };

		private static readonly ILog logger = LogManager.GetLogger(typeof (EtlContextBuilder));

		public static EtlConfigurationContext FromFile(string filename)
		{
			string rootName = Path.GetFileNameWithoutExtension(filename);
			string rootDir = Path.GetDirectoryName(filename);
			return From(rootDir, rootName, new FileInput(filename));
		}

		public static EtlConfigurationContext From(string rootDir, string rootName, params ICompilerInput[] inputs)
		{
			EtlConfigurationContext etlConfigurationContext;
			try
			{
				etlConfigurationContext = Compile(rootDir, rootName, inputs);
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

		private static EtlConfigurationContext Compile(string rootDir, string rootName, params ICompilerInput[] inputs)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Ducky = true;
			compiler.Parameters.Pipeline = new CompileToFile();
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			foreach (ICompilerInput compilerInput in inputs)
			{
				compiler.Parameters.Input.Add(compilerInput);
			}
			compiler.Parameters.References.Add(Assembly.GetExecutingAssembly());
			compiler.Parameters.References.Add(typeof (DbType).Assembly);
			compiler.Parameters.References.Add(typeof (TransactionScope).Assembly);
			compiler.Parameters.References.Add(typeof (CsvEngine).Assembly);
			compiler.Parameters.Pipeline.Insert(2, new AutoReferenceFilesCompilerStep(rootDir));
			compiler.Parameters.Pipeline.Insert(3, new TransformModuleToContextClass(defaultImports, rootName));
			compiler.Parameters.Pipeline.Insert(11, new TransfromGeneratorExpressionToBlocks());
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