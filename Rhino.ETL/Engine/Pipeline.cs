namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections.Generic;
	using Commons;
	using Interfaces;
	using Retlang;

	public class Pipeline : ContextfulObjectBase<Pipeline>
	{
		#region Delegates

		public delegate void PipelineCompleted(Pipeline completed);

		#endregion

		private readonly IList<PipelineAssociation> associations = new List<PipelineAssociation>();

		private readonly string name;
		private int completedDestinations = 0;
		private bool stopped = false;
		private int totalDestinations;
		private AssociationDependencies associationDependencies;

		public Pipeline(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddPipeline(name, this);
		}

		public override string Name
		{
			get { return name; }
		}


		public IList<PipelineAssociation> Associations
		{
			get { return associations; }
		}

		public event PipelineCompleted Completed = delegate
		{
		};

		public void AddAssociation(PipelineAssociation association)
		{
			associations.Add(association);
		}

		public void Validate(ICollection<string> messages)
		{
			using (EnterContext())
			{
				foreach (PipelineAssociation association in associations)
				{
					association.Validate(messages);
				}
			}
		}

		public void PerformSecondStagePass()
		{
			using (EnterContext())
			{
				foreach (PipelineAssociation association in associations)
				{
					association.PerformSecondStagePass();
				}
			}
		}


		public void Prepare()
		{
			PerformSecondStagePass();
		}

		public void Start(IProcessContextFactory contextFactory)
		{
			if (associations.Count == 0)
			{
				Completed(this);
				return;
			}

			associationDependencies = BuildDependencies();
			IProcessContext listenToPipeline = contextFactory.CreateAndStart();
			List<IProcessContext> pipelineProcesses = new List<IProcessContext>();
			listenToPipeline.Subscribe(new TopicEquals(Messages.Exception),
			                           OnPipelineException(listenToPipeline, pipelineProcesses));
			totalDestinations = associationDependencies.Dependencies.Count - associationDependencies.AssociationsByInput.Count;
			StartAllProcesses(contextFactory, listenToPipeline, pipelineProcesses);
		}

		private void StartAllProcesses(
			IProcessContextFactory contextFactory, 
			IProcessContext listenToPipeline, 
			ICollection<IProcessContext> pipelineProcesses)
		{
			using (EnterContext())
			{
				IList<IProcess> dependencies = SortByReverseDependencies(associationDependencies.Dependencies);
				foreach (IProcess process in dependencies)
				{
					List<string> inputNames = GetInputNames(associationDependencies, process);
					RegisterToProcessCompletion(listenToPipeline, associationDependencies, process);

					IProcessContext processContext = contextFactory.Create();
					pipelineProcesses.Add(processContext);
					ExecuteProcess(process, processContext, inputNames);
				}
			}
		}

		private void ExecuteProcess(IProcess process, IProcessContext processContext, List<string> inputNames)
		{
			processContext.Start();
			EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
			processContext.Enqueue(delegate
			{
				try
				{
					using (configurationContext.EnterContext())
					using (EnterContext())
					{
						process.Start(processContext, inputNames.ToArray());
					}
				}
				catch (Exception ex)
				{
					processContext.Publish(Messages.Exception,
					                       new InvalidOperationException(process.Name + " threw an exception", ex));
					processContext.Stop();
				}
			});
		}

		private void RegisterToProcessCompletion(IProcessContext listenToPipeline, AssociationDependencies associationDependencies, IProcess process)
		{
			if (associationDependencies.AssociationsByInput.ContainsKey(process)) // this is destination node
				return;
			listenToPipeline.Subscribe<object>(new TopicEquals(process.Name + Messages.Done),
			                                   delegate
			                                   {
			                                   	completedDestinations += 1;
			                                   	if (completedDestinations == totalDestinations)
			                                   	{
			                                   		Completed(this);
			                                   		listenToPipeline.Stop();
			                                   	}
			                                   });
		}

		private List<string> GetInputNames(AssociationDependencies associationDependencies, IProcess process)
		{
			List<string> inputNames = new List<string>();
			if (associationDependencies.AssociationsByOutput.ContainsKey(process)) // if not, it is an input only
			{
				IList<PipelineAssociation> outputs = associationDependencies.AssociationsByOutput[process];
				foreach (PipelineAssociation output in outputs)
				{
					output.Input.OutputName = output.ToQueue ?? "Output";
					string inputName = output.Input.Name + "." + output.Input.OutputName;
					inputNames.Add(inputName);
				}
			}
			return inputNames;
		}

		private OnMessage<Exception> OnPipelineException(IThreadController listenToPipeline, IEnumerable<IProcessContext> pipelineProcesses)
		{
			return delegate(IMessageHeader header, Exception msg)
			{
				Logger.Fatal("Error when running pipeline " + Name, msg);
				if (stopped)
					return;
				stopped = true;
				foreach (IProcessContext process in pipelineProcesses)
				{
					process.Stop();
				}
				Completed(this);
				listenToPipeline.Stop();
			};
		}

		private AssociationDependencies BuildDependencies()
		{
			AssociationDependencies deps = new AssociationDependencies();
			foreach (PipelineAssociation association in associations)
			{
				if (deps.Dependencies.ContainsKey(association.Output) == false)
					deps.Dependencies.Add(association.Output, new List<IProcess>());
				if (deps.Dependencies.ContainsKey(association.Input) == false)
					deps.Dependencies.Add(association.Input, new List<IProcess>());

				if (deps.AssociationsByOutput.ContainsKey(association.Output) == false)
					deps.AssociationsByOutput.Add(association.Output, new List<PipelineAssociation>());
				if (deps.AssociationsByInput.ContainsKey(association.Input) == false)
					deps.AssociationsByInput.Add(association.Input, new List<PipelineAssociation>());

				deps.AssociationsByOutput[association.Output].Add(association);
				deps.AssociationsByInput[association.Input].Add(association);
				deps.Dependencies[association.Output].Add(association.Input);
			}
			return deps;
		}


		private static IList<IProcess> SortByReverseDependencies(IDictionary<IProcess, IList<IProcess>> deps)
		{
			List<IProcess> orderedList = new List<IProcess>();
			Dictionary<IProcess, bool> visited = new Dictionary<IProcess, bool>();

			foreach (IProcess key in deps.Keys)
			{
				visited[key] = false;
			}

			Proc<IProcess> visit = null;
			visit = delegate(IProcess process)
			{
				if (visited[process])
					return;
				visited[process] = true;
				foreach (IProcess dependingProcess in deps[process])
				{
					visit(dependingProcess);
				}
				orderedList.Insert(0, process);
			};
			foreach (IProcess key in deps.Keys)
			{
				visit(key);
			}

			return orderedList;
		}

		#region Nested type: AssociationDependencies

		public class AssociationDependencies
		{
			public IDictionary<IProcess, IList<PipelineAssociation>> AssociationsByInput = new Dictionary<IProcess, IList<PipelineAssociation>>();
			public IDictionary<IProcess, IList<PipelineAssociation>> AssociationsByOutput = new Dictionary<IProcess, IList<PipelineAssociation>>();
			public IDictionary<IProcess, IList<IProcess>> Dependencies = new Dictionary<IProcess, IList<IProcess>>();
		}

		#endregion
	}
}