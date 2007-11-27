namespace Rhino.ETL2.Impl
{
	using System;

	internal class ColoredConsole
	{
		public static void WriteLine(ConsoleColor color, string message)
		{
			lock (typeof (ColoredConsole))
			{
				ConsoleColor old = Console.ForegroundColor;
				try
				{
					Console.ForegroundColor = color;
					Console.WriteLine("--------------");
					Console.WriteLine(message);
					Console.WriteLine("--------------");
				}
				finally
				{
					Console.ForegroundColor = old;
				}
			}
		}
	}
}