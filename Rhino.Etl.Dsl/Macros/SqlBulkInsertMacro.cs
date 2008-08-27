namespace Rhino.Etl.Dsl.Macros
{
	using Rhino.Etl.Core.ConventionOperations;
	using Rhino.Etl.Core.Operations;

	/// <summary>
	/// Create a new <see cref="ConventionSqlBulkInsertOperation"/> and instantiate it in place of the 
	/// macro
	/// </summary>
	public class SqlBulkInsertMacro : AbstractClassGeneratorMacro<ConventionSqlBulkInsertOperation>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlBulkInsertMacro"/> class.
		/// </summary>
		public SqlBulkInsertMacro()
			: base("PrepareSchema")
		{

		}
	}
}