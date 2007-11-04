namespace Rhino.ETL.Engine
{
	using System.Collections.Generic;

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
	
		}

		//public void Start(Target target)
		//{
		//    if (associations.Count == 0)
		//    {
		//        Completed(this);
		//        return;
		//    }
		//    if (AcquireAllConnections(target) == false)
		//        return;
		//    foreach (PipelineAssociation association in associations)
		//    {
		//        association.ConnectEnds(target, this);
		//    }
		//    foreach (PipelineAssociation association in associations)
		//    {
		//        association.Completed += AssociationCompleted;
		//    }
		//    foreach (DataSource value in EtlConfigurationContext.Current.Sources.Values)
		//    {
		//        DataSource cSharpSpec_21_5_2_Damn_It = value; 
		//        ExecutionPackage.Current.RegisterForExecution(target,
		//                                                      delegate { cSharpSpec_21_5_2_Damn_It.Start(this); }
		//            );
		//    }
		//}

	}
}