namespace Rhino.Etl.Core.DataReaders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Adapts a dictionary to a descriptor
    /// </summary>
    public class DictionaryDescriptorAdapter : Descriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDescriptorAdapter"/> class.
        /// </summary>
        /// <param name="pair">The pair.</param>
        public DictionaryDescriptorAdapter(KeyValuePair<string, Type> pair) : base(pair.Key, pair.Value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override object GetValue(object obj)
        {
            return ((IDictionary) obj)[Name];
        }
    }
}