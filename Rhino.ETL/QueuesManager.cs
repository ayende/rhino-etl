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
        private class Destination
        {
            public IOutput Output;
            public IDictionary Parameters;
            public string QueueName;

            public void Process(Row row)
            {
                Output.Process(QueueName, row, Parameters);
            }
        }

	    private string name;
		private ILog logger;
        Dictionary<string, List<Destination>> queueToOutputs = new Dictionary<string, List<Destination>>();
	    public QueuesManager(string name, ILog logger)
		{
	        this.name = name;
		    this.logger = logger;
		}

        // Note: We assume registration is done before we start to actually run
        // so we don't bother with thread safety here.
        public void ForwardTo(string inQueue, IOutput output, string outQueue, IDictionary parameters)
	    {
            if(queueToOutputs.ContainsKey(inQueue)==false)
                queueToOutputs.Add(inQueue, new List<Destination>());
	        Destination destination = new Destination();
	        destination.Output = output;
	        destination.QueueName = outQueue;
            destination.Parameters = parameters;
	        queueToOutputs[inQueue].Add(destination);
            logger.DebugFormat("{0}.{1} registered for {1}.{2}", output.Name, outQueue, name, inQueue);
	    }

	    public void Forward(string queueName, Row row)
	    {
	        List<Destination> destinations;
            if (queueToOutputs.TryGetValue(queueName, out destinations) == false)
            {
                logger.DebugFormat("No queue to forward to in queue {0}", queueName);
                return;
            }
            //We send copies of the row to additional outputs, to prevent
            //a case where both write to the same row
	        for (int i = 0; i < destinations.Count; i++)
	        {
                destinations[i].Process(row.Clone());	            
	        }
	        destinations[0].Process(row);
	    }

        public void Complete(string queueName)
	    {
	    }
	}
}