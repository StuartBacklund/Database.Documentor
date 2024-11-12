using System.Data;

namespace Database.Documentor.Providers
{
    public abstract class SchemaProvider
    {
        private ConnectionStringBuilder connStringBuilder;

        public ConnectionStringBuilder ConnStringBuilder
        {
            get
            {
                return connStringBuilder;
            }
            set
            {
                connStringBuilder = value;
            }
        }

        public SchemaProvider()
        {
        }

        /// <summary>Opens a connection to the database.</summary>
        public abstract void OpenConnection();

        /// <summary>Closes connection to the database.</summary>
        public abstract void CloseConnection();

        /// <summary>Retrieves list of functions from the database.
        /// Columns Required In Returned Data Table
        /// TABLE_NAME     string
        /// DESCRIPTION    string
        /// </summary>
        /// <returns>DataTable</returns>
        public abstract DataTable GetFunctions();

        public abstract DataTable GetFunction(string resourceName);

        /// <summary>Retrieves list of tables from the database.
        /// Columns Required In Returned Data Table
        /// TABLE_NAME     string
        /// DESCRIPTION    string
        /// </summary>
        /// <returns>DataTable</returns>
        public abstract DataTable GetTables();

        /// <summary>Retrieves list of views from the database.
        /// Columns Required In Returned Data Table
        /// TABLE_NAME       string
        /// VIEW_DEFINITION  string
        /// DESCRIPTION      string
        /// </summary>
        /// <returns>DataTable</returns>
        public abstract DataTable GetViews();

        /// <summary>Retrieves list of stored procedures from the database.
        /// Columns Required In Returned Data Table
        /// PROCEDURE_NAME        string
        /// PROCEDURE_DEFINITION  string
        /// DESCRIPTION           string
        /// </summary>
        /// <returns>DataTable</returns>
        public abstract DataTable GetProcedures();

        /// <summary>Retrieves information about the relationships for one table in the database.
        /// Columns Required In Returned Data Table
        /// PK_TABLE_NAME   string
        /// PK_COLUMN_NAME  string
        /// FK_TABLE_NAME   string
        /// FK_COLUMN_NAME  string
        /// </summary>
        /// <returns>DataTable</returns>
        /// <param name="_tableName">Name of the table to retrieve relationships data from.</param>
        public abstract DataTable GetRelationships(string _tableName);

        /// <summary>Retrieves information about the parameters of a stored procedure.
        /// Columns Required In Returned Data Table
        /// PROCEDURE_NAME            string
        /// PARAMETER_DIRECTION       string
        /// PARAMETER_NAME            string
        /// DATA_TYPE                 string
        /// CHARACTER_MAXIMUM_LENGTH  integer
        /// DESCRIPTION               string
        /// </summary>
        /// <returns>DataTable</returns>
        /// <param name="_routineName">Name of the stored procedure to retrieve parameters data from.</param>
        public abstract DataTable GetParameters(string _routineName);

        /// <summary>Retrieves information about the columns of a table.
        /// Columns Required In Returned Data Table
        /// ORDINAL_POSITION          integer
        /// TABLE_NAME                string
        /// COLUMN_NAME               string
        /// COLUMN_DEFAULT            string
        /// IS_NULLABLE               yes, no
        /// DATA_TYPE                 string
        /// CHARACTER_MAXIMUM_LENGTH  integer
        /// DESCRIPTION               string
        /// IS_IDENTITY               0,1     1 = yes, 0 = no
        /// </summary>
        /// <returns>DataTable</returns>
        /// <param name="_tableName">Name of the table to retrieve Columns data from.</param>
        public abstract DataTable GetColumns(string _tableName);

        /// <summary>Retrieves information about the Primary Key columns of a table.
        /// Columns Required In Returned Data Table
        /// PK_NAME       string
        /// TABLE_NAME    string
        /// COLUMN_NAME   string
        /// </summary>
        /// <returns>DataTable</returns>
        /// <param name="_tableName">Name of the table to retrieve primary key data from.</param>
        public abstract DataTable GetPrimaryKeyColumns(string _tableName);

        /// <summary>Retrieves information about the indexes of a table.
        /// Columns Required In Returned Data Table
        /// INDEX_NAME    string
        /// UNIQUE        yes, no
        /// CLUSTERED     yes, no
        /// COLUMN_NAME   string
        /// </summary>
        /// <returns>DataTable</returns>
        /// <param name="_tableName">Name of the table to retrieve Columns data from.</param>
        public abstract DataTable GetIndexes(string _tableName);
    }
}