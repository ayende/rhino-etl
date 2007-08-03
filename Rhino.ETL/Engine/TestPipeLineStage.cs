using System;
using System.Collections;

namespace Rhino.ETL
{
	public class TestPipeLineStage : PipeLineStage
	{
		public TestPipeLineStage(string incoming, IOutput output, string outgoing, int batchSize, IDictionary parameters) : base(incoming, output, outgoing, batchSize, parameters)
		{
		}

		public TestOutput TestOutput
		{
			get { return (TestOutput) Output; }
		}

		public void OnProcess(string queueName, Action<Row> action)
		{
			TestOutput.OnProcess(queueName, action);
		}
	}
}