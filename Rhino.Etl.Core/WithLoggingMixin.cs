namespace Rhino.Etl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using log4net;
    using log4net.Config;
    using log4net.Core;
    using log4net.Util;

    /// <summary>
    /// A base class that expose easily logging events
    /// </summary>
    public class WithLoggingMixin
    {
        private readonly ILog log;
        readonly List<Exception> errors = new List<Exception>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WithLoggingMixin"/> class.
        /// </summary>
        protected WithLoggingMixin()
        {
            log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Error(Exception exception, string format, params object[] args)
        {
            SystemStringFormat message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args);
            string errorMessage;
            if(exception!=null)
                errorMessage = string.Format("{0}: {1}", message, exception.Message);
            else
                errorMessage = message.ToString();
            errors.Add(new RhinoEtlException(errorMessage, exception));
            if (log.IsErrorEnabled)
            {
                log.Logger.Log(GetType(), Level.Error, message, exception);
            }
        }

        /// <summary>
        /// Logs a warn message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Warn(string format, params object[] args)
        {
            if (log.IsWarnEnabled)
            {
                log.Logger.Log(GetType(), Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Debug(string format, params object[] args)
        {
            if (log.IsDebugEnabled)
            {
                log.Logger.Log(GetType(), Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

		
        /// <summary>
        /// Logs a notice message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Notice(string format, params object[] args)
        {
            if (log.Logger.IsEnabledFor(Level.Notice))
            {
                log.Logger.Log(GetType(), Level.Notice, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }


        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Info(string format, params object[] args)
        {
            if (log.IsInfoEnabled)
            {
                log.Logger.Log(GetType(), Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        /// <summary>
        /// Gets all the errors
        /// </summary>
        /// <value>The errors.</value>
        public Exception[] Errors
        {
            get { return errors.ToArray(); }
        }
    }
}