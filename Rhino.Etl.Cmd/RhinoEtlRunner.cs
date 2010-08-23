namespace Rhino.Etl.Cmd
{
    using System;
    using System.IO;
    using Core;
    using log4net;
    using log4net.Config;

    public class RhinoEtlRunner : MarshalByRefObject
    {
        private readonly ILog log = LogManager.GetLogger(typeof (RhinoEtlRunner));

        public static void SetupLogging(bool verbose)
        {
            string configurationName = "Rhino.Etl.Cmd.standard.log4net.config";
            if (verbose)
                configurationName = "Rhino.Etl.Cmd.verbose.log4net.config";
            using (Stream stream = typeof(RhinoEtlSetup).Assembly.GetManifestResourceStream(configurationName))
                XmlConfigurator.Configure(stream);
        }

        public void Start(Type type, bool verboseLogging)
        {
            try
            {
                SetupLogging(verboseLogging);
                EtlProcess process = (EtlProcess)Activator.CreateInstance(type);
                process.Execute();
                foreach (Exception error in process.GetAllErrors())
                {
                    log.Debug(error);
                    log.Error(error.Message);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }
    }
}