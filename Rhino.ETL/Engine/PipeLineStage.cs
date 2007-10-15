using System;
using System.Collections;
using System.Collections.Generic;

namespace Rhino.ETL
{
	using Engine;

	public class PipeLineStage
	{
		private readonly Pipeline pipeline;
		private readonly string incoming;
		private readonly IOutput output;
		private readonly string outgoing;
		private readonly IDictionary parameters;
		private int batchSize;
		private QueueKey incomingKey;

		public PipeLineStage(Pipeline pipeline, string incoming, IOutput output,
							 string outgoing, int batchSize, IDictionary parameters)
		{
			this.pipeline = pipeline;
			this.incoming = incoming;
			this.output = output;
			this.outgoing = outgoing;
			this.batchSize = batchSize;
			this.parameters = parameters;
		}

		public string Incoming
		{
			get { return incoming; }
		}

		public IOutput Output
		{
			get { return output; }
		}

		public string Outgoing
		{
			get { return outgoing; }
		}

		public IDictionary Parameters
		{
			get { return parameters; }
		}

		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		public QueueKey IncomingKey
		{
			get
			{
				if (incomingKey == null)
					incomingKey = new QueueKey(incoming, pipeline);
				return incomingKey;
			}
		}

		public void Process(ICollection<Row> rows)
		{
			QueueKey key = new QueueKey(Outgoing, pipeline);
			using (pipeline.EnterContext())
			{
				foreach (Row row in rows)
				{
					Output.Process(key, row, Parameters);
				}
			}
		}

		public void Complete(string name)
		{
			QueueKey key = new QueueKey(Outgoing, pipeline);
			using (pipeline.EnterContext())
			{
				if (Incoming.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					Output.Complete(key);
			}
		}
	}
}