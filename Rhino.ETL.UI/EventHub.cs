using System;
using System.Collections.Generic;
using System.Text;
using Rhino.ETL.UI.Model;

namespace Rhino.ETL.UI
{
	public class EventHub
	{
		public static event Action<object> SelectedLiveViewItemChanged = delegate { };
		public static event Action<RetlProject> ProjectChanged = delegate { };

		public static void RaiseProjectChanged(RetlProject project)
		{
			ProjectChanged(project);
		}

		public static void RaiseItemChanged(object selectedItem)
		{
			SelectedLiveViewItemChanged(selectedItem);
		}
	}
}
