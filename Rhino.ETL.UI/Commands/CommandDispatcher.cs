using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Rhino.ETL.UI.Commands
{
	public static class CommandDispatcher
	{
		private static readonly Dictionary<string, AbstractUICommand> commands =
			new Dictionary<string, AbstractUICommand>();

		public static void Initialize(Form parent)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (typeof(AbstractUICommand).IsAssignableFrom(type) && type.IsAbstract == false)
				{
					commands.Add(
						type.Name,
						(AbstractUICommand)Activator.CreateInstance(type, parent)
						);
				}
			}
		}

		public static void Dispatch(string name)
		{
			AbstractUICommand command;
			if (commands.TryGetValue(name, out command) == false)
			{
				MessageBox.Show("Not command for " + name);
				return;
			}
			command.Execute();
		}
	}
}
