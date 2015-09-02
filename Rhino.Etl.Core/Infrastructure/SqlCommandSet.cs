#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Rhino.Etl.Core.Infrastructure
{
    /// <summary>
    /// Expose the batch functionality in ADO.Net 2.0
    /// Microsoft in its wisdom decided to make my life hard and mark it internal.
    /// Through the use of Reflection and some delegates magic, I opened up the functionality.
    /// 
    /// There is NO documentation for this, and likely zero support.
    /// Use at your own risk, etc...
    /// 
    /// Observable performance benefits are 50%+ when used, so it is really worth it.
    /// </summary>
    public class SqlCommandSet : IDisposable
    {
        private static readonly Type sqlCmdSetType;
        private readonly object instance;
        private readonly PropSetter<SqlConnection> connectionSetter;
        private readonly PropSetter<SqlTransaction> transactionSetter;
        private readonly PropSetter<int> timeoutSetter;
        private readonly PropGetter<SqlConnection> connectionGetter;
        private readonly PropGetter<SqlCommand> commandGetter;
        private readonly AppendCommand doAppend;
        private readonly ExecuteNonQueryCommand doExecuteNonQuery;
        private readonly DisposeCommand doDispose;
        private int countOfCommands = 0;

        static SqlCommandSet()
        {
            Assembly sysData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
            Guard.Against(sqlCmdSetType == null, "Could not find SqlCommandSet!");
        }

        /// <summary>
        /// Creates a new instance of SqlCommandSet
        /// </summary>
        public SqlCommandSet()
        {

            instance = Activator.CreateInstance(sqlCmdSetType, true);

            timeoutSetter = (PropSetter<int>)
                               Delegate.CreateDelegate(typeof(PropSetter<int>),
                                                       instance, "set_CommandTimeout");
            connectionSetter = (PropSetter<SqlConnection>)
                               Delegate.CreateDelegate(typeof(PropSetter<SqlConnection>),
                                                       instance, "set_Connection");
            transactionSetter = (PropSetter<SqlTransaction>)
                                Delegate.CreateDelegate(typeof(PropSetter<SqlTransaction>),
                                                        instance, "set_Transaction");
            connectionGetter = (PropGetter<SqlConnection>)
                               Delegate.CreateDelegate(typeof(PropGetter<SqlConnection>),
                                                       instance, "get_Connection");
            commandGetter =
                (PropGetter<SqlCommand>)Delegate.CreateDelegate(typeof(PropGetter<SqlCommand>), instance, "get_BatchCommand");
            doAppend = (AppendCommand)Delegate.CreateDelegate(typeof(AppendCommand), instance, "Append");
            doExecuteNonQuery = (ExecuteNonQueryCommand)
                                Delegate.CreateDelegate(typeof(ExecuteNonQueryCommand),
                                                        instance, "ExecuteNonQuery");
            doDispose = (DisposeCommand)Delegate.CreateDelegate(typeof(DisposeCommand), instance, "Dispose");

        }

        /// <summary>
        /// Append a command to the batch
        /// </summary>
        /// <param name="command"></param>
        public void Append(SqlCommand command)
        {
            AssertHasParameters(command);
            doAppend(command);
            countOfCommands++;
        }

        /// <summary>
        /// This is required because SqlClient.SqlCommandSet will throw if 
        /// the command has no parameters.
        /// </summary>
        /// <param name="command"></param>
        private static void AssertHasParameters(SqlCommand command)
        {
            if (command.Parameters.Count == 0 &&
                (RuntimeInfo.Version.Contains("2.0") || RuntimeInfo.Version.Contains("1.1")))
            {
                throw new ArgumentException(
                    "A command in SqlCommandSet must have parameters. You can't pass hardcoded sql strings.");
            }
        }
        
        /// <summary>
        /// Return the batch command to be executed
        /// </summary>
        public SqlCommand BatchCommand
        {
            get
            {
                return commandGetter();
            }
        }
        
        /// <summary>
        /// The number of commands batched in this instance
        /// </summary>
        public int CountOfCommands
        {
            get { return countOfCommands; }
        }

        /// <summary>
        /// Executes the batch
        /// </summary>
        /// <returns>
        /// This seems to be returning the total number of affected rows in all queries
        /// </returns>
        public int ExecuteNonQuery()
        {
            Guard.Against<ArgumentException>(Connection == null,
                                             "Connection was not set! You must set the connection property before calling ExecuteNonQuery()");
            if(CountOfCommands==0)
                return 0;
            return doExecuteNonQuery();
        }

        ///<summary>
        /// The connection the batch will use
        ///</summary>
        public SqlConnection Connection
        {
            get { return connectionGetter(); }
            set { connectionSetter(value); }
        }

        /// <summary>
        /// Set the timeout of the commandSet
        /// </summary>
        public int CommandTimeout
        {
            set { timeoutSetter(value); }
        }

        /// <summary>
        /// The transaction the batch will run as part of
        /// </summary>
        public SqlTransaction Transaction
        {
            set { transactionSetter(value); }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            doDispose();
        }

        #region Delegate Definations
        private delegate void PropSetter<T>(T item);
        private delegate T PropGetter<T>();
        private delegate void AppendCommand(SqlCommand command);
        private delegate int ExecuteNonQueryCommand();
        private delegate void DisposeCommand();
        #endregion
    }
}