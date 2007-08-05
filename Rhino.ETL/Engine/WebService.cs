using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Boo.Lang;
using Boo.Lang.Runtime;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Engine
{
	[WebServiceBinding()]
	public class WebService : IQuackFu
	{
		private string wsdlUrl;
		private object instance;

		public object Instance
		{
			get
			{
				if (instance == null)
					instance = CreateInstance();
				return instance;
			}
		}

		public string WsdlUrl
		{
			get { return wsdlUrl; }
			set { wsdlUrl = value; }
		}

		public object QuackGet(string name, object[] parameters)
		{
			if ("WsdlUrl".Equals(name, StringComparison.InvariantCultureIgnoreCase))
				return WsdlUrl;
			return RuntimeServices.GetProperty(Instance, name);
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			if ("WsdlUrl".Equals(name, StringComparison.InvariantCultureIgnoreCase))
				WsdlUrl = (string) value;
			return RuntimeServices.SetProperty(Instance, name, value);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return RuntimeServices.Invoke(Instance, name, args);
		}


		private object CreateInstance()
		{
			if (string.IsNullOrEmpty(WsdlUrl))
			{
				throw new InvalidWebServiceException("You must specify the 'WsdlUrl' for the web service");
			}

			Uri uri = new Uri(WsdlUrl);

			WebRequest webRequest = WebRequest.Create(uri);

			ServiceDescription sd;

			try
			{
				using (WebResponse response = webRequest.GetResponse())
				using (Stream requestStream = response.GetResponseStream())
					sd = ServiceDescription.Read(requestStream);
			}
			catch (WebException e)
			{
				using(Stream stream = e.Response.GetResponseStream())
				{
					StreamReader sr = new StreamReader(stream);
					throw new InvalidWebServiceException("Could not get WSDL for url '" + WsdlUrl + "'. Server reply was: "+ sr.ReadToEnd(), e);
				}
			}
			catch (Exception e)
			{
				throw new InvalidWebServiceException("Could not get WSDL for url '" + WsdlUrl + "'", e);
			}

			string sdName = sd.Services[0].Name;

			ServiceDescriptionImporter servImport = new ServiceDescriptionImporter();
			servImport.AddServiceDescription(sd, String.Empty, String.Empty);
			servImport.ProtocolName = "Soap";
			servImport.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;

			CodeNamespace nameSpace = new CodeNamespace();
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(nameSpace);
			ServiceDescriptionImportWarnings warnings = servImport.Import(nameSpace, codeCompileUnit);

			if (warnings != 0)
				throw new InvalidWebServiceException("Could not generate proxy from '" + WsdlUrl + "' because: " + warnings);

			StringWriter stringWriter = new StringWriter(System.Globalization.CultureInfo.CurrentCulture);
			Microsoft.CSharp.CSharpCodeProvider prov = new Microsoft.CSharp.CSharpCodeProvider();
			prov.GenerateCodeFromNamespace(nameSpace, stringWriter, new CodeGeneratorOptions());
			CompilerParameters param = new CompilerParameters(new string[]
			                                                  	{
			                                                  		"System.Web.Services.dll",
			                                                  		"System.Xml.dll"
			                                                  	});

			param.GenerateExecutable = false;
			param.GenerateInMemory = true;
			param.TreatWarningsAsErrors = false;
			param.WarningLevel = 4;
			CompilerResults results = prov.CompileAssemblyFromDom(param, codeCompileUnit);
			if (results.Errors.Count != 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Could not generate proxy for web service at: '{0}' because of the following errors:", WsdlUrl)
					.AppendLine();
				foreach (CompilerError error in results.Errors)
				{
					sb.AppendLine(error.ToString());
				}
				throw new InvalidWebServiceException(sb.ToString());
			}
			Assembly assembly = results.CompiledAssembly;

			return Activator.CreateInstance(assembly.GetType(sdName));
		}
	}
}