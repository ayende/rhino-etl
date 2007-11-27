using System;
using System.IO;

namespace Rhino.ETL.Tests
{
	using Engine;

	public class BaseTest
    {
        /// <summary>
        /// Builds the context.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        protected static EtlConfigurationContext BuildContext(string file)
        {
            return EtlContextBuilder.FromFile(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    file
                    ));
        }
    }
}