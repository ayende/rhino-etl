using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Boo.Lang;
using Boo.Lang.Compiler.MetaProgramming;
using Rhino.ETL.Engine;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public class DataSource : BaseDataElement<DataSource>, IInput
	{
		private readonly QueuesManager queueManager;
		private readonly string currentQueueKey = "Current.Queue.Key";
		private const string OutputQueueName = "Output";
		protected ICallable blockToExecute;

		public DataSource(string name)
			: base(name)
		{
			queueManager = new QueuesManager(name, Logger);
			EtlConfigurationContext.Current.AddSource(name, this);
		}

		public void RegisterForwarding(Target target, PipeLineStage pipeLineStage)
		{
			queueManager.RegisterForwarding(target, pipeLineStage);
		}

		public void Start(Pipeline pipeline)
		{
			QueueKey key = new QueueKey(OutputQueueName, pipeline);
			Items[CurrentQueueKey] = key;
			if (blockToExecute != null)
			{
				using (EnterContext())
					blockToExecute.Call(new object[] {this});
			}
			else
			{
				ReadFromDatabase(key, pipeline);
			}
			queueManager.Complete(key);
		}

		/// <summary>
		/// Custom behaviors can be had by overriding it using a block
		/// in the source element in the DSL
		/// </summary>
		protected virtual void ReadData(QueueKey key, Pipeline pipeline)
		{
			ReadFromDatabase(key, pipeline);
		}

		private void ReadFromDatabase(QueueKey key, Pipeline pipeline)
		{
			using (IDbCommand command = GetDbConnection(pipeline).CreateCommand())
			{
				command.CommandText = Command;
				AddParameters(command);

				using (IDataReader reader = command.ExecuteReader())
				{
					DataTable schema = reader.GetSchemaTable();
					List<string> columns = new List<string>();
					foreach (DataRow schemaRow in schema.Rows)
					{
						columns.Add((string) schemaRow["ColumnName"]);
					}
					while (reader.Read())
					{
						Row row = new Row();
						for (int i = 0; i < columns.Count; i++)
						{
							object value = reader.GetValue(i);
							if (value == DBNull.Value)
								value = null;
							row[columns[i]] = value;
						}
						SendRow(key, row);
					}
				}
			}
		}

		public void SendRow(QueueKey key, Row row)
		{
			queueManager.Forward(key, row);
		}

		public void SendRow(Row row)
		{
			queueManager.Forward((QueueKey) Items[CurrentQueueKey], row);
		}

		public string CurrentQueueKey
		{
			get { return currentQueueKey; }
		}

		[Meta]
		public void Execute(ICallable block)
		{
			blockToExecute = block;
		}

		[Meta]
		public void execute(ICallable block)
		{
			Execute(block);
		}


		protected override bool CustomActionSpecified
		{
			get { return blockToExecute != null; }
		}
	}
}