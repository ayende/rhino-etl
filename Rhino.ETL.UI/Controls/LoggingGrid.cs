using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rhino.ETL.UI.Controls
{
	using Commons;
	using log4net.Appender;
	using log4net.Config;
	using log4net.Core;

	public partial class LoggingGrid : UserControl
	{
		public LoggingGrid()
		{
			InitializeComponent();

			BasicConfigurator.Configure(new GridAppender("grid-appender",this));
			Message.Width = -2;
			Exception.Width = -2;
		}

		public void AddLog(LoggingEvent loggingEvent)
		{
			if (InvokeRequired)
			{
				BeginInvoke((Proc)delegate
				{
					AddLog(loggingEvent);
				});
				return;
			}
			AddLogInUIThread(loggingEvent);
		}

		private void AddLogInUIThread(LoggingEvent loggingEvent)
		{
			ListViewItem value = new ListViewItem(
				new string[]
					{
						loggingEvent.TimeStamp.ToShortTimeString(),
						loggingEvent.MessageObject.ToString(),
						loggingEvent.Level.DisplayName,
						((object)loggingEvent.ExceptionObject ?? "").ToString()
					}
				);
			if(loggingEvent.Level.Value >= log4net.Core.Level.Warn.Value)
			{
				value.ForeColor = Color.Red;
			}
			grid.Items.Add(value);
		}

		public class GridAppender : IAppender
		{
			private string name;
			private readonly LoggingGrid grid;


			public GridAppender(string name, LoggingGrid grid)
			{
				this.name = name;
				this.grid = grid;
			}

			public void Close()
			{

			}

			public void DoAppend(LoggingEvent loggingEvent)
			{
				grid.AddLog(loggingEvent);
			}

			public string Name
			{
				get { return name; }
				set { name = value; }
			}
		}
	}

}
