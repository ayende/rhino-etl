using System;
using System.Collections.Generic;
using log4net;
using System.Collections;

namespace Rhino.ETL
{
	/// <summary>
	/// Manages actionsOnQueue and dispatch actions.
	/// It is assumed that all action registration are happening before any execution,
	/// so no explicit thread safety is required.
	/// </summary>
	public class QueuesManager
	{
        private string name;
		private ILog logger;
        Dictionary<string, List<Queue>> queueToOutputs = new Dictionary<string, List<Queue>>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, List<Row>> savedQueues = new Dictionary<string, List<Row>>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, bool> completedQueues = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
		private List<string> outputQueuesNames = new List<string>();

		public QueuesManager(string name, ILog logger)
		{
	        this.name = name;
		    this.logger = logger;
		}

		// Note: We assume registration is done before we start to actually run
        // so we don't bother with thread safety here.
		public void RegisterForwarding(PipeLineStage pipeLineStage)
	    {
            if(queueToOutputs.ContainsKey(pipeLineStage.Incoming)==false)
                queueToOutputs.Add(pipeLineStage.Incoming, new List<Queue>());
        	Queue queue = new Queue(
				pipeLineStage.Incoming, 
				pipeLineStage.BatchSize,
				pipeLineStage);
        	queueToOutputs[pipeLineStage.Incoming].Add(queue);
            logger.DebugFormat("{0}.{1} registered for {1}.{2}", pipeLineStage.Output.Name, pipeLineStage.Outgoing, name, pipeLineStage.Incoming);
	    }

		public void Forward(string queueName, Row row)
	    {
	        List<Queue> destinations;
            if (queueToOutputs.TryGetValue(queueName, out destinations) == false)
            {
                logger.DebugFormat("No listeners to forward to in queue {0}", queueName);
                return;
            }
			EnsureListedInOutputQueues(queueName);
			//We send copies of the row to additional outputs, to prevent
            //a case where both write to the same row
	        for (int i = 1; i < destinations.Count; i++)
	        {
                destinations[i].Enqueue(row.Clone());	            
	        }
	        destinations[0].Enqueue(row);
	    }

		private void EnsureListedInOutputQueues(string queueName)
		{
			if(outputQueuesNames.Contains(queueName)==false)
			{
				lock(outputQueuesNames)
				{
					if(outputQueuesNames.Contains(queueName)==false)
						outputQueuesNames.Add(queueName);
				}
			}
		}

		public void Complete(string queueName)
	    {
			List<Queue> destinations;
            if (queueToOutputs.TryGetValue(queueName, out destinations) == false)
            {
                logger.DebugFormat("No listeners to mark complete to in queue {0}", queueName);
                return;
            }
        	foreach (Queue queue in destinations)
        	{
        		queue.Complete();
        	}
	    }

		public void Add(string queueName, Row row)
		{
			EnsureListedInOutputQueues(queueName);
			List<Row> queue = GetQueue(queueName);
			lock(queue)
			{
				queue.Add(row);
			}
		}

		public List<Row> GetQueue(string queueName)
		{
			List<Row> queue;
			if(savedQueues.TryGetValue(queueName, out queue)==false)
			{
				lock(this)
				{
					if(savedQueues.TryGetValue(queueName, out queue)==false)
					{
						savedQueues[queueName] = queue = new List<Row>();
					}
				}
			}
			return queue;
		}

		public void Clear(string queueName)
		{
			savedQueues.Remove(queueName);
		}

		public void SetCompleted(string queueName)
		{
			completedQueues[queueName] = true;
		}

		public bool IsCompleted(string queueName)
		{
			bool result;
			if (completedQueues.TryGetValue(queueName, out result))
				return result;
			return false;
		}

		public void CompleteAll()
		{
			foreach (List<Queue> queues in queueToOutputs.Values)
			{
				foreach (Queue queue in queues)
				{
					queue.Complete();
				}
			}
		}

		public void ForEachOutputQueue(Action<string> action)
		{
			foreach (string columnName in outputQueuesNames)
			{
				action(columnName);
			}
		}
	}
}