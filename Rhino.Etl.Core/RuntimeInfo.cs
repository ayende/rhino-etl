using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rhino.Etl.Core
{
    internal static class RuntimeInfo
    {
        public static string Version
        {
            get
            {
                var asm = Assembly.GetEntryAssembly();
                return asm.ImageRuntimeVersion;
            }
        }
    }
}
