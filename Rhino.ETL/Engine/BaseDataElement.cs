using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Boo.Lang;
using Boo.Lang.Compiler.MetaProgramming;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public abstract class BaseDataElement<T> : ContextfulObjectBase<T>, IConnectionUser
		where T : BaseDataElement<T>
	{
		private readonly string name;
		private string command;
		private ICallable commandGenerator;
		private string connection;
		private Connection connectionInstance;
		[CLSCompliant(false)]
		protected IDictionary<string, ICallable> commandParameters;


		public BaseDataElement(string name)
		{
			this.name = name;
			commandParameters = new Dictionary<string, ICallable>(StringComparer.InvariantCultureIgnoreCase);
		}

		[CLSCompliant(false)]
		public void AddParameter(string parameterName, ICallable callable)
		{
			if (commandParameters.ContainsKey(parameterName))
			{
				throw new DuplicateKeyException("[Source " + Name + "] already has a parameter called '" + parameterName + "'");
			}
			commandParameters.Add(parameterName, callable);
		}

		public object GetParameterValue(string parameterName)
		{
			if (commandParameters.ContainsKey(parameterName) == false)
			{
				throw new KeyNotFoundException("[Source " + Name + "] does not contains a parameter called '" + parameterName + "'");
			}
			using (EnterContext())
			{
				return commandParameters[parameterName].Call(new object[0]);
			}
		}

		[ReadOnly(true)]
		public string Connection
		{
			get { return connection ?? Name; }
			set { connection = value; }
		}

		[Browsable(false)]
		[CLSCompliant(false)]
		public ICallable CommandGenerator
		{
			get { return commandGenerator; }
			set { commandGenerator = value; }
		}

		public override string Name
		{
			get { return name; }
		}

		public Connection ConnectionInstance
		{
			get { return connectionInstance; }
		}

		public bool TryAcquireConnection(Pipeline pipeline)
		{
			if (CustomActionSpecified)//no need to grab connection
				return true;
			SetDbConnection(pipeline, ConnectionInstance.TryAcquire());
			return GetDbConnection(pipeline) != null;
		}

		protected abstract bool CustomActionSpecified { get; }

		protected IDbConnection GetDbConnection(Pipeline pipeline)
		{
			return (IDbConnection)pipeline.Items[DatabaseConnectionKey];
		}

		protected void SetDbConnection(Pipeline pipeline, IDbConnection dbConnection)
		{
			pipeline.Items[DatabaseConnectionKey] = dbConnection;
		}

		private string DatabaseConnectionKey
		{
			get { return Name + "DatabaseConnection"; }
		}

		public void ReleaseConnection(Pipeline pipeline)
		{
			ConnectionInstance.Release(GetDbConnection(pipeline));
		}

		[ReadOnly(true)]
		public string Command
		{
			get
			{
				if (CommandGenerator != null)
				{
					using (EnterContext())
					{
						return (string)CommandGenerator.Call(new object[0]);
					}
				}
				return command;
			}
			set { command = value; }
		}

		[Meta]
		public void Parameters(ICallable block)
		{
			using (EnterContext())
			{
				block.Call(new object[] { this });
			}
		}

		[Meta]
		public void parameters(ICallable block)
		{
			Parameters(block);
		}

		public void Validate(ICollection<string> messages)
		{
			bool hasConnection = EtlConfigurationContext.Current.Connections.ContainsKey(Connection);
			if (hasConnection == false &&
				CustomActionSpecified==false)//we assume that a block to execute would not use a standard connection
			{
				string msg = string.Format("Could not find connection '{0}' in context '{1}'", Connection, EtlConfigurationContext.Current.Name);
				Logger.WarnFormat("{0} failed validation: {1}", Name, msg);
				messages.Add(msg);
			}
		}

		public void PerformSecondStagePass()
		{
			if (CustomActionSpecified)
				return;
			connectionInstance = EtlConfigurationContext.Current.Connections[Connection];
		}

		protected void AddParameters(IDbCommand dbCommand)
		{
			foreach (KeyValuePair<string, ICallable> pair in commandParameters)
			{
				IDbDataParameter parameter = dbCommand.CreateParameter();
				parameter.ParameterName = pair.Key;
				object value = pair.Value.Call(new object[0]) ?? DBNull.Value;
				parameter.Value = value;
				dbCommand.Parameters.Add(parameter);
			}
		}
	}
}