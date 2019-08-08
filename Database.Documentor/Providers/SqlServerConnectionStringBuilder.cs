using System.ComponentModel;
using System.Text;

namespace Database.Documentor.Providers
{
    public class SqlServerConnectionStringBuilder : ConnectionStringBuilder
    {
        private string _server;
        private string _database;
        private string _userid;
        private string _password;
        private bool _integratedSecurity;

        /// <summary>Name of database server.</summary>
        /// <value>String containing name of database server.</value>
        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("localhost")]
        [Description("Name of database server.")]
        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
            }
        }

        /// <summary>Name of database.</summary>
        /// <value>String containing name of database.</value>
        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("Northwind")]
        [Description("Name of database.")]
        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                _database = value;
            }
        }

        /// <summary>Database User ID</summary>
        /// <value>String containing user IDs</value>
        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("sa")]
        [Description("User ID to use to connect to database Server.")]
        public string UserID
        {
            get
            {
                return _userid;
            }
            set
            {
                _userid = value;
            }
        }

        /// <summary>Database Password</summary>
        /// <value>String containing database password</value>
        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [Description("Password to use to connect to database Server.")]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        /// <summary>Should integrated security be used to connected to database.</summary>
        /// <value>Boolean</value>
        [Browsable(true)]
        [Category("Sql Server Database Connection Properties")]
        [DefaultValue("No")]
        [Description("Name of database server.")]
        public bool IntegratedSecurity
        {
            get
            {
                return _integratedSecurity;
            }
            set
            {
                _integratedSecurity = value;
            }
        }

        public SqlServerConnectionStringBuilder()
        {
            _type = "SqlServer";
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