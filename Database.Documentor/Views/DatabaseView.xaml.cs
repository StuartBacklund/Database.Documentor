using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Database.Documentor.Providers;

namespace Database.Documentor.Views
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        public SqlServerSchemaProvider SqlServerSchemaProviderProperty
        {
            get { return (SqlServerSchemaProvider)GetValue(SqlServerSchemaProviderPropertyProperty); }
            set { SetValue(SqlServerSchemaProviderPropertyProperty, value); }
        }

        public static readonly DependencyProperty SqlServerSchemaProviderPropertyProperty =
            DependencyProperty.Register("SqlServerSchemaProviderProperty", typeof(SqlServerSchemaProvider),
            typeof(DatabaseView));

        public string ServerName
        {
            get { return (string)GetValue(ServerNameProperty); }
            set { SetValue(ServerNameProperty, value); }
        }

        public static readonly DependencyProperty ServerNameProperty =
            DependencyProperty.Register("ServerName", typeof(string), typeof(DatabaseView));

        public string DatabaseName
        {
            get { return (string)GetValue(DatabaseNameProperty); }
            set { SetValue(DatabaseNameProperty, value); }
        }

        public static readonly DependencyProperty DatabaseNameProperty =
            DependencyProperty.Register("DatabaseName", typeof(string), typeof(DatabaseView));

        public bool IntegratedSecurity
        {
            get { return (bool)GetValue(IntegratedSecurityProperty); }
            set { SetValue(IntegratedSecurityProperty, value); }
        }

        public static readonly DependencyProperty IntegratedSecurityProperty =
            DependencyProperty.Register("IntegratedSecurity", typeof(bool), typeof(DatabaseView));

        public string UserId
        {
            get { return (string)GetValue(UserIdProperty); }
            set { SetValue(UserIdProperty, value); }
        }

        public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(string), typeof(DatabaseView), new PropertyMetadata(string.Empty));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(DatabaseView), new PropertyMetadata(string.Empty));

        public DatabaseView()
        {
            InitializeComponent();
        }
    }
}