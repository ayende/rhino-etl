using System;
using System.Collections.Generic;
using Rhino.Commons;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	using Retlang;

	public class ExecuteInParallelCommand : AbstractCommand, ICommandContainer
	{
		protected List<ICommand> commands = new List<ICommand>();
		protected CountdownLatch latch;

		public ExecuteInParallelCommand(Target target)
			: base(target)
		{
		}

		public IList<ICommand> Commands
		{
			get { return commands; }
		}

		public virtual void ForceEndOfCompletionWithoutFurtherWait()
		{
			int remaining;
			do
			{
				remaining = latch.Set();
			} while (remaining > 0);
		}

		public void Add(ICommand command)
		{
			commands.Add(command);
		}

		public virtual void WaitForCompletion(TimeSpan timeOut)
		{
			if (latch == null)
				throw new InvalidOperationException("Called WaitForCompletion before calling Execute");
			latch.WaitOne(timeOut);
		}

		protected override void DoExecute(IProcessContextFactory contextFactory)
		{
			latch = new CountdownLatch(commands.Count);
			IProcessContext context = contextFactory.CreateAndStart();
			try
			{
				BeforeExecutingCommands(context);
				foreach (ICommand command in commands)
				{
					command.Completed += delegate
					{
						int remaining = latch.Set();
						if (remaining == 0)
						{
							RaiseCompleted();
							context.Stop();
						}
					};
					try
					{
						RegisterForExecution(command, contextFactory, context);
					}
					catch (Exception ex)
					{
						context.Publish(Messages.Exception, ex);
						context.Stop();
						throw;
					}
				}
			}
			finally
			{
				context.Stop();
			}
		}

		protected virtual void BeforeExecutingCommands(IProcessContext context)
		{
			
		}

		protected virtual void RegisterForExecution(ICommand command, IProcessContextFactory contextFactory, IProcessContext context)
		{
			EtlConfigurationContext current = EtlConfigurationContext.Current;
			//context.Enqueue(delegate
			{
				using(current.EnterContext())
				{
					command.Execute(contextFactory);
				}
			}//);
		}
	}
}