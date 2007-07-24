using System;

namespace Rhino.ETL
{
	public interface IInput
	{
		void RegisterAction(string queueName, Action<Row> action, Command onComplete);
	}
}