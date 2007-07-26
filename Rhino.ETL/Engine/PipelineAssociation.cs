using System;
using System.Collections;
using System.Collections.Generic;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public class PipelineAssociation
	{
		private string from;
		private string to;
		private string fromQueue;
		private string toQueue;
		private AssoicationType fromType;
		private AssoicationType toType;
		private IDictionary parameters = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		private IInput fromInstance;
		private IOutput toInstance;

		public IDictionary Parameters
		{
			get { return parameters; }
		}

		public AssoicationType FromType
		{
			get { return fromType; }
			set { fromType = value; }
		}

		public AssoicationType ToType
		{
			get { return toType; }
			set { toType = value; }
		}

		public string From
		{
			get { return from; }
			set { from = value; }
		}

		public string To
		{
			get { return to; }
			set { to = value; }
		}

		public string FromQueue
		{
			get { return fromQueue; }
			set { fromQueue = value; }
		}

		public string ToQueue
		{
			get { return toQueue; }
			set { toQueue = value; }
		}

		public void Validate(ICollection<string> messages)
		{
			ValidateName(messages, From, FromType);
			ValidateName(messages, To, ToType);
		}

		private void ValidateName(ICollection<string> messages, string name, AssoicationType assoicationType)
		{
			int associationIndex = Pipeline.Current.Associations.IndexOf(this);
			int count = 0;
			if (EtlConfigurationContext.Current.Sources.ContainsKey(name) &&
			    (assoicationType == AssoicationType.Any || assoicationType == AssoicationType.Sources))
				count += 1;
			if (EtlConfigurationContext.Current.Destinations.ContainsKey(name) &&
			    (assoicationType == AssoicationType.Any || assoicationType == AssoicationType.Destinations))
				count += 1;
			if (EtlConfigurationContext.Current.Transforms.ContainsKey(name) &&
			    (assoicationType == AssoicationType.Any || assoicationType == AssoicationType.Transforms))
				count += 1;

			if (count == 0)
			{
				messages.Add(
					string.Format("Could not find element '{0}' on association #{1} in pipeline [{2}]", name, associationIndex,
					              Pipeline.Current.Name));
			}
			if (count > 1)
			{
				messages.Add(
					string.Format(
						"Ambigious match for '{0}' on association #{1} in pipeline [{2}] - you need to qualify it with Sources.{0}, Destinations.{0} or Transforms.{0}",
						name, associationIndex, Pipeline.Current.Name));
			}
		}

		public void PerformSecondStagePass()
		{
			FromInstance = FindFromContext<IInput>(From, FromType);
			ToInstance = FindFromContext<IOutput>(To, ToType);
		}

		public IOutput ToInstance
		{
			get { return toInstance; }
			set { toInstance = value; }
		}

		public IInput FromInstance
		{
			get { return fromInstance; }
			set { fromInstance = value; }
		}

		public T FindFromContext<T>(string name, AssoicationType assoicationType)
			where T : class
		{
			T obj = null;
			if (EtlConfigurationContext.Current.Sources.ContainsKey(name) &&
			    (assoicationType == AssoicationType.Any || assoicationType == AssoicationType.Sources))
			{
				obj = EtlConfigurationContext.Current.Sources[name] as T;
			}
			if (EtlConfigurationContext.Current.Destinations.ContainsKey(name) &&
			    (assoicationType == AssoicationType.Any || assoicationType == AssoicationType.Destinations))
			{
				obj = EtlConfigurationContext.Current.Destinations[name] as T;
			}
			if (EtlConfigurationContext.Current.Transforms.ContainsKey(name) &&
			    (assoicationType == AssoicationType.Any || assoicationType == AssoicationType.Transforms))
			{
				obj = EtlConfigurationContext.Current.Transforms[name] as T;
			}
			if (obj == null)
			{
				throw new ExpectedInterfaceNotfoundException("Expected " + name + " to implement " + typeof (T).Name +
				                                             " but it doesn't.");
			}
			return obj;
		}

	    public void ConnectEnds()
	    {
	    	FromInstance.RegisterForwarding(
	    		new PipeLineStage(
	    			FromQueue ?? DefaultOutputQueue,
	    			ToInstance, 
					ToQueue ?? DefaultInputQueue, 500, Parameters)
					);
	    }

	    private const string DefaultInputQueue = "Input";
        private const string DefaultOutputQueue = "Output";
	}
}