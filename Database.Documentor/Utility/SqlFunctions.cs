using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Database.Documentor.Utility
{
    public class SqlFunctions
    {
        public static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if ((command == null))
                throw new ArgumentNullException("command");
            if ((!(commandParameters == null)))
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if ((!(p == null)))
                    {
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                            p.Value = DBNull.Value;
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        public static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if (commandParameters == null || dataRow == null)

                return;

            int i = 0;
            foreach (SqlParameter commandParameter in commandParameters)
            {
                if ((commandParameter.ParameterName == null || commandParameter.ParameterName.Length <= 1))
                    throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: ' {1}' .", i, commandParameter.ParameterName));
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i = i + 1;
            }
        }

        public static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            int i;
            int j;

            if ((commandParameters == null) && (parameterValues == null)) return;

            if (commandParameters.Length != parameterValues.Length)
                throw new ArgumentException("Parameter count does not match Parameter Value count.");

            j = commandParameters.Length - 1;
            var loopTo = j;
            for (i = 0; i <= loopTo; i++)
            {
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if ((paramInstance.Value == null))
                        commandParameters[i].Value = DBNull.Value;
                    else
                        commandParameters[i].Value = paramInstance.Value;
                }
                else if ((parameterValues[i] == null))
                    commandParameters[i].Value = DBNull.Value;
                else
                    commandParameters[i].Value = parameterValues[i];
            }
        }

        public static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, ref bool mustCloseConnection)
        {
            if ((command == null))
                throw new ArgumentNullException("command");
            if ((commandText == null || commandText.Length == 0))
                throw new ArgumentNullException("commandText");

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            else
                mustCloseConnection = false;

            command.Connection = connection;

            command.CommandText = commandText;

            if (!(transaction == null))
            {
                if (transaction.Connection == null)
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            command.CommandType = commandType;

            if (!(commandParameters == null))
                AttachParameters(command, commandParameters);
            return;
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlCommand cmd = new SqlCommand();
            int retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            retval = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            if ((mustCloseConnection))
                connection.Close();

            return retval;
        }

        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();
            int retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
            retval = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            return retval;
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        }

        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                return ExecuteDataTable(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static DataTable ExecuteDataTable(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName);
        }

        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                dataAdatpter = new SqlDataAdapter(cmd);
                dataAdatpter.Fill(dt);
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }
            if ((mustCloseConnection))
                connection.Close();

            return dt;
        }

        public static DataTable ExecuteDataTable(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataTable(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataTable(connection, CommandType.StoredProcedure, spName);
        }

        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                dataAdatpter = new SqlDataAdapter(cmd);
                dataAdatpter.Fill(dt);
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }

            return dt;
        }

        public static DataTable ExecuteDataTable(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                dataAdatpter = new SqlDataAdapter(cmd);
                dataAdatpter.Fill(ds);
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }
            if ((mustCloseConnection))
                connection.Close();

            return ds;
        }

        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                dataAdatpter = new SqlDataAdapter(cmd);
                dataAdatpter.Fill(ds);
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }

            return ds;
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        }

        public enum SqlConnectionOwnership
        {
            // Connection is owned and managed by SqlFunctions
            Internal,

            // Connection is owned and managed by the caller
            External
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;

            SqlCommand cmd = new SqlCommand();
            try
            {
                // Create a reader
                SqlDataReader dataReader;

                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

                // Call ExecuteReader with the appropriate CommandBehavior
                if (connectionOwnership == SqlConnectionOwnership.External)
                    dataReader = cmd.ExecuteReader();
                else
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                bool canClear = true;

                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if ((canClear))
                    cmd.Parameters.Clear();

                return dataReader;
            }
            catch
            {
                if ((mustCloseConnection))
                    connection.Close();
                throw;
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                // Call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves
                if (!(connection == null))
                    connection.Dispose();
                throw;
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlCommand cmd = new SqlCommand();
            object retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);
            retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            if ((mustCloseConnection))
                connection.Close();

            return retval;
        }

        public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();
            object retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            retval = cmd.ExecuteScalar();

            cmd.Parameters.Clear();

            return retval;
        }

        public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            try
            {
                XmlReader retval;
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);
                retval = cmd.ExecuteXmlReader();
                cmd.Parameters.Clear();

                return retval;
            }
            catch
            {
                if ((mustCloseConnection))
                    connection.Close();
                throw;
            }
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
        } // ExecuteXmlReader

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();

            XmlReader retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            retval = cmd.ExecuteXmlReader();

            cmd.Parameters.Clear();

            return retval;
        } // ExecuteXmlReader

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
        } // ExecuteXmlReader

        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);

                connection.Open();

                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);

                connection.Open();

                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static void FillDataset(string connectionString, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        }

        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(SqlConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(SqlTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        public static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");

            SqlCommand command = new SqlCommand();

            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

            try
            {
                if (!(tableNames == null) && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    int index;
                    var loopTo = tableNames.Length - 1;
                    for (index = 0; index <= loopTo; index++)
                    {
                        if ((tableNames[index] == null || tableNames[index].Length == 0))
                            throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName = tableName + (index + 1).ToString();
                    }
                }

                dataAdapter.Fill(dataSet);

                command.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdapter == null)))
                    dataAdapter.Dispose();
            }

            if ((mustCloseConnection))
                connection.Close();
        }

        public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if ((insertCommand == null))
                throw new ArgumentNullException("insertCommand");
            if ((deleteCommand == null))
                throw new ArgumentNullException("deleteCommand");
            if ((updateCommand == null))
                throw new ArgumentNullException("updateCommand");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");
            if ((tableName == null || tableName.Length == 0))
                throw new ArgumentNullException("tableName");

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            try
            {
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;
                dataAdapter.Update(dataSet, tableName);
                dataSet.AcceptChanges();
            }
            finally
            {
                if ((!(dataAdapter == null)))
                    dataAdapter.Dispose();
            }
        }

        public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!(sourceColumns == null) && sourceColumns.Length > 0)
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                int index;
                var loopTo = sourceColumns.Length - 1;
                for (index = 0; index <= loopTo; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }

        public static int ExecuteNonQueryTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, dataRow);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static int ExecuteNonQueryTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        public static DataSet ExecuteDatasetTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReaderTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        public static object ExecuteScalarTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlFunctions.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlFunctions.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static object ExecuteScalarTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static object ExecuteScalarTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
        }
    } // SqlFunctions

    public sealed class SqlFunctionsParameterCache
    {
        private SqlFunctionsParameterCache()
        {
        }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter[] discoveredParameters;
            connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();
            if (!includeReturnValueParameter)
                cmd.Parameters.RemoveAt(0);

            discoveredParameters = new SqlParameter[cmd.Parameters.Count - 1 + 1];
            cmd.Parameters.CopyTo(discoveredParameters, 0);

            foreach (SqlParameter discoveredParameter in discoveredParameters)
                discoveredParameter.Value = DBNull.Value;

            return discoveredParameters;
        }

        public static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            int i = 0;
            int j = originalParameters.Length - 1;
            SqlParameter[] clonedParameters = new SqlParameter[j + 1];
            var loopTo = j;
            for (i = 0; i <= loopTo; i++)
            {
                clonedParameters[i] = originalParameters[i];
            }

            return clonedParameters;
        }

        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((commandText == null || commandText.Length == 0))
                throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((commandText == null || commandText.Length == 0))
                throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;
            SqlParameter[] cachedParameters = (SqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
                return null;
            else
                return CloneParameters(cachedParameters);
        } // GetCachedParameterSet

        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        } // GetSpParameterSet

        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            SqlConnection connection = null;
            SqlParameter[] sqlParameter = null;
            try
            {
                connection = new SqlConnection(connectionString);
                sqlParameter = GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
            return sqlParameter;
        } // GetSpParameterSet

        public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        } // GetSpParameterSet

        public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            SqlConnection clonedConnection = null;
            SqlParameter[] sqlParameter = null;
            try
            {
                clonedConnection = (SqlConnection)connection;
                sqlParameter = GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
            finally
            {
                if (!(clonedConnection == null))
                    clonedConnection.Dispose();
            }
            return sqlParameter;
        }

        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            SqlParameter[] cachedParameters;
            string hashKey;

            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            var returnparameterWording = string.Empty;

            if (includeReturnValueParameter)
            {
                returnparameterWording = ":include ReturnValue Parameter";
            }

            hashKey = connection.ConnectionString + ":" + spName + returnparameterWording;

            cachedParameters = (SqlParameter[])paramCache[hashKey];

            if ((cachedParameters == null))
            {
                SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }
    }
}