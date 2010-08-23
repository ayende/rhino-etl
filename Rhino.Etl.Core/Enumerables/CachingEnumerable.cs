namespace Rhino.Etl.Core.Enumerables
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// There are several places where we need to iterate over an enumerable
    /// several times, but we cannot assume that it is safe to do so.
    /// This class will allow to safely use an enumerable multiple times, by caching
    /// the results after the first iteration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachingEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private bool? isFirstTime = null;
        private IEnumerator<T> internalEnumerator;
        private readonly LinkedList<T> cache = new LinkedList<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingEnumerable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        public CachingEnumerable(IEnumerable<T> inner)
        {
            internalEnumerator = inner.GetEnumerator();
        }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if(isFirstTime==null)
            {
                isFirstTime = true;
            }
            else if(isFirstTime.Value)
            {
                isFirstTime = false;
                internalEnumerator.Dispose();
                internalEnumerator = cache.GetEnumerator();
            }
            else 
            {
                internalEnumerator = cache.GetEnumerator();
            }

            return this;
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        ///<summary>
        ///Gets the element in the collection at the current position of the enumerator.
        ///</summary>
        ///
        ///<returns>
        ///The element in the collection at the current position of the enumerator.
        ///</returns>
        ///
        T IEnumerator<T>.Current
        {
            get { return internalEnumerator.Current; }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            internalEnumerator.Dispose();
        }

        ///<summary>
        ///Advances the enumerator to the next element of the collection.
        ///</summary>
        ///
        ///<returns>
        ///true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            bool result = internalEnumerator.MoveNext();
            if (result && isFirstTime.Value)
                cache.AddLast(internalEnumerator.Current);
            return result;
        }

        ///<summary>
        ///Sets the enumerator to its initial position, which is before the first element in the collection.
        ///</summary>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public void Reset()
        {
            internalEnumerator.Reset();
        }

        ///<summary>
        ///Gets the current element in the collection.
        ///</summary>
        ///
        ///<returns>
        ///The current element in the collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception><filterpriority>2</filterpriority>
        public object Current
        {
            get { return internalEnumerator.Current; }
        }
    }
}