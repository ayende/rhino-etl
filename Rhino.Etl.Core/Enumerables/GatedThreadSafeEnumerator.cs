using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Rhino.Etl.Core.Enumerables
{
	/// <summary>
	/// An iterator to be consumed by concurrent threads only which supplies an element of the decorated enumerable one by one
	/// </summary>
	/// <typeparam name="T">The type of the decorated enumerable</typeparam>
	public class GatedThreadSafeEnumerator<T> :	WithLoggingMixin, IEnumerable<T>, IEnumerator<T>
	{
		private readonly int numberOfConsumers;
		private readonly IEnumerator<T> innerEnumerator;
		private int callsToMoveNext;
		private readonly object sync = new object();
		private bool moveNext;
		private T current;
		private int consumersLeft;

		/// <summary>
		/// Creates a new instance of <see cref="GatedThreadSafeEnumerator{T}"/>
		/// </summary>
		/// <param name="numberOfConsumers">The number of consumers that will be consuming this iterator concurrently</param>
		/// <param name="source">The decorated enumerable that will be iterated and fed one element at a time to all consumers</param>
		public GatedThreadSafeEnumerator(int numberOfConsumers, IEnumerable<T> source)
		{
			this.numberOfConsumers = numberOfConsumers;
			consumersLeft = numberOfConsumers;
			innerEnumerator = source.GetEnumerator();
		}

		///	<summary>
		///	Get	the	enumerator
		///	</summary>
		///	<returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		///	<summary>
		///	Dispose	the	enumerator
		///	</summary>
		public void Dispose()
		{
			if(Interlocked.Decrement(ref consumersLeft) == 0)
			{
				Debug("Disposing inner enumerator");
				innerEnumerator.Dispose();
			}
		}

		///	<summary>
		///	MoveNext the enumerator
		///	</summary>
		///	<returns></returns>
		public bool MoveNext()
		{
			lock (sync)
				if (Interlocked.Increment(ref callsToMoveNext) == numberOfConsumers)
				{
					callsToMoveNext = 0;
					moveNext = innerEnumerator.MoveNext();
					current = innerEnumerator.Current;

					Debug("Pulsing all waiting threads");

					Monitor.PulseAll(sync);
				}
				else
				{
					Monitor.Wait(sync);
				}

			return moveNext;
		}

		///	<summary>
		///	Reset the enumerator
		///	</summary>
		public void Reset()
		{
			throw new NotSupportedException();
		}

		///	<summary>
		///	The	current	value of the enumerator
		///	</summary>
		public T Current
		{
			get { return current; }
		}

		object IEnumerator.Current
		{
			get { return ((IEnumerator<T>)this).Current; }
		}

		///	<summary>
		///	Number of consumers	left that have not yet completed
		///	</summary>
		public int ConsumersLeft { get { return consumersLeft; } }
	}
}