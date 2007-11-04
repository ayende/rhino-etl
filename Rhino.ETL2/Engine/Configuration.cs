namespace Rhino.ETL.Engine
{
	using System.Configuration;
	using Impl;

	public class Configuration : QuackingDictionary
	{
		public override object QuackGet(string name, object[] parameters)
		{
			if (items.Contains(name) == false)
				throw new ConfigurationErrorsException("Could not find configuration item " + name);
			return base.QuackGet(name, parameters);
		}
	}
}