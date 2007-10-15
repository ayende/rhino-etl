namespace Rhino.ETL.Engine
{
	using System;
	using Boo.Lang;
	using Boo.Lang.Runtime;

	public class ProxyInstance : IQuackFu
	{
		private readonly object instance;

		public ProxyInstance(WebService srv, string type)
		{
			Type serviceType = srv.Instance.GetType();
			Type typeToCreate = serviceType.Assembly.GetType(type);
			if(typeToCreate==null)
				typeToCreate = serviceType.Assembly.GetType( serviceType.Namespace + "." + type);
			if (typeToCreate == null)
				throw new ArgumentException("type", "Could not find type " + type);
			instance = Activator.CreateInstance(typeToCreate);
		}

		public object QuackGet(string name, object[] parameters)
		{
			return RuntimeServices.GetProperty(instance, name);
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			return RuntimeServices.SetProperty(instance, name, value);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return RuntimeServices.Invoke(instance, name, args);
		}
	}
}