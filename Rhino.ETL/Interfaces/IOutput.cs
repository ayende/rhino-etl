using System;
using System.Collections;

namespace Rhino.ETL
{
	public delegate void OutputCompleted(IOutput output, string queueName);

	public interface IOutput
	{
		string Name { get; }

		event OutputCompleted Completed;

		void Process(string queueName, Row row, IDictionary parameters);

		void Complete(string queueName);
	}
}