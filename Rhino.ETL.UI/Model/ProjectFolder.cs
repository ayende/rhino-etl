using System.Collections.Generic;

namespace Rhino.ETL.UI.Model
{
	public class ProjectFolder
	{
		private readonly string name;

		public ProjectFolder(string name)
		{
			this.name = name;
		}

		private readonly List<ProjectFile> files = new List<ProjectFile>();

		public List<ProjectFile> Files
		{
			get { return files; }
		}

		public string Name
		{
			get { return name; }
		}
	}
}