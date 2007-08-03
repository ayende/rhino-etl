using System;
using System.Collections.Generic;

namespace Rhino.ETL
{
	/// <summary>
	/// This simple class will batch actions on queues, and then execute them.
	/// It is safe for multi threading, and while it takes the brute force approach
	/// for threading, it is easy to see that it is correct
	/// </summary>
	public class Queue
	{
		private List<Row> rows = new List<Row>();
		private bool completed = false;
		private readonly string name;
		private readonly int batchSize;
		private readonly PipeLineStage pipeLineStage;
		private int currentlyProcessing = 0;

		public Queue(
			string name, int batchSize,
			PipeLineStage pipeLineStage)
		{
			this.name = name;
			this.batchSize = batchSize;
			this.pipeLineStage = pipeLineStage;
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
			rows = new List<Row>();
			currentlyProcessing += 1;
			ExecutionPackage.Current.RegisterForExecution(delegate
			{
				// inside here we are OUTSIDE the lock!
				try
				{
					pipeLineStage.Process(copy);
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
					pipeLineStage.Complete(name);
				}
			}
		}

		public void Complete()
		{
			lock (this)
			{
				ExecuteBatch();
				if (currentlyProcessing == 0)
				{
					pipeLineStage.Complete(name);
				}
				completed = true;
			}
		}
	}
}