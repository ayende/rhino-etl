using System.Collections;

namespace Rhino.ETL
{
	public interface IOutput
	{
		void PushInto(string queue, Row row, IDictionary parameters);
		void Complete(string queueName);
	}
}