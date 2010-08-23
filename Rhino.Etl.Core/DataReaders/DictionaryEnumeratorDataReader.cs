namespace Rhino.Etl.Core.DataReaders
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A datareader over a collection of dictionaries
    /// </summary>
    public class DictionaryEnumeratorDataReader : EnumerableDataReader
    {
        private readonly IEnumerable<Row> enumerable;
        private readonly List<Descriptor> propertyDescriptors = new List<Descriptor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEnumeratorDataReader"/> class.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="enumerable">The enumerator.</param>
        public DictionaryEnumeratorDataReader(
            IDictionary<string, Type> schema,
            IEnumerable<Row> enumerable)
            : base(enumerable.GetEnumerator())
        {
            this.enumerable = enumerable;
            foreach (KeyValuePair<string, Type> pair in schema)
            {
                propertyDescriptors.Add(new DictionaryDescriptorAdapter(pair));
            }
        }

        /// <summary>
        /// Gets the descriptors for the properties that this instance
        /// is going to handle
        /// </summary>
        /// <value>The property descriptors.</value>
        protected override IList<Descriptor> PropertyDescriptors
        {
            get { return propertyDescriptors; }
        }

        /// <summary>
        /// Perform the actual closing of the reader
        /// </summary>
        protected override void DoClose()
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            disposable = enumerable as IDisposable;
            if(disposable != null)
                disposable.Dispose();
        }
    }
}