using System;
using System.Collections.Generic;
using System.Threading;
using Rhino.Commons;
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
							ExecutionResult result = new ExecutionResult(ExecutionStatus.Failure);
							InvalidTargetException exception = 
								new InvalidTargetException("Could not find target '" + targetName + "'");
							result.Exceptions.Add(exception);
							return result;
						}
						target.Prepare();
						target.Run();
						target.WaitForCompletion();
						return target.GetExecutionResult();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Fatal("Error executing target '" + targetName + "'", e);
				ExecutionResult result = new ExecutionResult(ExecutionStatus.InvalidPackage);
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

		public virtual void RegisterForExecution(Target target, Command action)
		{
			if (target.IsFaulted)
			{
				Logger.WarnFormat("Ignoring request to execute {0} because target {1} has faulted",
				                  DelegateToString(action),
				                  target.Name
					);
				return;
			}
			RhinoThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					using (EnterContext())
					using (configurationContext.EnterContext())
					{
						action();
					}
				}
				catch (Exception e)
				{
					Logger.Error("Error occured when executing " + DelegateToString(action) + " on target " + target.Name, e);
					target.AddFault(e);
					//note that this will also abort the currently executing thread!
					RhinoThreadPool.CancelAll(true);
				}
			});
		}

		private static string DelegateToString(Command action)
		{
			return action.Method.DeclaringType + "." + action.Method;
		}
	}
}