using System;
using System.Collections;
using System.Diagnostics;
using log4net;
using Rhino.ETL.Exceptions;
using Rhino.ETL.Impl;

namespace Rhino.ETL.Engine
{
	using Retlang;

	[DebuggerDisplay("{GetType().Name}: {Name}")]
	public abstract class ContextfulObjectBase<T> : Configurable
		where T : ContextfulObjectBase<T>
	{
		[ThreadStatic]
		protected static T current;
		[ThreadStatic]
		protected static int nestedCount;

		protected const string ProcessContextKey = "Process.Context.Key";

		private readonly QuackingDictionary items = new QuackingDictionary(
			new Hashtable(StringComparer.InvariantCultureIgnoreCase));
		private readonly ILog logger;


		protected IProcessContext ProcessContextFromCurrentContext
		{
			get { return (IProcessContext) Items[ProcessContextKey]; }
		}

		public abstract string Name
		{
			get;
		}

		public ContextfulObjectBase()
		{
			logger = LogManager.GetLogger(GetType());
		}

		public QuackingDictionary Items
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
				if (value != null && value == current)
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
				nestedCount = 1;
				current = value;
			}
		}

		public IDisposable EnterContext()
		{
			Current = (T)this;
			return new ExitContext();
		}

		private class ExitContext : IDisposable
		{
			public void Dispose()
			{
				Current = null;
			}
		}
	}
}