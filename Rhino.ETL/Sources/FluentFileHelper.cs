using System;
using System.Collections;
using FileHelpers;

namespace Rhino.ETL
{
	public class FluentFileHelper
	{
		private FileHelperAsyncEngine engine;

		public FluentFileHelper(Type type)
		{
			engine = new FileHelpers.FileHelperAsyncEngine(type);
		}

		public IEnumerable From(string filename)
		{
			engine.BeginReadFile(filename);
			return engine;
		}
	}
}