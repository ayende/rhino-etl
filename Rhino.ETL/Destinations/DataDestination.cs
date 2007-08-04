using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Rhino.ETL
{
	public class DataDestination : BaseDataElement<DataDestination>, IOutput
	{
		private int batchSize = 500;
		private List<Row> rows = new List<Row>();
		private bool hasCompleted = false;

		public event OutputCompleted Completed = delegate { };

		public DataDestination(string name) : base(name)
		{
			EtlConfigurationContext.Current.AddDestination(name, this);
		}

		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		public void Process(QueueKey key, Row row, IDictionary ignored)
		{
			lock (rows)
			{
				if (hasCompleted)
					throw new InvalidOperationException("Cannot process rows to a destination after it has been marked complete");
				rows.Add(row);
				if (rows.Count >= BatchSize)
				{
					ExecutionPackage.Current.RegisterForExecution(delegate
					{
						ProcessOutput(key.Pipeline);
					});
				}
			}
		}

		public void Complete(QueueKey key)
		{
			lock(rows)
			{
				hasCompleted = true;
			}
			ProcessOutput(key.Pipeline);//flush any additional output
			Completed(this, key);
		}

		public void ProcessOutput(Pipeline pipeline)
		{
			List<Row> copyRows;
			lock (rows)
			{
				copyRows = rows;
				rows = new List<Row>();
			}

			foreach (Row row in copyRows)
			{
				using (IDbCommand command = GetDbConnection(pipeline).CreateCommand())
				{
					command.CommandText = Command;
					AddParameters(command);
					foreach (string name in row.Columns)
					{
						IDbDataParameter parameter = command.CreateParameter();
						parameter.ParameterName = name;
						object value = row[name] ?? DBNull.Value;
						parameter.Value = value;
						command.Parameters.Add(parameter);
					}
					command.ExecuteNonQuery();
				}
			}
		}
	}
}