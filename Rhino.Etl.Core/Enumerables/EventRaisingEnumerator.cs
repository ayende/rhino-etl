namespace Rhino.Etl.Core.Enumerables
{
    using System.Collections.Generic;
    using Operations;

    /// <summary>
    /// An enumerator that will raise the events on the operation for each iterated item
    /// </summary>
    public class EventRaisingEnumerator : SingleRowEventRaisingEnumerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventRaisingEnumerator"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="inner">The innerEnumerator.</param>
        public EventRaisingEnumerator(IOperation operation, IEnumerable<Row> inner) : base(operation, inner)
        {}

        ///<summary>
        ///Advances the enumerator to the next element of the collection.
        ///</summary>
        ///
        ///<returns>
        ///true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public override bool MoveNext()
        {
            bool result = base.MoveNext();

            if(!result) 
                operation.RaiseFinishedProcessing();

            return result;
        }
    }
}
