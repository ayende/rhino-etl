using System;
using System.Collections.Generic;
using System.Threading;

namespace Rhino.ETL
{
	public class ExecutionPackage : ContextfulObjectBase<ExecutionPackage>
	{
		private EtlConfigurationContext configurationContext;
		private List<Command> onPipelineCompletedCommands = new List<Command>();

		public event EventHandler PipelineCompleted = delegate
		{
		};

		public List<Command> OnPipelineCompletedCommands
		{
			get { return onPipelineCompletedCommands; }
		}

		public EtlConfigurationContext ConfigurationContext
		{
			get { return configurationContext; }
		}

		public ExecutionPackage(EtlConfigurationContext configurationContext)
		{
			this.configurationContext = configurationContext;
		}

		public override string Name
		{
			get { return "Execution Package"; }
		}

		public void Execute()
		{
			using (EnterContext())
			{
				using (configurationContext.EnterContext())
				{
					foreach (Pipeline pipeline in configurationContext.Pipelines.Values)
					{
						Logger.InfoFormat("Starting pipeline {0}", pipeline.Name);
						pipeline.Completed += Pipeline_Completed;
						pipeline.Prepare();
						Pipeline copy = pipeline;
						ThreadPool.QueueUserWorkItem(delegate
						{
							RegisterForExecution(copy.Start);
						});
					}
				}
			}
			WaitForAllPipelines();
		}

		private void WaitForAllPipelines()
		{
			foreach (Pipeline pipeline in configurationContext.Pipelines.Values)
			{
				pipeline.WaitOne();
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

		public void RegisterForExecution(Command action)
		{
			//ideally this would be run via a thread pool, but
			//we still have to overcome the issue of syncing the complete
			//signal with remaining data, so it is single threaded per pipeline, for now
			ThreadPool.QueueUserWorkItem(delegate
			{
				using (EnterContext())
				using (configurationContext.EnterContext())
				{
					{
						action();
					}
				}
			});
		}
	}
}