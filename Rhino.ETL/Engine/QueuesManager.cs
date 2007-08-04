using System;
using System.Collections.Generic;
using log4net;

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
		Dictionary<QueueKey, List<Queue>> queueToOutputs = new Dictionary<QueueKey, List<Queue>>();
		Dictionary<QueueKey, List<Row>> savedQueues = new Dictionary<QueueKey, List<Row>>();
		Dictionary<QueueKey, bool> completedQueues = new Dictionary<QueueKey, bool>();
		Dictionary<Pipeline, List<string>> outputQueuesNames = new Dictionary<Pipeline, List<string>>();

		public QueuesManager(string name, ILog logger)
		{
	        this.name = name;
		    this.logger = logger;
		}

		// Note: We assume registration is done before we start to actually run
        // so we don't bother with thread safety here.
		public void RegisterForwarding(PipeLineStage pipeLineStage)
	    {
            if(queueToOutputs.ContainsKey(pipeLineStage.IncomingKey)==false)
				queueToOutputs.Add(pipeLineStage.IncomingKey, new List<Queue>());
        	Queue queue = new Queue(
				pipeLineStage.Incoming, 
				pipeLineStage.BatchSize,
				pipeLineStage);
			queueToOutputs[pipeLineStage.IncomingKey].Add(queue);
            logger.DebugFormat("{0}.{1} registered for {1}.{2}", pipeLineStage.Output.Name, pipeLineStage.Outgoing, name, pipeLineStage.Incoming);
	    }

		public void Forward(QueueKey key, Row row)
	    {
	        List<Queue> destinations;
			if (queueToOutputs.TryGetValue(key, out destinations) == false)
            {
				logger.DebugFormat("No listeners to forward to in queue {0}", key.Name);
                return;
            }
			EnsureListedInOutputQueues(key);
			//We send copies of the row to additional outputs, to prevent
            //a case where both write to the same row
	        for (int i = 1; i < destinations.Count; i++)
	        {
                destinations[i].Enqueue(row.Clone());	            
	        }
	        destinations[0].Enqueue(row);
	    }

		private void EnsureListedInOutputQueues(QueueKey key)
		{
			if(outputQueuesNames.ContainsKey(key.Pipeline)==false)
			{
				lock(outputQueuesNames)
				{
					if (outputQueuesNames.ContainsKey(key.Pipeline) == false)
						outputQueuesNames.Add(key.Pipeline,new List<string>());
				}
			}
			List<string> list = outputQueuesNames[key.Pipeline];
			if(list.Contains(key.Name)==false)
			{
				lock(outputQueuesNames)
				{
					if(list.Contains(key.Name)==false)
						list.Add(key.Name);
				}
			}
		}

		public void Complete(QueueKey key)
	    {
			List<Queue> destinations;
            if (queueToOutputs.TryGetValue(key, out destinations) == false)
            {
                logger.DebugFormat("No listeners to mark complete to in queue {0}", key.Name);
                return;
            }
        	foreach (Queue queue in destinations)
        	{
        		queue.Complete();
        	}
	    }

		public void Add(QueueKey key, Row row)
		{
			EnsureListedInOutputQueues(key);
			List<Row> queue = GetQueue(key);
			lock(queue)
			{
				queue.Add(row);
			}
		}

		public List<Row> GetQueue(QueueKey key)
		{
			List<Row> queue;
			if (savedQueues.TryGetValue(key, out queue) == false)
			{
				lock(this)
				{
					if (savedQueues.TryGetValue(key, out queue) == false)
					{
						savedQueues[key] = queue = new List<Row>();
					}
				}
			}
			return queue;
		}

		public void Clear(QueueKey key)
		{
			savedQueues.Remove(key);
		}

		public void SetCompleted(QueueKey key)
		{
			completedQueues[key] = true;
		}

		public bool IsCompleted(QueueKey key)
		{
			bool result;
			if (completedQueues.TryGetValue(key, out result))
				return result;
			return false;
		}

		public void CompleteAll(Pipeline pipeline)
		{
			foreach (KeyValuePair<QueueKey, List<Queue>> pair in queueToOutputs)
			{
				if(pair.Key.Pipeline == pipeline)
				{
					foreach (Queue queue in pair.Value)
					{
						queue.Complete();
					}
				}
			}
		}

		public void ForEachOutputQueue(Pipeline pipeline,Action<string> action)
		{
			List<string> list;
			if(outputQueuesNames.TryGetValue(pipeline, out list)==false)
				return;
			foreach (string queueName in list)
			{
				action(queueName);
			}
		}
	}
}