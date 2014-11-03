using Boo.Lang;

namespace Rhino.Etl.Core
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Text;
    using Exceptions;

    /// <summary>
    /// A dictionary that can be access with a natural syntax from Boo
    /// </summary>
    [Serializable]
    public class QuackingDictionary : IDictionary, IQuackFu
    {
        /// <summary>
        /// Default value for the Comparer property.
        /// Defines how keys are compared (case sensitivity, hashing algorithm, etc.)
        /// </summary>
        public static StringComparer DefaultComparer { get; set; }

        /// <summary>
        /// Initialization for static members/properties
        /// </summary>
        static QuackingDictionary()
        {
            DefaultComparer = StringComparer.InvariantCultureIgnoreCase;
        }

        /// <summary>
        /// The inner items collection
        /// </summary>
        protected IDictionary items;

        /// <summary>
        /// The last item that was access, useful for debugging
        /// </summary>
        protected string lastAccess;

        private bool throwOnMissing = false;

        /// <summary>
        /// Defines how keys are compared (case sensitivity, hashing algorithm, etc.)
        /// </summary>
        protected StringComparer Comparer { get; set; }

        /// <summary>
        /// Set the flag to thorw if key not found.
        /// </summary>
        public void ShouldThorwIfKeyNotFound()
        {
            throwOnMissing = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuackingDictionary"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="comparer">Defines key equality</param>
        public QuackingDictionary(IDictionary items, StringComparer comparer)
        {
            Comparer = comparer;

            if (items != null)
                this.items = new Hashtable(items, Comparer);
            else
                this.items = new Hashtable(Comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuackingDictionary"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public QuackingDictionary(IDictionary items)
            : this(items, DefaultComparer)
        {
        }


        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get
            {
                if (throwOnMissing && items.Contains(key) == false)
                    throw new MissingKeyException(key);

                lastAccess = key;
                return items[key];
            }
            set
            {
                lastAccess = key;
                if(value == DBNull.Value)
                    items[key] = null;
                else
                    items[key] = value;
            }
        }

        /// <summary>
        /// Get a value by name or first parameter
        /// </summary>
        public virtual object QuackGet(string name, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return this[name];
            if (parameters.Length == 1)
                return this[(string)parameters[0]];
            throw new ParameterCountException("You can only call indexer with a single parameter");
        }

        /// <summary>
        /// Set a value on the given name or first parameter
        /// </summary>
        public object QuackSet(string name, object[] parameters, object value)
        {
            if (parameters == null || parameters.Length == 0)
                return this[name] = value;
            if (parameters.Length == 1)
                return this[(string)parameters[0]] = value;
            throw new ParameterCountException("You can only call indexer with a single parameter");
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public object QuackInvoke(string name, params object[] args)
        {
            throw new NotSupportedException(
                "You cannot invoke methods on a row, it is merely a data structure, after all.");
        }

        /// <summary>
        /// A debbug view of quacking dictionary
        /// </summary>
        internal class QuackingDictionaryDebugView
        {
            private readonly IDictionary items;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuackingDictionaryDebugView"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            public QuackingDictionaryDebugView(QuackingDictionary dictionary)
            {
                this.items = dictionary.items;
            }

            /// <summary>
            /// Gets the items.
            /// </summary>
            /// <value>The items.</value>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair[] Items
            {
                get
                {
                    var pairs = new System.Collections.Generic.List<KeyValuePair>();
                    foreach (DictionaryEntry item in items)
                    {
                        pairs.Add(new KeyValuePair(item.Key, item.Value));
                    }
                    return pairs.ToArray();
                }
            }

            /// <summary>
            /// Represent a single key/value pair for the debugger
            /// </summary>
            [DebuggerDisplay("{value}", Name = "[{key}]", Type = "")]
            internal class KeyValuePair
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private readonly object key;
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private readonly object value;


                /// <summary>
                /// Initializes a new instance of the <see cref="KeyValuePair"/> class.
                /// </summary>
                /// <param name="key">The key.</param>
                /// <param name="value">The value.</param>
                public KeyValuePair(object key, object value)
                {
                    this.key = key;
                    this.value = value;
                }

                /// <summary>
                /// Gets the key.
                /// </summary>
                /// <value>The key.</value>
                public object Key
                {
                    get { return key; }
                }

                /// <summary>
                /// Gets the value.
                /// </summary>
                /// <value>The value.</value>
                public object Value
                {
                    get { return value; }
                }
            }
        }


        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (DictionaryEntry item in items)
            {
                sb.Append(item.Key)
                    .Append(" : ");
                if (item.Value is string)
                {
                    sb.Append("\"")
                        .Append(item.Value)
                        .Append("\"");
                }
                else
                {
                    sb.Append(item.Value);
                }
                sb.Append(", ");

            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new Hashtable(items).GetEnumerator();
        }


        ///<summary>
        ///Returns an <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        ///<summary>
        ///Determines whether the <see cref="T:System.Collections.IDictionary"></see> object contains an element with the specified key.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Collections.IDictionary"></see> contains an element with the key; otherwise, false.
        ///</returns>
        ///
        ///<param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"></see> object.</param>
        ///<exception cref="T:System.ArgumentNullException">key is null. </exception><filterpriority>2</filterpriority>
        public bool Contains(object key)
        {
            return items.Contains(key);
        }

        ///<summary>
        ///Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</summary>
        ///
        ///<param name="value">The <see cref="T:System.Object"></see> to use as the value of the element to add. </param>
        ///<param name="key">The <see cref="T:System.Object"></see> to use as the key of the element to add. </param>
        ///<exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"></see> object. </exception>
        ///<exception cref="T:System.ArgumentNullException">key is null. </exception>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> is read-only.-or- The <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception><filterpriority>2</filterpriority>
        public void Add(object key, object value)
        {
            items.Add(key, value);
        }

        ///<summary>
        ///Removes all elements from the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</summary>
        ///
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> object is read-only. </exception><filterpriority>2</filterpriority>
        public void Clear()
        {
            items.Clear();
        }


        ///<summary>
        ///Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</summary>
        ///
        ///<param name="key">The key of the element to remove. </param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> object is read-only.-or- The <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception>
        ///<exception cref="T:System.ArgumentNullException">key is null. </exception><filterpriority>2</filterpriority>
        public void Remove(object key)
        {
            items.Remove(key);
        }

        ///<summary>
        ///Gets or sets the element with the specified key.
        ///</summary>
        ///
        ///<returns>
        ///The element with the specified key.
        ///</returns>
        ///
        ///<param name="key">The key of the element to get or set. </param>
        ///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IDictionary"></see> object is read-only.-or- The property is set, key does not exist in the collection, and the <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception>
        ///<exception cref="T:System.ArgumentNullException">key is null. </exception><filterpriority>2</filterpriority>
        public object this[object key]
        {
            get { return items[key]; }
            set { items[key] = value; }
        }

        ///<summary>
        ///Gets an <see cref="T:System.Collections.ICollection"></see> object containing the keys of the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.ICollection"></see> object containing the keys of the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public ICollection Keys
        {
            get { return items.Keys; }
        }

        ///<summary>
        ///Gets an <see cref="T:System.Collections.ICollection"></see> object containing the values in the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.ICollection"></see> object containing the values in the <see cref="T:System.Collections.IDictionary"></see> object.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public ICollection Values
        {
            get { return items.Values; }
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object is read-only.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Collections.IDictionary"></see> object is read-only; otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size; otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool IsFixedSize
        {
            get { return items.IsFixedSize; }
        }

        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        ///</summary>
        ///
        ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
        ///<param name="index">The zero-based index in array at which copying begins. </param>
        ///<exception cref="T:System.ArgumentNullException">array is null. </exception>
        ///<exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
        ///<exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception><filterpriority>2</filterpriority>
        public void CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
        ///</summary>
        ///
        ///<returns>
        ///The number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public int Count
        {
            get { return items.Count; }
        }

        ///<summary>
        ///Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</summary>
        ///
        ///<returns>
        ///An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public object SyncRoot
        {
            get { return items.SyncRoot; }
        }

        ///<summary>
        ///Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
        ///</summary>
        ///
        ///<returns>
        ///true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool IsSynchronized
        {
            get { return items.IsSynchronized; }
        }
    }
}