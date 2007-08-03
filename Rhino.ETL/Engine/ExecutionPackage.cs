using System;
using System.Collections.Generic;
using System.Threading;

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

		public void Execute()
		{
			using (EnterContext())
			{
				using (configurationContext.EnterContext())
				{
					foreach (Pipeline pipeline in configurationContext.Pipelines.Values)
					{
						ExecuteSinglePipeline(pipeline);
					}
				}
			}
			WaitForAllPipelines();
			ClearAllContexts();
		}

		private void ClearAllContexts()
		{
			ExecutionPackage.Current = null;
			EtlConfigurationContext.Current = null;
			Transform.Current = null;
			Join.Current = null;
			Pipeline.Current = null;
			DataSource.Current = null;
			DataDestination.Current = null;
		}

		public void Execute(Pipeline pipeline)
		{
			using (EnterContext())
			{
				using (configurationContext.EnterContext())
				{
					ExecuteSinglePipeline(pipeline);
				}
			}
			pipeline.WaitOne();
		}


		private void ExecuteSinglePipeline(Pipeline pipeline)
		{
			Logger.InfoFormat("Starting pipeline {0}", pipeline.Name);
			pipeline.Completed += Pipeline_Completed;
			pipeline.Prepare();
			Pipeline copy = pipeline;
			RegisterForExecution(copy.Start);
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