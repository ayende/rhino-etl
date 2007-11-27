namespace Rhino.ETL.Cmd
{
	using System;
	using System.Xml;
	using Boo.Lang.Compiler;
	using CommandLine;
	using Engine;
	using Impl;
	using log4net;
	using log4net.Appender;
	using log4net.Config;
	using log4net.Core;
	using log4net.Layout;

	internal class Program
	{
		private static void Main(string[] args)
		{
			ApplicationOptions options = new ApplicationOptions();

			if (Parser.ParseArgumentsWithUsage(args, options) == false)
				Environment.Exit(2);
			if (options.Verbose)
			{
				ColoredConsoleAppender appender = new ColoredConsoleAppender();
				appender.Layout = new PatternLayout(@"%timestamp [%thread] %level %logger - %message %exception %newline");
				AddColorMapping(appender, Level.Error, ColoredConsoleAppender.Colors.Red);
				AddColorMapping(appender, Level.Warn, ColoredConsoleAppender.Colors.Yellow);
				AddColorMapping(appender, Level.Info, ColoredConsoleAppender.Colors.Green);
				AddColorMapping(appender, Level.Debug, ColoredConsoleAppender.Colors.Green);
				appender.ActivateOptions();
				BasicConfigurator.Configure(appender);
			}
			ILog logger = LogManager.GetLogger(typeof(Program));

			logger.Debug("Starting...");
			BulidConfiguration(options);

			try
			{
				DateTime start = DateTime.Now;

				EtlConfigurationContext configurationContext = EtlContextBuilder.FromFile(options.File);
				ExecutionPackage package = configurationContext.BuildPackage();
				ExecutionResult executionResult = package.Execute(options.Target);

				ReportResult(executionResult);

				Console.WriteLine("Execution completed in {0}", DateTime.Now - start);
			}
			catch (CompilerError err)
			{
				Console.WriteLine("Compiler error(s) when evaluating package:");
				Console.Write(err);
				Environment.Exit(1);
			}
			catch (Exception e)
			{
				Console.WriteLine("A fatal error occured, this is a bug:");
				Console.Write(e);
				Environment.Exit(1);
			}
		}

		private static void ReportResult(ExecutionResult executionResult)
		{
			switch (executionResult.Status)
			{
				case ExecutionStatus.Success:
					using (UseConsoleColor(ConsoleColor.Green))
					{
						Console.WriteLine("Execution completed successfully");
					}
					break;
				case ExecutionStatus.Failure:
					using (UseConsoleColor(ConsoleColor.Red))
					{
						Console.WriteLine("Failed to execute package");
						foreach (Exception exception in executionResult.Exceptions)
						{
							Console.WriteLine("---------------");
							Console.WriteLine(exception);
						}
					}
					break;
				case ExecutionStatus.InvalidPackage:
					using (UseConsoleColor(ConsoleColor.Yellow))
					{
						Console.WriteLine("Invalid package");
						foreach (Exception exception in executionResult.Exceptions)
						{
							Console.WriteLine("---------------");
							Console.WriteLine(exception);
						}
					}
					break;
				default:
					break;
			}
		}

		private static void BulidConfiguration(ApplicationOptions options)
		{
			if (string.IsNullOrEmpty(options.Config) == false)
			{
				try
				{
					XmlDocument xdoc = new XmlDocument();
					xdoc.Load(options.Config);
					Configuration qd = new Configuration();
					foreach (XmlNode node in xdoc.FirstChild.ChildNodes)
					{
						qd[node.Name] = node.InnerText;
					}
					Configurable.InitalizeConfiguration(qd);
				}
				catch (Exception e)
				{
					Console.WriteLine("Could not parse configuration file " + options.Config);
					Console.WriteLine(e);
					Environment.Exit(3);
				}
			}
		}

		private static void AddColorMapping(ColoredConsoleAppender appender, Level level, ColoredConsoleAppender.Colors color)
		{
			ColoredConsoleAppender.LevelColors mapping = new ColoredConsoleAppender.LevelColors();
			mapping.Level = level;
			mapping.ForeColor = color;
			appender.AddMapping(mapping);
		}

		public static ChangeConsoleColor UseConsoleColor(ConsoleColor color)
		{
			return new ChangeConsoleColor(color);
		}

		public class ChangeConsoleColor : IDisposable
		{
			private readonly ConsoleColor old;

			public ChangeConsoleColor(ConsoleColor color)
			{
				this.old = Console.ForegroundColor;
				Console.ForegroundColor = color;
			}


			public void Dispose()
			{
				Console.ForegroundColor = old;
			}
		}
	}
}