namespace Rhino.Etl.Cmd
{
    using System;
    using System.IO;
    using System.Reflection;
    using Boo.Lang.Useful.CommandLine;
    using Core;
    using log4net;
    using log4net.Config;
    using Dsl;

    public class RhinoEtlSetup
    {
        private readonly ILog log = LogManager.GetLogger(typeof(RhinoEtlSetup));


        private static void Main(string[] args)
        {
            try
            {
                RhinoEtlCommandLineOptions options = new RhinoEtlCommandLineOptions();
                try
                {
                    options.Parse(args);
                }
                catch (CommandLineException e)
                {
                    Console.WriteLine(e.Message);
                    options.PrintOptions();
                    return;
                }
                new RhinoEtlSetup().Execute(options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Execute(RhinoEtlCommandLineOptions options)
        {
            RhinoEtlRunner.SetupLogging(options.Verbose);

            log.DebugFormat("Starting with {0}", options.File);
            try
            {
                string ext = Path.GetExtension(options.File).ToLower();
                Type processType;
                if(ext==".exe" || ext==".dll")
                {
                    processType = GetFromAssembly(options);
                }
                else
                {
                    processType = GetFromDslFile(options.File);
                }

                ExecuteProcessInSeparateAppDomain(processType, options);
            }
            catch (Exception e)
            {
                log.Debug(e);
                log.Error(e.Message);
            }
        }

        private static Type GetFromAssembly(RhinoEtlCommandLineOptions options)
        {
            FileInfo _assemblyInfo = new FileInfo(options.File);
            Assembly asm = Assembly.LoadFile(_assemblyInfo.FullName);
            //Assembly asm = Assembly.Load(options.File);
            foreach (Type type in asm.GetTypes())
            {
                if(typeof(EtlProcess).IsAssignableFrom(type) && type.Name.Equals(options.Process, StringComparison.InvariantCultureIgnoreCase))
                    return type;
            }
            throw new InvalidOperationException("Could not find type nameed '" + options.Process + "' on: " +
                                                options.File);
        }

        private static Type GetFromDslFile(string filename)
        {
            Type processType;
            EtlProcess process = EtlDslEngine.Factory.Create<EtlProcess>(filename);
            processType = process.GetType();
            return processType;
        }

        private void ExecuteProcessInSeparateAppDomain(Type processType, RhinoEtlCommandLineOptions options)
        {
            try
            {
                FileInfo _assemblyInfo = new FileInfo(options.File);
                //we have to run the code in another appdomain, because we want to
                //setup our own app.config for it
                AppDomainSetup appDomainSetup = new AppDomainSetup();
                appDomainSetup.ApplicationBase = _assemblyInfo.DirectoryName;
                appDomainSetup.ConfigurationFile = options.File + ".config";
                AppDomain appDomain = AppDomain.CreateDomain("etl.domain", null, appDomainSetup);
                appDomain.Load(processType.Assembly.GetName());
                RhinoEtlRunner runner = (RhinoEtlRunner)appDomain.CreateInstanceAndUnwrap(typeof (RhinoEtlRunner).Assembly.GetName().FullName,
                                                                                          typeof (RhinoEtlRunner).FullName);
                runner.Start(processType, options.Verbose);
            }
            catch (Exception e)
            {
                log.Debug(e);
                log.Error(e.Message);
            }
        }
    }
}
