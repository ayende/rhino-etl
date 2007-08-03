using System.Collections;
using System.Collections.Generic;

namespace Rhino.ETL
{
	public class PipeLineStage
	{
		private readonly string incoming;
		private readonly IOutput output;
		private readonly string outgoing;
		private readonly IDictionary parameters;
		private int batchSize;

		public PipeLineStage(string incoming, IOutput output, 
			string outgoing, int batchSize, IDictionary parameters)
		{
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
			foreach (Row row in rows)
			{
				Output.Process(Outgoing, row, Parameters);
			}
		}
	}
}