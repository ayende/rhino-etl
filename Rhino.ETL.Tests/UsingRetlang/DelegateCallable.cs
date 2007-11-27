namespace Rhino.ETL2.Tests.UsingRetlang
{
	using Boo.Lang;

	internal class DelegateCallable : ICallable
	{
		private readonly System.Predicate<object[]> predicate;

		public DelegateCallable(System.Predicate<object[]> predicate)
		{
			this.predicate = predicate;
		}


		public object Call(object[] args)
		{
			return predicate(args);
		}
	}
}