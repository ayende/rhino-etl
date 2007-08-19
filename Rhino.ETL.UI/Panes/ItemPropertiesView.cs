using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;

namespace Rhino.ETL.UI.Panes
{
	public partial class ItemPropertiesView : DockContent
	{
		public ItemPropertiesView()
		{
			InitializeComponent();
			EventHub.SelectedLiveViewItemChanged += EventHub_SelectedLiveViewItemChanged;
		}

		void EventHub_SelectedLiveViewItemChanged(object obj)
		{
			PropertyGrid.SelectedObject = obj;
		}
	}
}