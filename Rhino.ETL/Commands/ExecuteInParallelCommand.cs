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

		public virtual bool WaitForCompletion(TimeSpan timeOut)
		{
			if (latch == null)
				throw new InvalidOperationException("Called WaitForCompletion before calling Execute");
			return latch.WaitOne(timeOut);
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
					try
					{
						ExecuteCommand(contextFactory, context, command);
					}
					catch (Exception ex)
					{
						context.Publish(Messages.Exception, ex);
						throw;
					}
				}
			}
			finally
			{
				context.Stop();
			}
		}

		protected virtual void ExecuteCommand(IProcessContextFactory contextFactory, ICommandQueue context, ICommand command)
		{
			EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
			context.Enqueue(delegate
			{
				IProcessContext reportStatusContext = contextFactory.CreateAndStart();
				command.Completed += OnCommandCompleted(reportStatusContext);
				try
				{
					using (configurationContext.EnterContext())
						command.Execute(contextFactory);
				}
				catch (Exception ex)
				{
					reportStatusContext.Publish(Messages.Exception, ex);
					logger.Error("Failed to execute command " + command, ex);
				}
				finally
				{
					reportStatusContext.Stop();
				}
			});
		}

		protected Action<ICommand> OnCommandCompleted(IThreadController context)
		{
			return delegate
			{
				int remaining = latch.Set();
				if (remaining == 0)
				{
					RaiseCompleted();
					context.Stop();
				}
			};
		}

		protected virtual void BeforeExecutingCommands(IProcessContext context)
		{

		}
	}
}