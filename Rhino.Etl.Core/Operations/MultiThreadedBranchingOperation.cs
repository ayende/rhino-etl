using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
			if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
				throw new NotSupportedException(string.Format("Cannot use {0} in a single threaded apartment state", GetType().Name));

			var handles = new List<WaitHandle>(Operations.Count);

			var input = new GiveOnePullOneThreadsafeEnumerable<Row>(Operations.Count, rows);

			foreach (var operation in Operations)
			{
				var clone = input.Select(r => r.Clone());
				var result = operation.Execute(clone);

				if (result == null)
					continue;

				var enumerator = result.GetEnumerator();
				var handle = new ManualResetEvent(false);

				handles.Add(handle);

				ThreadPool.QueueUserWorkItem(delegate
				                             {
				                             	try
				                             	{
				                             		while (enumerator.MoveNext()) ;
				                             	}
				                             	catch
				                             	{
				                             		enumerator.Dispose();
				                             	}
				                             	finally
				                             	{
				                             		handle.Set();
				                             	}
				                             });
			}

			WaitHandle.WaitAll(handles.ToArray());

			yield break;
		}
	}
}