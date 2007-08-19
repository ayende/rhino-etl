namespace Rhino.ETL.UI.Model
{
	public class ProjectFile
	{
		private readonly string name;

		public string Name
		{
			get { return name; }
		}

		public ProjectFile(string name)
		{
			this.name = name;
		}
	}
}