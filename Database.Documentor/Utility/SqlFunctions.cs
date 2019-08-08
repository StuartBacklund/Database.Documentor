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
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                            p.Value = DBNull.Value;
                        command.Parameters.Add(p);
                    }
                }
            }
        } // AttachParameters

        public static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if (commandParameters == null || dataRow == null)
                // Do nothing if we get no data
                return;

            int i = 0;
            foreach (SqlParameter commandParameter in commandParameters)
            {
                // Check the parameter name
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

            if ((commandParameters == null) && (parameterValues == null))
                // Do nothing if we get no data
                return;

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
                throw new ArgumentException("Parameter count does not match Parameter Value count.");

            // Value array
            j = commandParameters.Length - 1;
            var loopTo = j;
            for (i = 0; i <= loopTo; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
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
        } // AssignParameterValues

        public static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, ref bool mustCloseConnection)
        {
            if ((command == null))
                throw new ArgumentNullException("command");
            if ((commandText == null || commandText.Length == 0))
                throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            else
                mustCloseConnection = false;

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it.
            if (!(transaction == null))
            {
                if (transaction.Connection == null)
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (!(commandParameters == null))
                AttachParameters(command, commandParameters);
            return;
        } // PrepareCommand

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            // Create & open a SqlConnection, and dispose of it after we are done
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)

                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            int retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            // Finally, execute the command
            retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if ((mustCloseConnection))
                connection.Close();

            return retval;
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            int retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            // Finally, execute the command
            retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            return retval;
        } // ExecuteNonQuery

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        } // ExecuteNonQuery

        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataTable(connectionString, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteDataTable

        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection, and dispose of it after we are done
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataTable(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (!(connection == null))
                    connection.Dispose();
            }
        } // ExecuteDataTable

        public static DataTable ExecuteDataTable(string connectionString, string spName, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName);
        } // ExecuteDataTable

        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataTable(connection, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteDataTable

        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                // Create the DataAdapter & DataSet
                dataAdatpter = new SqlDataAdapter(cmd);

                // Fill the DataSet using default values for DataTable names, etc
                dataAdatpter.Fill(dt);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }
            if ((mustCloseConnection))
                connection.Close();

            // Return the dataset
            return dt;
        } // ExecuteDataTable

        public static DataTable ExecuteDataTable(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataTable(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataTable(connection, CommandType.StoredProcedure, spName);
        } // ExecuteDataTable

        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataTable(transaction, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                // Create the DataAdapter & DataSet
                dataAdatpter = new SqlDataAdapter(cmd);

                // Fill the DataSet using default values for DataTable names, etc
                dataAdatpter.Fill(dt);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }

            // Return the dataset
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection, and dispose of it after we are done
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteDataset

        // Execute a SqlCommand (that returns a resultset) against the specified SqlConnection
        // using the provided parameters.
        // e.g.:
        // Dim ds As Dataset = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24))
        // Parameters:
        // -connection - a valid SqlConnection
        // -commandType - the CommandType (stored procedure, text, etc.)
        // -commandText - the stored procedure name or T-SQL command
        // -commandParameters - an array of SqlParamters used to execute the command
        // Returns: A dataset containing the resultset generated by the command
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                // Create the DataAdapter & DataSet
                dataAdatpter = new SqlDataAdapter(cmd);

                // Fill the DataSet using default values for DataTable names, etc
                dataAdatpter.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }
            if ((mustCloseConnection))
                connection.Close();

            // Return the dataset
            return ds;
        }

        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            try
            {
                // Create the DataAdapter & DataSet
                dataAdatpter = new SqlDataAdapter(cmd);

                // Fill the DataSet using default values for DataTable names, etc
                dataAdatpter.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
            }
            finally
            {
                if ((!(dataAdatpter == null)))
                    dataAdatpter.Dispose();
            }

            // Return the dataset
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
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
            // Create a command and prepare it for execution
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

                // Detach the SqlParameters from the command object, so they can be used again
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
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            // Pass through the call to private overload using a null transaction value
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;
            // If we receive parameter values, we need to figure out where they go
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
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            // Pass through to private overload, indicating that the connection is owned by the caller
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

            // If we receive parameter values, we need to figure out where they go
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
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            // Create & open a SqlConnection, and dispose of it after we are done.
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            object retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

            // Execute the command & return the results
            retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            object retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            // Execute the command & return the results
            retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
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
            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            // Pass through the call using a null transaction value
            if ((connection == null))
                throw new ArgumentNullException("connection");
            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            try
            {
                XmlReader retval;

                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);

                // Create the DataAdapter & DataSet
                retval = cmd.ExecuteXmlReader();

                // Detach the SqlParameters from the command object, so they can be used again
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null/* TODO Change to default(_) if this is not a reference type */);
        } // ExecuteXmlReader

        // Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        // using the provided parameters.
        // e.g.:
        // Dim r As XmlReader = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24))
        // Parameters:
        // -transaction - a valid SqlTransaction
        // -commandType - the CommandType (stored procedure, text, etc.)
        // -commandText - the stored procedure name or T-SQL command using "FOR XML AUTO"
        // -commandParameters - an array of SqlParamters used to execute the command
        // Returns: An XmlReader containing the resultset generated by the command
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            // Create a command and prepare it for execution
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();

            XmlReader retval;
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            // Create the DataAdapter & DataSet
            retval = cmd.ExecuteXmlReader();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            return retval;
        } // ExecuteXmlReader

        // Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlTransaction
        // using the provided parameter values.  This method will discover the parameters for the
        // stored procedure, and assign the values based on parameter order.
        // This method provides no access to output parameters or the stored procedure' s return value parameter.
        // e.g.:
        // Dim r As XmlReader = ExecuteXmlReader(trans, "GetOrders", 24, 36)
        // Parameters:
        // -transaction - a valid SqlTransaction
        // -spName - the name of the stored procedure
        // -parameterValues - an array of objects to be assigned as the input values of the stored procedure
        // Returns: A dataset containing the resultset generated by the command
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            SqlParameter[] commandParameters;

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
        } // ExecuteXmlReader

        // Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in
        // the connection string.
        // e.g.:
        // FillDataset (connString, CommandType.StoredProcedure, "GetOrders", ds, new String() {"orders"})
        // Parameters:
        // -connectionString: A valid connection string for a SqlConnection
        // -commandType: the CommandType (stored procedure, text, etc.)
        // -commandText: the stored procedure name or T-SQL command
        // -dataSet: A dataset wich will contain the resultset generated by the command
        // -tableNames: this array will be used to create table mappings allowing the DataTables to be referenced
        // by a user defined name (probably the actual table name)
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");

            // Create & open a SqlConnection, and dispose of it after we are done
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);

                connection.Open();

                // Call the overload that takes a connection in place of the connection string
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

            // Create & open a SqlConnection, and dispose of it after we are done
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);

                connection.Open();

                // Call the overload that takes a connection in place of the connection string
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

            // Create & open a SqlConnection, and dispose of it after we are done
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);

                connection.Open();

                // Call the overload that takes a connection in place of the connection string
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
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null/* TODO Change to default(_) if this is not a reference type */);
        }

        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null/* TODO Change to default(_) if this is not a reference type */, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(SqlConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null/* TODO Change to default(_) if this is not a reference type */);
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

            // If we receive parameter values, we need to figure out where they go
            if (!(parameterValues == null) && parameterValues.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }

        public static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            if ((dataSet == null))
                throw new ArgumentNullException("dataSet");

            // Create a command and prepare it for execution
            SqlCommand command = new SqlCommand();

            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);

            // Create the DataAdapter & DataSet
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

            try
            {
                // Add the table mappings specified by the user
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

                // Fill the DataSet using default values for DataTable names, etc
                dataAdapter.Fill(dataSet);

                // Detach the SqlParameters from the command object, so they can be used again
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

            // Create a SqlDataAdapter, and dispose of it after we are done
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            try
            {
                // Set the data adapter commands
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                // Update the dataset changes in the data source
                dataAdapter.Update(dataSet, tableName);

                // Commit all the changes made to the DataSet
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
            // Create a SqlCommand
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // If we receive parameter values, we need to figure out where they go
            if (!(sourceColumns == null) && sourceColumns.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided source columns to these parameters based on parameter order
                int index;
                var loopTo = sourceColumns.Length - 1;
                for (index = 0; index <= loopTo; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the SqlCommand object
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
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
            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
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

            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
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
            // If the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
        }

        // an XmlReader containing the resultset generated by the command
        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            if ((transaction == null))
                throw new ArgumentNullException("transaction");
            if (!(transaction == null) && (transaction.Connection == null))
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null || spName.Length == 0))
                throw new ArgumentNullException("spName");
            // if the row has values, the store procedure parameters must be initialized
            if ((!(dataRow == null) && dataRow.ItemArray.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlFunctionsParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
        }
    } // SqlFunctions

    public sealed class SqlFunctionsParameterCache
    {
        // Since this class provides only static methods, make the default constructor private to prevent
        // instances from being created with "new SqlFunctionsParameterCache()".
        private SqlFunctionsParameterCache()
        {
        } // New

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
        } // DiscoverSpParameterSet

        // Deep copy of cached SqlParameter array
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
        } // CloneParameters

        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0))
                throw new ArgumentNullException("connectionString");
            if ((commandText == null || commandText.Length == 0))
                throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        } // CacheParameterSet

        // retrieve a parameter array from the cache
        // Parameters:
        // -connectionString - a valid connection string for a SqlConnection
        // -commandText - the stored procedure name or T-SQL command
        // Returns: An array of SqlParamters
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
        } // GetSpParameterSet

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
        } // GetSpParameterSet
    } // SqlFunctionsParameterCache
}