using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Boo.Lang;

namespace Rhino.ETL
{
	using Engine;

	public class DataDestination : BaseDataElement<DataDestination>, IOutput
	{
		private int batchSize = 500;
		private List<Row> rows = new List<Row>();
		private bool hasCompleted = false;
		private bool firstCall = true;
		private ICallable initializeBlock, onRowBlock, cleanUpBlock;

		[Browsable(false)]
		public ICallable InitializeBlock
		{
			get { return initializeBlock; }
			set { initializeBlock = value; }
		}

		[Browsable(false)]
		public ICallable OnRowBlock
		{
			get { return onRowBlock; }
			set { onRowBlock = value; }
		}
		
		[Browsable(false)]
		public ICallable CleanUpBlock
		{
			get { return cleanUpBlock; }
			set { cleanUpBlock = value; }
		}

		protected override bool CustomActionSpecified
		{
			get { return onRowBlock != null; }
		}

		public event OutputCompleted Completed = delegate { };

		public DataDestination(string name)
			: base(name)
		{
			EtlConfigurationContext.Current.AddDestination(name, this);
		}

		[ReadOnly(true)]
		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		public void Process(QueueKey key, Row row, IDictionary ignored)
		{
			lock (rows)
			{
				if (firstCall && initializeBlock != null)
				{
					lock (onRowBlock)
					{
						initializeBlock.Call(new object[] { Items });
					}
				}
				firstCall = false;
				if (hasCompleted)
					throw new InvalidOperationException("Cannot process rows to a destination after it has been marked complete");
				rows.Add(row);
				if (rows.Count >= BatchSize)
				{
					ProcessOutput(key.Pipeline);
				}
			}
		}

		public void Complete(QueueKey key)
		{
			lock (rows)
			{
				hasCompleted = true;
			}
			ProcessOutput(key.Pipeline); //flush any additional output
			if (onRowBlock != null)
			{
				lock (onRowBlock)
				{
					if (cleanUpBlock != null)
						cleanUpBlock.Call(new object[] { Items });
				}
			}
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
			if (CustomActionSpecified == false)
			{
				SendToDatabase(copyRows, pipeline);
			}
			else
			{
				lock (onRowBlock)
				{
					foreach (Row copyRow in copyRows)
					{
						onRowBlock.Call(new object[] { Items, copyRow });
					}
				}
			}
		}

		private void SendToDatabase(IEnumerable<Row> copyRows, Pipeline pipeline)
		{
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