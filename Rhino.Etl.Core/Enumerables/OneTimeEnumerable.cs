using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rhino.Etl.Core.Enumerables
{
    /// <summary>
    /// An enumerator which enumerates its decorated enumerable only once
    /// </summary>
    /// <remarks>
    /// This is used to prevent side effects due to multiple enumerations of the inner enumerable
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    internal class OneTimeEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly IEnumerator<T> innerEnumerator;
        private bool alreadyEnumerated;

        public OneTimeEnumerable(IEnumerable<T> inner)
        {
            innerEnumerator = inner.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (alreadyEnumerated)
                return Enumerable.Empty<T>().GetEnumerator();

            alreadyEnumerated = true;
            return innerEnumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (alreadyEnumerated == false)
                innerEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return innerEnumerator.MoveNext();
        }

        public void Reset()
        {
            innerEnumerator.Reset();
        }

        public T Current
        {
            get { return innerEnumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}