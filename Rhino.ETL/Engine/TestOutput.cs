using System;
using System.Collections;
using System.Collections.Generic;

namespace Rhino.ETL
{
	using Engine;

	public class TestOutput : IOutput
	{
		private Dictionary<string, bool> completedActions = new Dictionary<string, bool>();
		private Dictionary<string, Action<Row>> actions = new Dictionary<string, Action<Row>>();
		public string Name
		{
			get { return "test output"; }
		}

		public event OutputCompleted Completed = delegate { };

		public void Process(QueueKey key, Row row, IDictionary parameters)
		{
			actions[key.Name](row);
		}

		public void Complete(QueueKey key)
		{
			completedActions[key.Name] = true;
			Completed(this, key);
		}

		public bool IsComplete(string queueName)
		{
			return completedActions[queueName];
		}

		public void OnProcess(string queueName, Action<Row> action)
		{
			actions[queueName] = action;
		}

		public static TestPipeLineStage GetPipelineStage(Pipeline pipeline)
		{
			return new TestPipeLineStage(pipeline, "Output", new TestOutput(), "Output", 1, new Hashtable());
		}
	}
}