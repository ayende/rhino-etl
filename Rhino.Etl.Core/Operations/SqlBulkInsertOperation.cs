namespace Rhino.Etl.Core.Operations
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using DataReaders;
	using Rhino.Commons;

	/// <summary>
	/// Allows to execute an operation that perform a bulk insert into a sql server database
	/// </summary>
	public abstract class SqlBulkInsertOperation : AbstractDatabaseOperation
	{
		/// <summary>
		/// The schema of the destination table
		/// </summary>
		protected readonly IDictionary<string, Type> Schema = new Dictionary<string, Type>();

		/// <summary>
		/// The mapping of columns from the row to the database schema.
		/// Important: The column name in the database is case sensitive!
		/// </summary>
		protected IDictionary<string, string> Mappings = new Dictionary<string, string>();
		private SqlBulkCopy sqlBulkCopy;
		private readonly string targetTable;
		private readonly int timeout;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlBulkInsertOperation"/> class.
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string.</param>
		/// <param name="targetTable">The target table.</param>
		protected SqlBulkInsertOperation(string connectionStringName, string targetTable)
			: this(connectionStringName, targetTable, 600)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlBulkInsertOperation"/> class.
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string.</param>
		/// <param name="targetTable">The target table.</param>
		/// <param name="timeout">The timeout.</param>
		protected SqlBulkInsertOperation(string connectionStringName, string targetTable, int timeout)
			: base(connectionStringName)
		{
			Guard.Against(string.IsNullOrEmpty(targetTable), "TargetTable was not set, but it is mandatory");
			this.targetTable = targetTable;
			this.timeout = timeout;
		}

		/// <summary>
		/// Prepares the mapping for use, by default, it uses the schema mapping.
		/// This is the preferred appraoch
		/// </summary>
		public virtual void PrepareMapping()
		{
			foreach (KeyValuePair<string, Type> pair in Schema)
			{
				Mappings[pair.Key] = pair.Key;
			}
		}

		/// <summary>
		/// Executes this operation
		/// </summary>
		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			Guard.Against<ArgumentException>(rows == null, "SqlBulkInsertOperation cannot accept a null enumerator");
			PrepareSchema();
			PrepareMapping();
			using (SqlConnection connection = (SqlConnection)Use.Connection(ConnectionStringName))
			using (SqlTransaction transaction = connection.BeginTransaction())
			{
				sqlBulkCopy = CreateSqlBulkCopy(connection, transaction);
				DictionaryEnumeratorDataReader adapter = new DictionaryEnumeratorDataReader(Schema, rows);
				sqlBulkCopy.WriteToServer(adapter);
				
				if (PipelineExecuter.HasErrors)
				{
					Warn("Rolling back transaction in {0}", Name);
					transaction.Rollback();
					Warn("Rolled back transaction in {0}", Name);
				}
				else
				{
					Debug("Committing {0}", Name);
					transaction.Commit();
					Debug("Committed {0}", Name);
				}
			}
			yield break;
		}

		/// <summary>
		/// Prepares the schema of the target table
		/// </summary>
		protected abstract void PrepareSchema();

		/// <summary>
		/// Creates the SQL bulk copy instance
		/// </summary>
		private SqlBulkCopy CreateSqlBulkCopy(SqlConnection connection, SqlTransaction transaction)
		{
			SqlBulkCopy copy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
			foreach (KeyValuePair<string, string> pair in Mappings)
			{
				copy.ColumnMappings.Add(pair.Key, pair.Value);
			}
			copy.DestinationTableName = targetTable;
			copy.BulkCopyTimeout = timeout;
			return copy;
		}
	}
}