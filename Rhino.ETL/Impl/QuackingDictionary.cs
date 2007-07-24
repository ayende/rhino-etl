using System;
using System.Collections;
using Boo.Lang;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Impl
{
	public class QuackingDictionary : IQuackFu
	{
		protected IDictionary items;

		public QuackingDictionary(IDictionary items)
		{
			this.items = items;
		}


		public object this[string key]
		{
			get { return items[key]; }
			set { items[key] = value; }
		}

		public object QuackGet(string name, object[] parameters)
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
	}
}