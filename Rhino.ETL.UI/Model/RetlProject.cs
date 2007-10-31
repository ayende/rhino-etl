using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;

namespace Rhino.ETL.UI.Model
{
	using Engine;

	public class RetlProject
	{

		private static RetlProject instance = RetlProject.CreateDefault();
		private readonly List<InputSource> sources = new List<InputSource>();
		private string name;
		private string directory;
		private EtlConfigurationContext configurationContext;
		private readonly List<ProjectFolder> folders = new List<ProjectFolder>();

		private RetlProject()
		{
		}

		public void Build()
		{
			List<ICompilerInput> inputs = new List<ICompilerInput>();
			foreach (InputSource source in sources)
			{
				ICompilerInput input = source.CompilerInput;
				if(input!=null)
					inputs.Add(input);
			}
			configurationContext = EtlContextBuilder.From(directory, name, inputs.ToArray());
			EventHub.RaiseProjectChanged(this);
		}


		public EtlConfigurationContext ConfigurationContext
		{
			get { return configurationContext; }
			set { configurationContext = value; }
		}

		public List<InputSource> Sources
		{
			get { return sources; }
		}

		public static RetlProject Instance
		{
			get { return instance;}
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public ICollection<ProjectFolder> Folders
		{
			get { return folders; }
		}

		public static RetlProject CreateFrom(string file)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(file);
			instance = new RetlProject();
			instance.directory = Path.GetDirectoryName(file);
			XmlNode projectNode = xdoc.SelectSingleNode("/project/@name");
			if (projectNode != null)
				instance.name = projectNode.Value;
			else
				instance.name = "Unamed Project";

			foreach (XmlNode node in xdoc.SelectNodes("/project/sources/file"))
			{
				XmlAttribute attribute = node.Attributes["path"];
				if(attribute!=null)
					instance.AddFile(attribute.Value);
			}
			return instance;
		}

		public void AddFile(string value)
		{
			sources.Add(new InputSource(null, new FileInfo(value)));
		}

		public InputSource GetSourceFor(string fileName)
		{
			string path = Path.GetFullPath(fileName);
			foreach (InputSource file in sources)
			{
				if(file.FileInfo != null && 
					file.FileInfo.FullName.Equals(path,StringComparison.InvariantCultureIgnoreCase))
					return file;
			}
			InputSource source = new InputSource(null, new FileInfo(fileName));
			sources.Add(source);
			return source;
		}

		public InputSource AddDocument(Document document)
		{
			InputSource source = new InputSource(document, null);
			sources.Add(source);
			return source;
		}

		private static RetlProject CreateDefault()
		{
			RetlProject proj = new RetlProject();
			proj.name = "Rhino ETL Project";
			proj.Folders.Add(new ProjectFolder("Connections"));
			proj.Folders.Add(new ProjectFolder("Sources"));
			proj.Folders.Add(new ProjectFolder("Transforms"));
			proj.Folders.Add(new ProjectFolder("Destinations"));
			proj.Folders.Add(new ProjectFolder("Pipelines"));
			proj.Folders.Add(new ProjectFolder("Targets"));
			return proj;
		}

		public ExecutionResult Execute()
		{
			ExecutionPackage package = configurationContext.BuildPackage();
			return package.Execute("default");
		}
	}
}