using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Rhino.Etl.Core.Enumerables
{
	internal class GiveOnePullOneThreadsafeEnumerable<T> : IEnumerable<T>, IEnumerator<T>
	{
		private readonly int numberOfConsumers;
		private readonly IEnumerator<T> innerEnumerator;
		private int callsToMoveNext;
		private readonly object sync = new object();
		private bool moveNext;
		private T current;
		private int callsToDispose;

		public GiveOnePullOneThreadsafeEnumerable(int numberOfConsumers, IEnumerable<T> source)
		{
			this.numberOfConsumers = numberOfConsumers;
			innerEnumerator = source.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			if(Interlocked.Increment(ref callsToDispose) == numberOfConsumers)
				innerEnumerator.Dispose();
		}

		public bool MoveNext()
		{
			lock (sync)
				if (++callsToMoveNext == numberOfConsumers)
				{
					callsToMoveNext = 0;
					moveNext = innerEnumerator.MoveNext();
					current = innerEnumerator.Current;

					Monitor.PulseAll(sync);
				}
				else
				{
					Monitor.Wait(sync);
				}

			return moveNext;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public T Current
		{
			get { return current; }
		}

		object IEnumerator.Current
		{
			get { return ((IEnumerator<T>)this).Current; }
		}
	}
}