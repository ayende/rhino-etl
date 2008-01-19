namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Collections;
    using System.Data;
    using Commons;

    /// <summary>
    /// Represent an operation that uses the database can occure during the ETL process
    /// </summary>
    public abstract class AbstractDatabaseOperation : AbstractOperation
    {
        private readonly string connectionStringName;
    	private static Hashtable supportedTypes;
		///<summary>
		///The parameter prefix to use when adding parameters
		///</summary>
		protected string paramPrefix = "";
    	/// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName
        {
            get { return connectionStringName; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected AbstractDatabaseOperation(string connectionStringName)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(connectionStringName),
                                             "Connection string name must have a value");
            this.connectionStringName = connectionStringName;
        }

		 private static void InitializeSupportedTypes()
        {
            supportedTypes = new Hashtable();
            supportedTypes[typeof (byte[])] = typeof (byte[]);
			supportedTypes[typeof (Guid)] = typeof (Guid);
            supportedTypes[typeof (Object)] = typeof (Object);
            supportedTypes[typeof (Boolean)] = typeof (Boolean);
            supportedTypes[typeof (SByte)] = typeof (SByte);
            supportedTypes[typeof (SByte)] = typeof (SByte);
            supportedTypes[typeof (Byte)] = typeof (Byte);
            supportedTypes[typeof (Int16)] = typeof (Int16);
            supportedTypes[typeof (UInt16)] = typeof (UInt16);
            supportedTypes[typeof (Int32)] = typeof (Int32);
            supportedTypes[typeof (UInt32)] = typeof (UInt32);
            supportedTypes[typeof (Int64)] = typeof (Int64);
            supportedTypes[typeof (UInt64)] = typeof (UInt64);
            supportedTypes[typeof (Single)] = typeof (Single);
            supportedTypes[typeof (Double)] = typeof (Double);
            supportedTypes[typeof (Decimal)] = typeof (Decimal);
            supportedTypes[typeof (DateTime)] = typeof (DateTime);
            supportedTypes[typeof (String)] = typeof (String);
        }


		    private static Hashtable SupportedTypes
        {
            get
            {
                if (supportedTypes == null)
                {
                    InitializeSupportedTypes();
                }
                return supportedTypes;
            }
        }

		/// <summary>
		/// Copies the row values to command parameters.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="row">The row.</param>
    	protected void CopyRowValuesToCommandParameters(IDbCommand command, Row row)
    	{
    		foreach (string column in row.Columns)
    		{
    			object value = row[column];
    			if (CanUseAsParameter(value))
    				AddParameter(command, column, value);
    		}
    	}

		/// <summary>
		/// Adds the parameter the specifed command
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="name">The name.</param>
		/// <param name="val">The val.</param>
        protected void AddParameter(IDbCommand command, string name, object val)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = paramPrefix + name;
            parameter.Value = val ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

		/// <summary>
        /// Determines whether this value can be use as a parameter to ADO.Net provider.
        /// This perform a simple heuristic 
        /// </summary>
        /// <param name="value">The value.</param>
        private static bool CanUseAsParameter(object value)
        {
			if(value==null)
				return true;
            return SupportedTypes.ContainsKey(value.GetType());
        }
    }
}