namespace Rhino.ETL
{
	using System.Data;
	using System.Reflection;
	using System.Transactions;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipelines;
	using FileHelpers;
	using Impl;

	public class CompileToRhinoEtl : CompileToFile
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
															"Rhino.ETL.WebServices",
		                                                  	"Rhino.ETL.FileHelpersExtensions",
		                                                  	"System.Transactions",
		                                                  	"Rhino.ETL.Engine"
		                                                  };


		protected override void Prepare(Boo.Lang.Compiler.CompilerContext context)
		{
			base.Prepare(context);
			context.Parameters.Ducky = true;
			context.Parameters.OutputType = CompilerOutputType.Library;
			context.Parameters.Debug = true;
			context.Parameters.References.Add(Assembly.GetExecutingAssembly());
			context.Parameters.References.Add(typeof(DbType).Assembly);
			context.Parameters.References.Add(typeof(TransactionScope).Assembly);
			context.Parameters.References.Add(typeof(CsvEngine).Assembly);
			Insert(2, new AutoReferenceFilesAndAddToContextCompilerStep());
			Insert(3, new TransformModuleToContextClass(defaultImports));
			Insert(11, new TransfromGeneratorExpressionToBlocks());

		}
	}
}