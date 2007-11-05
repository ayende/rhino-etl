namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections.Generic;
	using Commons;
	using Interfaces;
	using Retlang;

	public class Pipeline : ContextfulObjectBase<Pipeline>
	{
		private readonly IList<PipelineAssociation> associations = new List<PipelineAssociation>();

		public delegate void PipelineCompleted(Pipeline completed);

		public event PipelineCompleted Completed = delegate { };

		private readonly string name;

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
			IDictionary<IProcess, IList<IProcess>> deps;
			Dictionary<IProcess, IList<PipelineAssociation>> associationsByOutput;
			Dictionary<IProcess, IList<PipelineAssociation>> associationsByInput;
			BuildDependencies(out deps, out associationsByOutput, out associationsByInput);
			IProcessContext listenToPipeline = contextFactory.CreateAndStart();
			List<IProcessContext> pipelineProcesses = new List<IProcessContext>();
			bool stopped = false;
			listenToPipeline.Subscribe<Exception>(new TopicEquals(Messages.Exception),
												delegate(IMessageHeader header, Exception msg)
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
												});
			int completedDestinations = 0;
			int totalDestinations = deps.Count - associationsByInput.Count;
			using (EnterContext())
			{
				IList<IProcess> dependencies = SortByReverseDependencies(deps);
				foreach (IProcess process in dependencies)
				{
					List<string> inputNames = new List<string>();
					if (associationsByOutput.ContainsKey(process)) // if not, it is an input only
					{
						IList<PipelineAssociation> outputs = associationsByOutput[process];
						foreach (PipelineAssociation output in outputs)
						{
							output.Input.OutputName = output.ToQueue ?? "Output";
							string inputName = output.Input.Name + "." + output.Input.OutputName;
							inputNames.Add(inputName);
						}
					}
					if (associationsByInput.ContainsKey(process) == false)// this is destination node
					{
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

					IProcessContext processContext = contextFactory.Create();
					pipelineProcesses.Add(processContext);
					processContext.Start();
					EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
					IProcess tempToGoAroundForEachVar = process;
					processContext.Enqueue(delegate
					{
						try
						{
							using(configurationContext.EnterContext())
							using(this.EnterContext())
							{
								tempToGoAroundForEachVar.Start(processContext, inputNames.ToArray());
							}
						}
						catch (Exception ex)
						{
							processContext.Publish(Messages.Exception, 
								new InvalidOperationException(process.Name +" threw an exception", ex));
							processContext.Stop();
						}
					});
				}
			}
		}

		private void BuildDependencies(
			out IDictionary<IProcess, IList<IProcess>> deps,
			out Dictionary<IProcess, IList<PipelineAssociation>> associationsByOutput,
			out Dictionary<IProcess, IList<PipelineAssociation>> associationsByInput)
		{
			deps = new Dictionary<IProcess, IList<IProcess>>();
			associationsByOutput = new Dictionary<IProcess, IList<PipelineAssociation>>();
			associationsByInput = new Dictionary<IProcess, IList<PipelineAssociation>>();
			foreach (PipelineAssociation association in associations)
			{
				if (deps.ContainsKey(association.Output) == false)
					deps.Add(association.Output, new List<IProcess>());
				if (deps.ContainsKey(association.Input) == false)
					deps.Add(association.Input, new List<IProcess>());

				if (associationsByOutput.ContainsKey(association.Output) == false)
					associationsByOutput.Add(association.Output, new List<PipelineAssociation>());
				if (associationsByInput.ContainsKey(association.Input) == false)
					associationsByInput.Add(association.Input, new List<PipelineAssociation>());

				associationsByOutput[association.Output].Add(association);
				associationsByInput[association.Input].Add(association);
				deps[association.Output].Add(association.Input);
			}
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
	}
}