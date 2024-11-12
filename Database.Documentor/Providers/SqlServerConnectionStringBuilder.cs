using System.ComponentModel;
using System.Text;

namespace Database.Documentor.Providers
{
    public class SqlServerConnectionStringBuilder : ConnectionStringBuilder
    {
        private string server;
        private string database;
        private string userid;
        private string password;
        private bool integratedSecurity;

        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("localhost")]
        [Description("Name of database server.")]
        public string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
            }
        }

        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("Northwind")]
        [Description("Name of database.")]
        public string Database
        {
            get
            {
                return database;
            }
            set
            {
                database = value;
            }
        }

        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("sa")]
        [Description("User ID to use to connect to database Server.")]
        public string UserID
        {
            get
            {
                return userid;
            }
            set
            {
                userid = value;
            }
        }

        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [Description("Password to use to connect to database Server.")]
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("No")]
        [Description("Name of database server.")]
        public bool IntegratedSecurity
        {
            get
            {
                return integratedSecurity;
            }
            set
            {
                integratedSecurity = value;
            }
        }

        public SqlServerConnectionStringBuilder()
        {
            type = "SqlServer";
        }

        public override string ConnectionString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("Server=" + this.Server + ";");
            s.Append("Database=" + this.Database + ";");

            if (this.IntegratedSecurity == true)
                s.Append("Integrated Security=SSPI;");
            else
            {
                s.Append("User Id=" + this.UserID + ";");
                s.Append("Password=" + this.Password + ";");
            }

            return s.ToString();
        }
    }
}