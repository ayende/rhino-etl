using System;
using System.Threading;
using log4net;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	public class ExecutePipeline : AbstractCommand
	{
		private readonly Pipeline pipeline;
		private static ILog logger = LogManager.GetLogger(typeof(ExecutePipeline));

		public ExecutePipeline(Target target, Pipeline pipeline)
			: base(target)
		{
			this.pipeline = pipeline;
		}

		protected override void DoExecute()
		{
			logger.InfoFormat("Starting pipeline {0}", pipeline.Name);
			pipeline.Completed += delegate { RaiseCompleted(); };
			pipeline.Prepare();
			ExecutionPackage.Current.RegisterForExecution(target, delegate
			{
				pipeline.Start(target);
			});
		}
	}
}

