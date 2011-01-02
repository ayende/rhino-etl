using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Rhino.Etl.Core.Enumerables;

namespace Rhino.Etl.Core.Operations
{
	/// <summary>
	/// Branch the current pipeline flow into all its inputs
	/// </summary>
	public class MultiThreadedBranchingOperation : AbstractBranchingOperation
	{
		/// <summary>
		/// Executes this operation
		/// </summary>
		/// <param name="rows">The rows.</param>
		/// <returns></returns>
		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			var input = new GatedThreadSafeEnumerator<Row>(Operations.Count, rows);

			var sync = new object();

			foreach (var operation in Operations)
			{
				var clone = input.Select(r => r.Clone());
				var result = operation.Execute(clone);

				if (result == null)
				{
					input.Dispose();
					continue;
				}

				var enumerator = result.GetEnumerator();

				ThreadPool.QueueUserWorkItem(delegate
				                             {
				                             	try
				                             	{
				                             		while (enumerator.MoveNext()) ;
				                             	}
				                             	finally
				                             	{
				                             		lock (sync)
				                             		{
														enumerator.Dispose();
														Monitor.Pulse(sync);
				                             		}
				                             	}
				                             });
			}

			lock (sync)
				while (input.ConsumersLeft > 0)
					Monitor.Wait(sync);

			yield break;
		}
	}
}