using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Database.Documentor.Commands;
using Database.Documentor.Utility;
using GalaSoft.MvvmLight;
using Prism.Commands;
using Prism.Events;

namespace Database.Documentor.ViewModel
{
    public class SqlConnectionViewModel : ViewModelBase
    {
        private string userId;
        private IEventAggregator eventAggregator;
        private async Task UIThreadAction(Action act)
        {
            await _dispatcher.InvokeAsync(() => act.Invoke());
        }
        private Dispatcher _dispatcher
        {
            get { return App.Current.Dispatcher; }
        }
        private async void PropertyChangedAsync(string property)
        {
            await UIThreadAction(() => RaisePropertyChanged(property));
        }
        private Action addfile = null;
        public string UserId
        {
            get { return userId; }
            set
            {
                userId = value;
                RaisePropertyChanged(() => UserId);
            }
        }
        private string serverName;

        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                RaisePropertyChanged(() => ServerName);
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
        public ObservableCollection<SqlServerInstance> InstanceList { get { return instanceList; } }
        private ObservableCollection<SqlServerInstance> instanceList = new ObservableCollection<SqlServerInstance>();

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

        private SqlServerInstance selectedInstance;

        public SqlServerInstance SelectedInstance
        {
            get
            {
                return selectedInstance;
            }
            set
            {
                if (value != null)
                {
                    this.ServerName = value.ServerInstance;
                    RaisePropertyChanged(() => ServerName);
                    selectedInstance = value;
                    RaisePropertyChanged(() => SelectedInstance);
                }

            }
        }


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

            this.SelectedInstance = e.AddedItems[0] as SqlServerInstance;
        }

        public ICommand ListDatases
        {
            get
            {
                return new DelegateCommand<RoutedEventArgs>(this.ListDatasesEvent);
            }
        }
        public ICommand ListServersCommand
        {
            get
            {
                return new DelegateCommand<RoutedEventArgs>(this.ListServersEvent);
            }
        }
        public bool IsBusy
        {
            get { return !_isidle; }
        }

        private bool _isidle;

        public bool IsIdle
        {
            get { return _isidle; }
            set
            {
                _isidle = value;
                RaisePropertyChanged(() => IsIdle);
                RaisePropertyChanged(() => IsBusy);
            }
        }

        private void ListServersEvent(RoutedEventArgs obj)
        {
            IsIdle = false;
            ListServersAsyncCommand.Execute(InstanceList);
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
        public DelegateCommand<object> ListServersAsyncCommand { get; set; }

        public SqlConnectionViewModel(IEventAggregator eventAggregator)
        {

            this.eventAggregator = eventAggregator;
            IsIdle = true;
            ListServersAsyncCommand = new DelegateCommand<object>(async (ob) =>
            {
                this.eventAggregator.GetEvent<ShowProgressDialogEvent>().Publish(true);
                await GetServersAsync(InstanceList);
                RaisePropertyChanged(() => InstanceList);

                IsIdle = true;
                this.eventAggregator.GetEvent<ShowProgressDialogEvent>().Publish(false);
                RaisePropertyChanged(() => IsIdle);
            });
        }
        public Task GetServersAsync(object parameter)
        {
            instanceList = parameter as ObservableCollection<SqlServerInstance>;
            return Task.Run(() => GetServers(instanceList));
        }
        public async void GetServers(ObservableCollection<SqlServerInstance> parameter)
        {


            var sim = GetDatabaseInstances();
            addfile = () =>
            {
                foreach (var item in sim)
                {
                    parameter.Add(item);
                };
            };
            await UIThreadAction(() => addfile.Invoke());

        }
        public List<string> GetDatabaseList()
        {
            List<string> list = new List<string>();

            try
            {
                var selectedServer = string.IsNullOrEmpty(this.SelectedInstance.ServerInstance) ? this.ServerName : this.SelectedInstance.ServerInstance;
                var credentials = IntegratedSecurity ? "Integrated Security=True" : $"User Id={UserId};Password={Password}";
                string conString = $"Data Source={selectedServer};{credentials};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

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

        public List<SqlServerInstance> GetDatabaseInstances()
        {
            var list = new List<SqlServerInstance>();
            var instanceTable = SqlManangement.SqlDataSourceTable();

            foreach (DataRow row in instanceTable.Rows)
            {

                string servername;
                string instancename = row["InstanceName"].ToString();

                if (!string.IsNullOrEmpty(instancename))
                {
                    servername = row["ServerName"].ToString() + '\\' + instancename;
                }
                else
                {
                    servername = row["ServerName"].ToString();
                }

                list.Add(new SqlServerInstance() { ServerInstance = servername, Version = row["Version"].ToString() });

            }
            return list;
        }
    }
    public class SqlServerInstance
    {
        public string ServerInstance { get; set; }
        public string Version { get; set; }
    }

}