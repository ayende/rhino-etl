using System;
using log4net;

namespace Rhino.ETL.Commands
{
	public class ExecutePipeline : ICommand
	{
		private readonly Pipeline pipeline;
		private static ILog logger = LogManager.GetLogger(typeof (ExecutePipeline));
		
		public event Action<ICommand> Completed = delegate { };

		public ExecutePipeline(Pipeline pipeline)
		{
			this.pipeline = pipeline;
		}

		public void Execute()
		{
			logger.InfoFormat("Starting pipeline {0}", pipeline.Name);
			pipeline.Completed += delegate { Completed(this); };
			pipeline.Prepare();
			ExecutionPackage.Current.RegisterForExecution(pipeline.Start);
		}
	}
}

