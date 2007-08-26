namespace Rhino.ETL.Cmd
{
    using CommandLine;

    public class ApplicationOptions
    {
        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify the target to run. Defaults to ... 'default'", ShortName = "t", DefaultValue = "default")]
        public string Target;

        [Argument(ArgumentType.Required, HelpText = "Specify the .retl file to execute", ShortName = "f")]
        public string File;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Show log4net output to console.", ShortName = "v", DefaultValue = false)]
        public bool Verbose;
    }
}