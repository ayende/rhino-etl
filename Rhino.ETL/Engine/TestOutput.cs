using System;
using System.Collections;
using System.Collections.Generic;

namespace Rhino.ETL
{
	public class TestOutput : IOutput
	{
		private Dictionary<string, bool> completed = new Dictionary<string, bool>();
		private Dictionary<string,Action<Row>> actions = new Dictionary<string, Action<Row>>();
		public string Name
		{
			get { return "test output"; }
		}

		public void Process(string queueName, Row row, IDictionary parameters)
		{
			actions[queueName](row);
		}

		public void Complete(string queueName)
		{
			completed[queueName] = true;
		}

		public bool IsComplete(string queueName)
		{
			return completed[queueName];
		}

		public void OnProcess(string queueName, Action<Row> action)
		{
			actions[queueName] = action;
		}

		public static TestPipeLineStage GetPipelineStage()
		{
			return new TestPipeLineStage("Output", new TestOutput(), "Output", 1, new Hashtable());
		}
	}
}