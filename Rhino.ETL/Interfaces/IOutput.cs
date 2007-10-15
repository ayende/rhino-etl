using System;
using System.Collections;

namespace Rhino.ETL
{
	using Engine;

	public delegate void OutputCompleted(IOutput output, QueueKey key);

	public interface IOutput
	{
		string Name { get; }

		event OutputCompleted Completed;

		void Process(QueueKey key, Row row, IDictionary parameters);

		void Complete(QueueKey key);
	}
}