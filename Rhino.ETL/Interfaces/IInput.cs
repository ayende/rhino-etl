using System;
using System.Collections;

namespace Rhino.ETL
{
	public interface IInput
	{
        string Name { get; }
	    void RegisterForwarding(PipeLineStage parameters);
	}
}