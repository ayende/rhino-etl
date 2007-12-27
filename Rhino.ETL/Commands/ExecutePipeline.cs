using System;
using System.Threading;
using log4net;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	using Retlang;

	public class ExecutePipeline : AbstractCommand
	{
		private readonly Pipeline pipeline;

		public ExecutePipeline(Target target, Pipeline pipeline)
			: base(target)
		{
			this.pipeline = pipeline;
		}

		protected override void DoExecute(IProcessContextFactory contextFactory)
		{
			logger.InfoFormat("Starting pipeline {0}", pipeline.Name);
			pipeline.Completed += delegate { RaiseCompleted(); };
			pipeline.Prepare();
			pipeline.Start(contextFactory);
		}
	}
}

