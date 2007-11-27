namespace Rhino.ETL.Engine
{
	using System.Collections;
	using System.Configuration;
	using Impl;

	public abstract class Configurable
	{
		protected static Configuration Configuration = new Configuration();

		public static void InitalizeConfiguration(Configuration configuration)
		{
			Configuration = configuration;
		}
	}
}