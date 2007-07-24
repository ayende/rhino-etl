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
		private const string DefaultColumnName = "Output";
		private Dictionary<string, List<Action<Row>>> actionsOnQueue = new Dictionary<string, List<Action<Row>>>();
		private Dictionary<string, List<Command>> completeOnQueue = new Dictionary<string, List<Command>>();

		private ILog logger;

		public QueuesManager(ILog logger)
		{
			this.logger = logger;
		}

		public void RegisterAction(string queueName, Action<Row> action, Command onComplete)
		{
			queueName = queueName ?? DefaultColumnName;
			if (actionsOnQueue.ContainsKey(queueName) == false)
			{
				actionsOnQueue[queueName] = new List<Action<Row>>();
				completeOnQueue[queueName] = new List<Command>();
			}
			actionsOnQueue[queueName].Add(action);
			completeOnQueue[queueName].Add(onComplete);
		}

		public void PushInto(string queueName, Row row)
		{
			queueName = queueName ?? DefaultColumnName;
			if (actionsOnQueue.ContainsKey(queueName) == false)
			{
				logger.InfoFormat("Got row for queue {0} that has no registered actions", queueName);
				return;
			}
			List<Action<Row>> actions = actionsOnQueue[queueName];
			//send copies to all actions except the first, which gets the original row
			//doing it this way to support safe multi threading
			for (int i = 1; i < actions.Count; i++)
			{
				Action<Row> action = actions[i];
				Row cloned = row.Clone();
				ExecutionPackage.Current.RegisterForExecution(delegate
				{
					action(cloned);
				});
			}
			Action<Row> firstAction = actions[0];
			ExecutionPackage.Current.RegisterForExecution(delegate
			{
				firstAction(row);
			});
		}

		public void Complete(string queueName)
		{
			queueName = queueName ?? "Output";
			if (completeOnQueue.ContainsKey(queueName) == false)
			{
				logger.InfoFormat("Queue {0} completed, but had no registered actions", queueName);
				return;
			}
			List<Command> list = completeOnQueue[queueName];
			foreach (Command command in list)
			{
				ExecutionPackage.Current.RegisterForExecution(command);
			}
		}
	}
}