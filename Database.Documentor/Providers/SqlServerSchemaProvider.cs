using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Database.Documentor.Properties;
using Database.Documentor.Utility;

namespace Database.Documentor.Providers
{
    [SchemaAttribute("Sql Server", "SqlServer")]
    public class SqlServerSchemaProvider : SchemaProvider
    {
        private SqlConnection sqlConn;

        public SqlServerSchemaProvider()
        {
            ConnStringBuilder = new SqlServerConnectionStringBuilder();
        }

        public override void OpenConnection()
        {
            sqlConn = new SqlConnection(ConnStringBuilder.ConnectionString());
            sqlConn.Open();
        }

        public override void CloseConnection()
        {
            sqlConn.Close();
            sqlConn = null;
        }

        public override DataTable GetFunctions()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SELECT name AS function_name, SCHEMA_NAME(schema_id) AS schema_name, type_desc as DESCRIPTION");
            s.Append(", create_date as Created , modify_date as Modified FROM sys.objects WHERE type_desc LIKE '%FUNCTION%'");

            DataTable dt = new DataTable(Resources.FunctionsText);

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetFunction(string functionName)
        {
            StringBuilder s = new StringBuilder();

            s.Append($"SELECT SCHEMA_NAME(schema_id) AS schema_name, o.name AS object_name ,o.type_desc ,p.parameter_id ");
            s.Append($",p.name AS parameter_name  ,TYPE_NAME(p.user_type_id) AS parameter_type");
            s.Append($", p.max_length,p.precision,p.scale,p.is_output,m.definition as DESCRIPTION");
            s.Append($" FROM sys.objects AS o INNER JOIN sys.parameters AS p ON o.object_id = p.object_id");
            s.Append($" INNER JOIN sys.sql_modules AS m ON m.object_id = p.object_id");
            s.Append($" WHERE o.object_id = OBJECT_ID('{functionName}')");
            s.Append($"ORDER BY schema_name, object_name, p.parameter_id;");
            DataTable dt = new DataTable("Function");

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetTables()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SELECT a.TABLE_NAME, ISNULL(b.value, '') as DESCRIPTION ");
            s.Append(" FROM INFORMATION_SCHEMA.TABLES as a LEFT JOIN");
            s.Append(" (SELECT * FROM ::fn_listextendedproperty ('ms_description', 'user', 'dbo', 'table', default, default, default)) as b");
            s.Append(" ON a.TABLE_NAME = b.objname COLLATE DATABASE_DEFAULT");
            s.Append(" WHERE (a.TABLE_TYPE = 'BASE TABLE')");
            s.Append($" ORDER BY a.TABLE_NAME");

            DataTable dt = new DataTable(Resources.TablesText);

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetViews()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SELECT a.TABLE_NAME, a.VIEW_DEFINITION, ISNULL(b.value, '') as DESCRIPTION");
            s.Append(" FROM INFORMATION_SCHEMA.VIEWS as a LEFT JOIN");
            s.Append(" (SELECT * FROM ::fn_listextendedproperty ('ms_description', 'user', 'dbo', 'view', default, default, default)) as b");
            s.Append(" ON a.TABLE_NAME = b.objname COLLATE DATABASE_DEFAULT");
            s.Append(" WHERE objectproperty(object_id(TABLE_NAME),'IsMsShipped') = 0");
            s.Append($" ORDER BY a.TABLE_NAME");

            DataTable dt = new DataTable(Resources.ViewsText);

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetProcedures()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SELECT a.ROUTINE_NAME as PROCEDURE_NAME, a.ROUTINE_DEFINITION as PROCEDURE_DEFINITION, ISNULL(b.value, '') as DESCRIPTION");
            s.Append(" FROM INFORMATION_SCHEMA.ROUTINES AS a LEFT JOIN ");
            s.Append(" (SELECT * FROM ::fn_listextendedproperty ('ms_description', 'user', 'dbo', 'procedure', default, default, default)) as b");
            s.Append(" ON a.ROUTINE_NAME = b.objname COLLATE DATABASE_DEFAULT");
            s.Append(" WHERE a.ROUTINE_TYPE = 'procedure'");
            s.Append(" AND objectproperty(object_id(ROUTINE_NAME),'IsMsShipped')=0");
            s.Append($" ORDER BY PROCEDURE_NAME");

            DataTable dt = new DataTable(Resources.ProceduresText);

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetRelationships(string tableName)
        {
            // Parameter @Table_Name
            StringBuilder s = new StringBuilder();
            s.Append("SELECT  rkey.TABLE_NAME as PK_TABLE_NAME, rkey.COLUMN_NAME as PK_COLUMN_NAME, ");
            s.Append(" fkey.TABLE_NAME as FK_TABLE_NAME, fkey.COLUMN_NAME as FK_COLUMN_NAME");
            s.Append(" FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE rkey JOIN ");
            s.Append(" INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS ref ON rkey.CONSTRAINT_NAME = ref.UNIQUE_CONSTRAINT_NAME");
            s.Append(" JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE fkey ON ref.CONSTRAINT_NAME = fkey.CONSTRAINT_NAME");
            s.Append(" WHERE rkey.TABLE_NAME = @table_name OR fkey.TABLE_NAME = @table_name");
            s.Append($" ORDER BY PK_TABLE_NAME");

            DataTable dt = new DataTable("Relationships");

            SqlParameter[] Params = new[] { new SqlParameter("@table_name", SqlDbType.VarChar) };
            Params[0].Value = tableName;

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString(), Params);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetParameters(string routineName)
        {
            // Parameter @Routine_Name
            StringBuilder s = new StringBuilder();
            s.Append("SELECT a.SPECIFIC_NAME AS PROCEDURE_NAME, a.PARAMETER_MODE AS PARAMETER_DIRECTION, ");
            s.Append(" a.PARAMETER_NAME, a.DATA_TYPE, a.CHARACTER_MAXIMUM_LENGTH, ISNULL(b.value, '') AS DESCRIPTION");
            s.Append(" FROM INFORMATION_SCHEMA.PARAMETERS AS a LEFT JOIN");
            s.Append(" (SELECT * FROM ::fn_listextendedproperty ('ms_description', 'user', 'dbo', 'procedure', @routine_name, 'parameter', default)) as b");
            s.Append(" ON a.PARAMETER_NAME = b.objname COLLATE DATABASE_DEFAULT");
            s.Append(" WHERE a.SPECIFIC_NAME = @routine_name");
            s.Append(" ORDER BY a.ORDINAL_POSITION");

            DataTable dt = new DataTable("Parameters");

            SqlParameter[] Params = new[] { new SqlParameter("@routine_name", SqlDbType.VarChar) };
            Params[0].Value = routineName;

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString(), Params);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetColumns(string tableName)
        {
            // Parameter @Table_Name
            StringBuilder s = new StringBuilder();
            s.Append("SELECT A.ORDINAL_POSITION, A.TABLE_NAME, A.COLUMN_NAME, A.COLUMN_DEFAULT, A.IS_NULLABLE, A.DATA_TYPE, ");
            s.Append(" A.CHARACTER_MAXIMUM_LENGTH, B.value as DESCRIPTION,");
            s.Append(" [IS_IDENTITY] = ColumnProperty(object_id(TABLE_NAME), COLUMN_NAME,'IsIdentity')");
            s.Append(" FROM INFORMATION_SCHEMA.COLUMNS as A LEFT JOIN");
            s.Append(" (SELECT * FROM ::fn_listextendedproperty ('ms_description', 'user', 'dbo', 'table', @table_name, 'column', default)) AS B");
            s.Append(" ON A.COLUMN_NAME = B.objname COLLATE DATABASE_DEFAULT");
            s.Append(" WHERE A.TABLE_NAME = @table_name");
            s.Append(" ORDER BY A.ORDINAL_POSITION");

            DataTable dt = new DataTable("Columns");

            SqlParameter[] Params = new[] { new SqlParameter("@table_name", SqlDbType.VarChar) };
            Params[0].Value = tableName;

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.Text, s.ToString(), Params);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetPrimaryKeyColumns(string tableName)
        {
            // Parameter @Table_Name
            string sqlText = "sp_pkeys";

            DataTable dt = new DataTable("PrimaryKeys");

            SqlParameter[] Params = new[] { new SqlParameter("@table_name", SqlDbType.VarChar) };
            Params[0].Value = tableName;

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.StoredProcedure, sqlText, Params);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override DataTable GetIndexes(string tableName)
        {
            // Parameter @objname
            string sqlText = "sp_helpindex";

            DataTable dt = new DataTable("Indexes");

            SqlParameter[] Params = new[] { new SqlParameter("@objname", SqlDbType.VarChar) };
            Params[0].Value = tableName;

            try
            {
                dt = SqlFunctions.ExecuteDataTable(sqlConn, CommandType.StoredProcedure, sqlText, Params);

                DataTable dtOut = new DataTable();
                dtOut.Columns.Add(new DataColumn("INDEX_NAME"));
                dtOut.Columns.Add(new DataColumn("COLUMN_NAME"));
                dtOut.Columns.Add(new DataColumn("UNIQUE"));
                dtOut.Columns.Add(new DataColumn("CLUSTERED"));

                DataRow newrow;
                foreach (DataRow row in dt.Rows)
                {
                    newrow = dtOut.NewRow();

                    newrow["INDEX_NAME"] = System.Convert.ToString(row["INDEX_NAME"]);
                    newrow["COLUMN_NAME"] = System.Convert.ToString(row["INDEX_KEYS"]);

                    if (System.Convert.ToString(row["index_description"]).IndexOf("unique") >= 0)
                        newrow["UNIQUE"] = "yes";
                    else
                        newrow["UNIQUE"] = "no";

                    if (System.Convert.ToString(row["index_description"]).IndexOf("nonclustered") >= 0)
                        newrow["CLUSTERED"] = "no";
                    else
                        newrow["CLUSTERED"] = "yes";

                    dtOut.Rows.Add(newrow);
                }

                return dtOut;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}