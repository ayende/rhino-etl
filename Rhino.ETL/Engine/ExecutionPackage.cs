using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rhino.Commons;
using Rhino.ETL.Engine;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Engine
{
	using Retlang;

	public class ExecutionPackage : ContextfulObjectBase<ExecutionPackage>
	{
		private readonly EtlConfigurationContext configurationContext;
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

		public ExecutionPackage()
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

		public ExecutionResult Execute(string targetName)
		{
			try
			{
				using (EnterContext())
				{
					using (configurationContext.EnterContext())
					{
						Target target;
						if (configurationContext.Targets.TryGetValue(targetName, out target) == false)
						{
						    ExecutionResult result = new ExecutionResult(ExecutionStatus.Failure, configurationContext.Errors);
						    InvalidTargetException exception =
						        new InvalidTargetException("Could not find target '" + targetName + "'");
						    result.Exceptions.Add(exception);
						    return result;
						}
						target.Prepare();
						IProcessContextFactory contextFactory = new DebugProcessContextFactory();
						contextFactory.Start();
						target.Run(contextFactory);
						target.WaitForCompletion();
						contextFactory.Stop();
						return target.GetExecutionResult(configurationContext);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Fatal("Error executing target '" + targetName + "'", e);
				ExecutionResult result =  new ExecutionResult(ExecutionStatus.InvalidPackage, configurationContext.Errors);
				result.Exceptions.Add(e);
				return result;
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
	}
}