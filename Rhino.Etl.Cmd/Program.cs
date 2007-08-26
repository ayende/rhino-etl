namespace Rhino.ETL.Cmd
{
    using System;
    using CommandLine;
    using Rhino.ETL;

    class Program
    {
        static void Main(string[] args)
        {
            ApplicationOptions options = new ApplicationOptions();

            if (Parser.ParseArgumentsWithUsage(args, options) == false)
                Environment.Exit(2);

            if(options.Verbose) log4net.Config.BasicConfigurator.Configure();

            try
            {
                EtlConfigurationContext configurationContext = EtlContextBuilder.FromFile(options.File);
                ExecutionPackage package = configurationContext.BuildPackage();
                package.Execute(options.Target);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured:");
                Console.Write(e);
            }
        }
    }
}
