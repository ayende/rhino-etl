using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rhino.ETL.UI.Commands;

namespace Rhino.ETL.UI
{
	public partial class MainGui : Form
	{
		public MainGui()
		{
			InitializeComponent();
			CommandDispatcher.Initialize(this);
		}
	}
}