using System;
using System.Collections;
using log4net;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public abstract class ContextfulObjectBase<T> 
		where T : ContextfulObjectBase<T>
	{
		[ThreadStatic] private static T current;
		[ThreadStatic] private static int nestedCount;

		private IDictionary items = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		private ILog logger;

		public abstract string Name
		{
			get;
		}

		public ContextfulObjectBase()
		{
			logger = LogManager.GetLogger(GetType());
		}

		public IDictionary Items
		{
			get { return items; }
		}


		public ILog Logger
		{
			get { return logger; }
		}

		public static T Current
		{
			get
			{
				if (current == null)
					throw new ContextException("You are not inside context for " + typeof(T).Name);
				return current;
			}
			set
			{
				if (value == current)
				{
					nestedCount += 1;
					return;
				}
				if (current != null && value != null)
				{
					string s = string.Format("Tried to set the Current context for {0} without first clearing the existing one!", typeof(T).Name);
					throw new ContextException(s);
				}
				nestedCount -= 1;
				if (nestedCount > 0)
					return;
				nestedCount = 0;
				current = value;
			}
		}

		public IDisposable EnterContext()
		{
			Current = (T)this;
			return new ExitContext();
		}

		private class ExitContext :IDisposable
		{
			public void Dispose()
			{
				Current = null;
			}
		}
	}
}