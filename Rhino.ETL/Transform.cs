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
            this.name = name;
            this.queuesManager = new QueuesManager(name, Logger);
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

        public void ForwardTo(string inQueue, IOutput output, string outQueue, IDictionary parameters)
        {
            queuesManager.ForwardTo(inQueue, output, outQueue, parameters);
        }

	    public void Process(string queueName, Row row, IDictionary parameters)
	    {
            Apply(row, parameters);
            if (ShouldSkipRow)
                return;
            queuesManager.Forward(queueName, row);
	    }

	    public void Complete(string queueName)
		{
			queuesManager.Complete(queueName);
		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
    }
}