namespace Rhino.Etl.Tests.Aggregation
{
	using System;
	using System.IO;
	using Core;
	using Rhino.Etl.Dsl;

	public class BaseDslTest
	{
		protected static EtlProcess CreateDslInstance(string url)
		{
			return EtlDslEngine.Factory.Create<EtlProcess>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,url));
		}
	}
}