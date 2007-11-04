using System;
using System.Collections;
using Rhino.ETL.Engine;
using Rhino.ETL.Impl;

namespace Rhino.ETL.Engine
{
	using Retlang;

	public abstract class Transform : TransformationBase<Transform>
	{
		protected Transform(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddTransform(name, this);
		}


		public void Apply(Row row, IDictionary parameters)
		{
			PrepareCurrentTransformParameters(row);
			DoApply(row, new QuackingDictionary(parameters));
		}

		public void Start(IProcessContext context, string input)
		{
			Items[ProcessContextKey] = context;
			context.Subscribe<object>(new TopicEquals(input + Messages.Done), delegate
			{
				context.Publish(OutputName+Messages.Done, Messages.Done);
				context.Stop();
			});
			Hashtable parameters = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			context.Subscribe<Row>(new TopicEquals(input), delegate(IMessageHeader header, Row msg)
			{
				Process(msg, parameters);
			});
		}

		public void Process(Row row, IDictionary parameters)
		{
			Apply(row, parameters);
			ForwardRow();
		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
	}
}