using System;
using System.Collections.Generic;
using System.Threading;

namespace Rhino.ETL
{
	public class Queue
	{
		private List<Row> rows = new List<Row>();
		private bool completed = false;
		private readonly string name;
		private readonly int batchSize;
		private readonly IOutput output;
		private readonly Action<ICollection<Row>> onBatch;
		private int currentlyProcessing = 0;

		public Queue(
			string name, int batchSize,
			IOutput output,
			Action<ICollection<Row>> onBatch)
		{
			this.name = name;
			this.batchSize = batchSize;
			this.output = output;
			this.onBatch = onBatch;
		}

		public void Enqueue(Row row)
		{
			lock (this)
			{
				if (completed)
					throw new InvalidOperationException("The queue '" + name + "' is closed.");

				rows.Add(row);
				if (rows.Count >= batchSize)
				{
					ExecuteBatch();
				}
			}
		}

		/// <summary>
		/// this must always be called under the lock
		/// </summary>
		private void ExecuteBatch()
		{
			List<Row> copy = rows;
			this.rows = new List<Row>();
			currentlyProcessing += 1;
			ExecutionPackage.Current.RegisterForExecution(delegate
			{
				try
				{
					onBatch(copy);
				}
				finally
				{
					OnFinishedProcessing();
				}
			});
		}

		private void OnFinishedProcessing()
		{
			lock (this)
			{
				currentlyProcessing -= 1;
				if (currentlyProcessing == 0 && completed)
				{
					output.Complete(name);
				}
			}
		}

		public void Complete()
		{
			lock (this)
			{
				completed = true;
				ExecuteBatch();
				if (currentlyProcessing == 0)
				{
					output.Complete(name);
				}
			}
		}
	}
}