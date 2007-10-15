namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using log4net;

	public abstract class EtlConfigurationContext : ContextfulObjectBase<EtlConfigurationContext>
	{
		private readonly IDictionary<string, Connection> connections;
		private readonly IDictionary<string, DataSource> sources;
		private readonly IDictionary<string, DataDestination> destinations;
		private readonly IDictionary<string, Join> joins;
		private readonly IDictionary<string, Transform> transforms;
		private readonly IDictionary<string, Pipeline> pipelines;
		private readonly IDictionary<string, Target> targets;
		private List<string> validationMessages = new List<string>();

		private ILog log;
		private List<string> errors = new List<string>();

		public ILog Log
		{
			get { return log; }
		}

		public EtlConfigurationContext()
		{
			log = LogManager.GetLogger(GetType());

			connections = new Dictionary<string, Connection>(StringComparer.InvariantCultureIgnoreCase);
			sources = new Dictionary<string, DataSource>(StringComparer.InvariantCultureIgnoreCase);
			transforms = new Dictionary<string, Transform>(StringComparer.InvariantCultureIgnoreCase);
			destinations = new Dictionary<string, DataDestination>(StringComparer.InvariantCultureIgnoreCase);
			pipelines = new Dictionary<string, Pipeline>(StringComparer.InvariantCultureIgnoreCase);
			joins = new Dictionary<string, Join>(StringComparer.InvariantCultureIgnoreCase);
			targets = new Dictionary<string, Target>(StringComparer.InvariantCultureIgnoreCase);
		}

		public IDictionary<string, Pipeline> Pipelines
		{
			get { return pipelines; }
		}

		public IDictionary<string, Transform> Transforms
		{
			get { return transforms; }
		}

		public IDictionary<string, Join> Joins
		{
			get { return joins; }
		}

		public IDictionary<string, Connection> Connections
		{
			get { return connections; }
		}

		public IDictionary<string, DataSource> Sources
		{
			get { return sources; }
		}

		public IDictionary<string, DataDestination> Destinations
		{
			get { return destinations; }
		}

		public IDictionary<string, Target> Targets
		{
			get { return targets; }
		}

		public void AddConnection(string name, Connection connection)
		{
			if (connections.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a connection named [" + name + "]");
			connections.Add(name, connection);
		}

		public void AddSource(string name, DataSource source)
		{
			if (sources.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a source named [" + name + "]");
			sources.Add(name, source);
		}

		public abstract void BuildConfig();

		public void AddTransform(string name, Transform transform)
		{
			if (transforms.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a transfrom named [" + name + "]");
			transforms.Add(name, transform);
		}

		public void AddDestination(string name, DataDestination destination)
		{
			if (destinations.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a destination named [" + name + "]");
			destinations.Add(name, destination);
		}

		public bool Validate()
		{
			using (EnterContext())
			{
				validationMessages = new List<string>();
				foreach (DataSource source in sources.Values)
				{
					source.Validate(validationMessages);
				}

				foreach (DataDestination destination in destinations.Values)
				{
					destination.Validate(validationMessages);
				}

				foreach (Pipeline pipeline in pipelines.Values)
				{
					pipeline.Validate(validationMessages);
				}
				return validationMessages.Count == 0;
			}
		}


		public List<string> ValidationMessages
		{
			get { return validationMessages; }
		}

		public List<string> Errors
		{
			get { return errors; }
		}

		public void AddPipeline(string name, Pipeline pipeline)
		{
			if (pipelines.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a pipeline named [" + name + "]");
			pipelines.Add(name, pipeline);
		}

		public void AddJoin(string name, Join join)
		{
			if (joins.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a join named [" + name + "]");
			joins.Add(name, join);
		}

		public void AddTarget(string name, Target target)
		{
			if (targets.ContainsKey(name))
				throw new DuplicateKeyException("The current context already contains a target named [" + name + "]");
			targets.Add(name, target);
		
		}

		public ExecutionPackage BuildPackage()
		{
			if (Validate() == false)
				throw new ValidationExceptionException(string.Join(Environment.NewLine, ValidationMessages.ToArray()));
			using (EnterContext())
			{
				foreach (DataSource source in sources.Values)
				{
					source.PerformSecondStagePass();
				}

				foreach (DataDestination destination in destinations.Values)
				{
					destination.PerformSecondStagePass();
				}

				foreach (Pipeline pipeline in pipelines.Values)
				{
					pipeline.PerformSecondStagePass();
				}
			}
			return new ExecutionPackage(this);
		}

		public void AddError(string error)
		{
			Log.Error(error);
			errors.Add(error);
		}

		public FluentFileHelper Read(Type type)
		{
			return new FluentFileHelper(type);
		}

		public FluentFileHelper Write(Type type)
		{
			return new FluentFileHelper(type);
		}
	}
}