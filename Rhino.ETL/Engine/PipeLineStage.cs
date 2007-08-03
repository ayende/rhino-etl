using System;
using System.Collections;
using System.Collections.Generic;

namespace Rhino.ETL
{
	public class PipeLineStage
	{
		private readonly Pipeline pipeline;
		private readonly string incoming;
		private readonly IOutput output;
		private readonly string outgoing;
		private readonly IDictionary parameters;
		private int batchSize;

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

		public void Process(ICollection<Row> rows)
		{
			using (pipeline.EnterContext())
			{
				foreach (Row row in rows)
				{
					Output.Process(Outgoing, row, Parameters);
				}
			}
		}

		public void Complete(string name)
		{
			using (pipeline.EnterContext())
			{
				if (Incoming.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					Output.Complete(Outgoing);
			}
		}
	}
}