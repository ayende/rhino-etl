namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections;
	using Exceptions;
	using Impl;
	using Interfaces;
	using Retlang;

	public abstract class Transform : TransformationBase<Transform>, IProcess
	{
		protected Transform(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddTransform(name, this);
		}


		protected virtual void OnComplete(string QueueName)
		{

		}

		public void Apply(IProcessContext context, Row row, IDictionary parameters)
		{
			PrepareCurrentTransformParameters(row);
			try
			{
				DoApply(row, new QuackingDictionary(parameters));
			}
			catch (Exception ex)
			{
				string message = "Failed to execute "+ Name+" with row: " + row;
				Logger.Error(message,ex);
				context.Publish(Messages.Exception, new ScriptEvalException(message, ex));
			}
		}

		public void Start(IProcessContext context, params string[]  inputNames)
		{
			if (inputNames.Length != 1)
				throw new ArgumentException("Transform must have a single input name");
			string input = inputNames[0];
		
			EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
			Pipeline currentPipeline = Pipeline.Current;
			Items[ProcessContextKey] = context;
			context.Subscribe<object>(new TopicEquals(input + Messages.Done), delegate
			{
				using (configurationContext.EnterContext())
				using (currentPipeline.EnterContext())
				{
					OnComplete(OutputName);
					context.Publish(Name + "." + OutputName + Messages.Done, Messages.Done);
					context.Stop();
				}
			});
			Hashtable parameters = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			context.Subscribe<Row>(new TopicEquals(input), delegate(IMessageHeader header, Row msg)
			{
				using (configurationContext.EnterContext())
				using (currentPipeline.EnterContext())
					Process(context, msg, parameters);
			});
		}

		public void Process(IProcessContext context,Row row, IDictionary parameters)
		{
			Apply(context, row, parameters);
			ForwardRow();
		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
	}
}