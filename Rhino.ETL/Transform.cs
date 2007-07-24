using System;
using System.Collections;
using Rhino.ETL.Impl;

namespace Rhino.ETL
{
	public abstract class Transform : ContextfulObjectBase<Transform>, IInputOutput
    {
		[ThreadStatic]
		private static bool shouldSkipRow;
		
		private string name;
		QueuesManager queuesManager;

    	public bool ShouldSkipRow
        {
            get { return shouldSkipRow; }
        }

        protected Transform(string name)
        {
			this.queuesManager = new QueuesManager(Logger);
            this.name = name;
            EtlConfigurationContext.Current.AddTransform(name, this);
        }

        public override string Name
        {
            get { return name; }
        }

    	public void RemoveRow()
        {
            shouldSkipRow = true;
        }

		public void Apply(Row row, IDictionary parameters)
        {
            shouldSkipRow = false;
			DoApply(row, new QuackingDictionary(parameters));
        }


		public void RegisterAction(string queueName, Action<Row> action, Command onComplete)
		{
			queuesManager.RegisterAction(queueName, action, onComplete);
		}

		public void PushInto(string queueName, Row row, IDictionary parameters)
		{
			Apply(row, parameters);
			if(ShouldSkipRow)
				return;
			queuesManager.PushInto(queueName, row);
		}

		public void Complete(string queueName)
		{
			queuesManager.Complete(queueName);
		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
    }
}