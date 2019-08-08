using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Database.Documentor.Utility;
using GalaSoft.MvvmLight;
using Prism.Commands;

namespace Database.Documentor.ViewModel
{
    public class SqlConnectionViewModel : ViewModelBase
    {
        private string userId;

        public string UserId
        {
            get { return userId; }
            set
            {
                userId = value;
                RaisePropertyChanged(() => UserId);
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        private List<string> databaseList;

        public List<string> DatabaseList
        {
            get
            {
                return databaseList;
            }
            set
            {
                databaseList = value;
                RaisePropertyChanged(() => DatabaseList);
            }
        }

        private ObservableCollection<string> instanceList;

        public ObservableCollection<string> InstanceList
        {
            get
            {
                return instanceList;
            }
            set
            {
                instanceList = value;
                RaisePropertyChanged(() => InstanceList);
            }
        }

        private string selectedDatabase;

        public string SelectedDatabase
        {
            get
            {
                return selectedDatabase;
            }
            set
            {
                selectedDatabase = value;
                RaisePropertyChanged(() => SelectedDatabase);
            }
        }

        private string selectedInstance;

        public string SelectedInstance
        {
            get
            {
                return selectedInstance;
            }
            set
            {
                selectedInstance = value;
                RaisePropertyChanged(() => SelectedInstance);
            }
        }

        // public string ServerName { get; set; }
        // public string DatabaseName { get; set; }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<SelectionChangedEventArgs>(this.SelectionChanged);
            }
        }

        private void SelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] == null) return;

            this.SelectedInstance = e.AddedItems[0].ToString();
        }

        public ICommand ListDatases
        {
            get
            {
                return new DelegateCommand<RoutedEventArgs>(this.ListDatasesEvent);
            }
        }

        private void ListDatasesEvent(RoutedEventArgs obj)
        {
            DatabaseList = GetDatabaseList();
        }

        private bool integratedSecurity;

        public bool IntegratedSecurity
        {
            get { return integratedSecurity; }
            set
            {
                integratedSecurity = value;
                RaisePropertyChanged(() => IntegratedSecurity);
            }
        }

        public SqlConnectionViewModel()//IEventAggregator eventaggregator
        {
            InstanceList = GetDatabaseInstances();
        }

        public List<string> GetDatabaseList()
        {
            List<string> list = new List<string>();

            try
            {
                var credentials = IntegratedSecurity ? "Integrated Security=True" : $"User Id={UserId};Password={Password}";
                string conString = $"Data Source={this.SelectedInstance};{credentials};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(dr[0].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return list;
        }

        public ObservableCollection<string> GetDatabaseInstances()
        {
            InstanceList = new ObservableCollection<string>();
            var instanceTable = SqlManangement.SqlDataSourceTable();

            foreach (DataRow row in instanceTable.Rows)
            {
                InstanceList.Add(row[0].ToString());
            }
            return InstanceList;
        }
    }
}