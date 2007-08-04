using System;
using System.Collections;
using Rhino.ETL.Engine;

namespace Rhino.ETL
{
	public interface IInput
	{
        string Name { get; }
		void RegisterForwarding(Target target,PipeLineStage pipeLineStage);
	}
}