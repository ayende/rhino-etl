using System;
using System.Collections.Generic;
using System.Threading;
using Rhino.ETL.Engine;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public class ExecutionPackage : ContextfulObjectBase<ExecutionPackage>
	{
		private EtlConfigurationContext configurationContext;
		private List<Command> onPipelineCompletedCommands = new List<Command>();

		public event EventHandler PipelineCompleted = delegate { };

		public List<Command> OnPipelineCompletedCommands
		{
			get { return onPipelineCompletedCommands; }
		}

		public EtlConfigurationContext ConfigurationContext
		{
			get { return configurationContext; }
		}

		protected ExecutionPackage()
		{
		}

		public ExecutionPackage(EtlConfigurationContext configurationContext)
		{
			this.configurationContext = configurationContext;
		}

		public override string Name
		{
			get { return "Execution Package"; }
		}

		public void Execute(string targetName)
		{
			using (EnterContext())
			{
				using (configurationContext.EnterContext())
				{
					Target target;
					if (configurationContext.Targets.TryGetValue(targetName, out target) == false)
						throw new InvalidTargetException("Could not find target '" + targetName + "'");
					target.Prepare();
					target.Run();
					target.WaitForCompletion();
				}
			}
		}

		public void Pipeline_Completed(Pipeline completed)
		{
			Logger.InfoFormat("Pipeline {0} completed.", completed.Name);
			List<Command> copy;
			lock (onPipelineCompletedCommands)
			{
				copy = onPipelineCompletedCommands;
				onPipelineCompletedCommands = new List<Command>();
			}
			foreach (Command command in copy)
			{
				command();
			}
		}

		public void ExecuteOnPipelineCompleted(Command block)
		{
			lock (onPipelineCompletedCommands)
			{
				onPipelineCompletedCommands.Add(block);
			}
		}

		public virtual void RegisterForExecution(Command action)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				using (EnterContext())
				using (configurationContext.EnterContext())
				{
					action();
				}
			});

			
		}
	}
}