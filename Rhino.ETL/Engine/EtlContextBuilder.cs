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

	public class EtlContextBuilder
	{
		
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
			compiler.Parameters.Pipeline = new CompileToRhinoEtl();
			foreach (ICompilerInput compilerInput in inputs)
			{
				compiler.Parameters.Input.Add(compilerInput);
			}
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