using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Boo.Lang;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public class DataSource : BaseDataElement<DataSource>, IInput
	{
		private QueuesManager queueManager;
		const string OutputQueueName = "Output";

		public DataSource(string name)
			: base(name)
		{
			queueManager = new QueuesManager(Name, Logger);
			EtlConfigurationContext.Current.AddSource(name, this);
		}

		public void Start()
		{
			ExecutionPackage.Current
				.RegisterForExecution(ProcessInput);
		}


	    public void ForwardTo(string inQueue, IOutput output, string outQueue, IDictionary parameters)
	    {
            queueManager.ForwardTo(inQueue, output, outQueue, parameters);
	    }

	    private void ProcessInput()
		{
			using (IDbCommand command = dbConnection.CreateCommand())
			{
				command.CommandText = Command;
				AddParameters(command);

				using (IDataReader reader = command.ExecuteReader())
				{
					DataTable schema = reader.GetSchemaTable();
					List<string> columns = new List<string>();
					foreach (DataRow schemaRow in schema.Rows)
					{
						columns.Add((string)schemaRow["ColumnName"]);
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
						queueManager.Forward(OutputQueueName, row);
					}
				}
			}
			queueManager.Complete(OutputQueueName);
		}
	}
}