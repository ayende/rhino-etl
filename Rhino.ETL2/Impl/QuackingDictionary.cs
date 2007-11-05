using System;
using System.Collections;
using System.Diagnostics;
using Boo.Lang;
using Rhino.ETL.Exceptions;
using System.Collections.Generic;

namespace Rhino.ETL.Impl
{
	using System.Text;

	[Serializable]
	public class QuackingDictionary : IQuackFu
	{
		protected IDictionary items;

		public QuackingDictionary() : this(null)
		{
		}

		public QuackingDictionary(IDictionary items)
		{
			this.items = new Hashtable(items ?? new Hashtable(), StringComparer.InvariantCultureIgnoreCase);
		}


		public object this[string key]
		{
			get { return items[key]; }
			set { items[key] = value; }
		}

		public virtual object QuackGet(string name, object[] parameters)
		{
			if (parameters == null || parameters.Length == 0)
				return items[name];
			if (parameters.Length == 1)
				return items[parameters[0]];
			throw new ParameterCountException("You can only call indexer with a single parameter");
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			if (parameters == null || parameters.Length == 0)
				return items[name] = value;
			if (parameters.Length == 1)
				return items[parameters[0]] = value;
			throw new ParameterCountException("You can only call indexer with a single parameter");
		}

		public object QuackInvoke(string name, params object[] args)
		{
			throw new InvalidOperationException(
				"You cannot invoke methods on a row, it is merely a data structure, after all.");
		}

		internal class QuackingDictionaryDebugView
		{
			private IDictionary items;

			public QuackingDictionaryDebugView(QuackingDictionary dictionary)
			{
				this.items = dictionary.items;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePairs[] Items
			{
				get
				{
					List<KeyValuePairs> pairs = new List<KeyValuePairs>();
					foreach (DictionaryEntry item in items)
					{
						pairs.Add(new KeyValuePairs(item.Key, item.Value));
					}
					return pairs.ToArray();
				}
			}

			[DebuggerDisplay("{value}", Name = "[{key}]", Type = "")]
			internal class KeyValuePairs
			{
				[DebuggerBrowsable(DebuggerBrowsableState.Never)]
				private object key;
				[DebuggerBrowsable(DebuggerBrowsableState.Never)]
				private object value;


				public KeyValuePairs(object key, object value)
				{
					this.key = key;
					this.value = value;
				}

				public object Key
				{
					get { return key; }
				}

				public object Value
				{
					get { return value; }
				}
			}
		}


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
	}
}