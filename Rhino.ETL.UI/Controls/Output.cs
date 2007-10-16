namespace Rhino.ETL.UI.Controls
{
	using System;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;
	using Commons;

	public partial class Output : UserControl
	{
		public Output()
		{
			InitializeComponent();
			Console.SetOut(new TextBoxTextWriter(textBox1));
		}

		#region Nested type: TextBoxTextWriter

		public class TextBoxTextWriter : TextWriter

		{
			protected TextBox theTextBox;

			public TextBoxTextWriter(TextBox textBox)

			{
				theTextBox = textBox;
			}

			public override Encoding Encoding

			{
				get { return Encoding.UTF8; }
			}

			public override void Write(string value)

			{
				if(theTextBox.InvokeRequired)
				{
					theTextBox.BeginInvoke((Proc) delegate
					{
						Write(value);
					});
					return;
				}
				theTextBox.Text = value + theTextBox.Text;
			}

			public override void WriteLine(string value)

			{
				Write(value + "\r\n");
			}
		}

		#endregion
	}
}