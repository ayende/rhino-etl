namespace Rhino.ETL2.Tests
{
	using System.Collections;
	using Boo.Lang;
	using Rhino.ETL.Engine;

	public class PutInSyncList : ICallable
	{
		public IList List = ArrayList.Synchronized(new ArrayList());

		public object Call(object[] args)
		{
			Row dic = args[1] as Row;
			List.Add(dic["id"]);
			return null;
		}
	}
}