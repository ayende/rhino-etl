namespace Rhino.ETL.Engine
{
	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Net;
	using System.Reflection;
	using System.Text;
	using System.Web.Services;
	using System.Web.Services.Description;
	using System.Xml.Serialization;
	using Boo.Lang;
	using Boo.Lang.Runtime;
	using Exceptions;
	using Microsoft.CSharp;

	[WebServiceBinding()]
	public class WebService : IQuackFu
	{
		private static Dictionary<string, KeyValuePair<Assembly, string>> urlToAssembliesCache = new Dictionary<string, KeyValuePair<Assembly, string>>();

		private ICredentials credentials;
		private object instance;
		private string wsdlUrl;

		public object Instance
		{
			get
			{
				if (instance == null)
					instance = CreateInstance();
				return instance;
			}
		}

		public ICredentials Credentials
		{
			get { return credentials; }
			set { credentials = value; }
		}

		public string WsdlUrl
		{
			get { return wsdlUrl; }
			set { wsdlUrl = value; }
		}

		#region IQuackFu Members

		public object QuackGet(string name, object[] parameters)
		{
			if ("WsdlUrl".Equals(name, StringComparison.InvariantCultureIgnoreCase))
				return WsdlUrl;
			return RuntimeServices.GetProperty(Instance, name);
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			if ("WsdlUrl".Equals(name, StringComparison.InvariantCultureIgnoreCase))
				WsdlUrl = (string)value;
			return RuntimeServices.SetProperty(Instance, name, value);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return RuntimeServices.Invoke(Instance, name, args);
		}

		#endregion

		private object CreateInstance()
		{
			if (string.IsNullOrEmpty(WsdlUrl))
			{
				throw new InvalidWebServiceException("You must specify the 'WsdlUrl' for the web service");
			}


			object createdInstance = GetCreatedInstance();

			createdInstance.GetType()
				.GetProperty("Credentials")
				.SetValue(createdInstance, Credentials, null);
			return createdInstance;
		}

		private object GetCreatedInstance()
		{
			KeyValuePair<Assembly, string> value;
			if (urlToAssembliesCache.TryGetValue(WsdlUrl, out value) == false)
			{
				lock (urlToAssembliesCache)
				{
					if (urlToAssembliesCache.TryGetValue(WsdlUrl, out value) == false)
					{
						string serviceName;
						Assembly tmpAssembly = GetWsdlAndCreateAssembly(out serviceName);
						value = new KeyValuePair<Assembly, string>(tmpAssembly, serviceName);
						urlToAssembliesCache.Add(WsdlUrl, value);
					}
				}
			}

			Assembly asm = value.Key;
			string srvName = value.Value;
			return Activator.CreateInstance(asm.GetType(srvName));
		}

		private Assembly GetWsdlAndCreateAssembly(out string sdName)
		{
			Uri uri = new Uri(WsdlUrl);

			WebRequest webRequest = WebRequest.Create(uri);
			webRequest.Credentials = Credentials;
			ServiceDescription sd;

			try
			{
				using (WebResponse response = webRequest.GetResponse())
				using (Stream requestStream = response.GetResponseStream())
					sd = ServiceDescription.Read(requestStream);
			}
			catch (WebException e)
			{
				using (Stream stream = e.Response.GetResponseStream())
				{
					StreamReader sr = new StreamReader(stream);
					throw new InvalidWebServiceException("Could not get WSDL for url '" + WsdlUrl + "'. Server reply was: " + sr.ReadToEnd(), e);
				}
			}
			catch (Exception e)
			{
				throw new InvalidWebServiceException("Could not get WSDL for url '" + WsdlUrl + "'", e);
			}

			sdName = sd.Services[0].Name;

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

			StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture);
			CSharpCodeProvider prov = new CSharpCodeProvider();
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
			return results.CompiledAssembly;
		}
	}
}