using System;
using System.Collections.Generic;
using Rhino.ETL.Exceptions;
using System.Threading;

namespace Rhino.ETL
{
	public class Pipeline : ContextfulObjectBase<Pipeline>
	{
		private IList<PipelineAssociation> associations = new List<PipelineAssociation>();
		public delegate void PipelineCompleted(Pipeline completed);
		public event PipelineCompleted Completed = delegate { };

		private TimeSpan timeOut = TimeSpan.FromSeconds(30);
		private string name;
		private Semaphore destinationToComplete;

		public Pipeline(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddPipeline(name, this);
		}


		public TimeSpan TimeOut
		{
			get { return timeOut; }
			set { timeOut = value; }
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

		private IEnumerable<T> GetFromAssoicationsAll<T>()
		{
			foreach (PipelineAssociation association in associations)
			{
				if (association.FromInstance is T)
					yield return (T) association.FromInstance;
				if (association.ToInstance is T)
					yield return (T) association.ToInstance;
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
				EnsureCanGetAllConnections();
			}
		}

		private void EnsureCanGetAllConnections()
		{
			Dictionary<Connection, int> connectionCount = new Dictionary<Connection, int>();
			foreach (IConnectionUser useConnection in GetFromAssoicationsAll<IConnectionUser>())
			{
				if (connectionCount.ContainsKey(useConnection.ConnectionInstance) == false)
					connectionCount.Add(useConnection.ConnectionInstance, 0);
				connectionCount[useConnection.ConnectionInstance] += 1;
			}
			//Can't do it in validation stage, since this require that we will have a fully
			//connected graph
			foreach (KeyValuePair<Connection, int> pair in connectionCount)
			{
				if (pair.Key.ConcurrentConnections < pair.Value)
				{
					throw new TooManyConcurrentConnectionsRequiredException(
						string.Format("Pipeline '{0}' requires {1} concurrent connections from '{2}', but limit is {3}",
						              Name, pair.Value, pair.Key.Name, pair.Key.ConcurrentConnections));
				}
			}
		}

		public void Prepare()
		{
			int destinationCount = EtlConfigurationContext.Current.Destinations.Count;
			destinationToComplete = new Semaphore(0, destinationCount);
		}

		public void Start()
		{
			if (AcquireAllConnections() == false)
				return;
			foreach (PipelineAssociation association in associations)
			{
			    association.ConnectEnds();
			}
			foreach (DataDestination value in EtlConfigurationContext.Current.Destinations.Values)
			{
				value.Completed += DestinationCompleted;
			}

			foreach (DataSource value in EtlConfigurationContext.Current.Sources.Values)
			{
				ExecutionPackage.Current.RegisterForExecution(value.Start);
			}
		}

		private void DestinationCompleted(DataDestination destination)
		{
			int count = destinationToComplete.Release();
			if (count == 0)
				Completed(this);
		}


		private bool AcquireAllConnections()
		{
			List<IConnectionUser> aquiredConnection = new List<IConnectionUser>();

			foreach (IConnectionUser connectionUser in GetFromAssoicationsAll<IConnectionUser>())
			{
				if (connectionUser.TryAcquireConnection())
				{
					aquiredConnection.Add(connectionUser);
				}
				else
				{
					Logger.WarnFormat(
						"Could not aquired all connections in pipeline '{0}', will retry when the next pipeline completes", Name);
					foreach (IConnectionUser user in aquiredConnection)
					{
						user.ReleaseConnection();
					}
					ExecutionPackage.Current.ExecuteOnPipelineCompleted(delegate { Start(); });
					return false;
				}
			}
			return true;
		}

		public void WaitOne()
		{
			if(destinationToComplete.WaitOne(TimeOut, false)==false)
			{
				throw new TimeoutException("Waited for " + TimeOut + " for pipeline to completed, aborting");
			}
		}
	}
}