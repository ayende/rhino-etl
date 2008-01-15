namespace Rhino.Etl.Cmd
{
    using System;
    using Boo.Lang.Useful.CommandLine;

    [Serializable]
    public class RhinoEtlCommandLineOptions : AbstractCommandLine
    {
        [Option("File name", ShortForm = "f", MinOccurs = 1, MaxOccurs = 1)]
        public string File;
        [Option("Process name", ShortForm = "p", MinOccurs = 0, MaxOccurs = 1)]
        public string Process;
        [Option("Verbse logging", ShortForm = "v", MaxOccurs = 1)]
        public bool Verbose = false;
    }
}