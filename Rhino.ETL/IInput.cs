using System;
using System.Collections;

namespace Rhino.ETL
{
	public interface IInput
	{
        string Name { get; }
	    void ForwardTo(string inQueue, IOutput instance, string outQueue, IDictionary parameters);
	}
}