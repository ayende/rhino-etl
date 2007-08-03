using System;

namespace Rhino.ETL
{
	public class TransformationBase<T> : ContextfulObjectBase<T>
		where T : TransformationBase<T>
	{
		protected const string DefaultOutputQueue = "Output";
		protected string name;

		[ThreadStatic]
		public static TransformParameters CurrentTransformParameters;


		public override string Name
		{
			get { return name; }
		}

		public void RemoveRow()
		{
			CurrentTransformParameters.ShouldSkipRow = true;
		}

	}
}