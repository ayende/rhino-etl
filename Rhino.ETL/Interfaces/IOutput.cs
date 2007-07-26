using System.Collections;

namespace Rhino.ETL
{
	public interface IOutput
	{
        string Name { get; }
	    void Process(string queueName, Row row, IDictionary parameters);
		void Complete(string queueName);
	}
}