using System;
using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Core.ConventionOperations
{
	/// <summary>Convertion wrapper around the <see cref="SqlBulkInsertOperation"/>.</summary>
	public class ConventionSqlBulkInsertOperation : SqlBulkInsertOperation
	{
		/// <summary>Creates a new <see cref="ConventionSqlBulkInsertOperation"/></summary>
		/// <param name="connectionStringName"></param>
		/// <param name="targetTable"></param>
		public ConventionSqlBulkInsertOperation(string connectionStringName, string targetTable) : base(connectionStringName, targetTable)
		{
		}

		/// <summary>
		/// Prepares the schema of the target table
		/// </summary>
		protected override void PrepareSchema()
		{
		}

		/// <summary>
		/// Prepares the mapping for use, by default, it uses the schema mapping.
		/// This is the preferred appraoch
		/// </summary>
		public override void PrepareMapping()
		{
		}

		/// <summary>Adds a column to the destination mapping.</summary>
		/// <param name="destinationColumn">The name of column, this is case sensitive.</param>
		public virtual void MapColumn(string destinationColumn)
		{
			MapColumn(destinationColumn, typeof (string));
		}

		/// <summary>Adds a column and specified type to the destination mapping.</summary>
		/// <param name="destinationColumn">The name of the column</param>
		/// <param name="columnType">The type of the column.</param>
		public virtual void MapColumn(string destinationColumn, Type columnType)
		{
			MapColumn(destinationColumn, destinationColumn, columnType);
		}

		/// <summary>Adds a column and the specified type to the destination mapping with the given sourceColumn mapping.</summary>
		/// <param name="destinationColumn"></param>
		/// <param name="columnType"></param>
		/// <param name="sourceColumn"></param>
		public virtual void MapColumn(string destinationColumn, string sourceColumn, Type columnType)
		{
			Schema[destinationColumn] = columnType;
			Mappings[sourceColumn] = destinationColumn;
		}
	}
}