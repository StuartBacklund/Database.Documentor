using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Database.Documentor.Providers;
using Database.Documentor.Settings;
using Database.Documentor.Utility;
using GalaSoft.MvvmLight;
using Prism.Commands;

namespace Database.Documentor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string DataDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public SqlServerConnectionStringBuilder SqlServerConnectionStringBuilder
        {
            get;
            set;
        }

        public ICommand CreateDocumentation
        {
            get
            {
                return new DelegateCommand<object>(this.CreateDocumentationEvent);
            }
        }

        private string databaseName;

        public string DatabaseName
        {
            get { return databaseName; }
            set
            {
                databaseName = value;
                RaisePropertyChanged(() => DatabaseName);
            }
        }

        private string directoryName;

        public string DirectoryName
        {
            get { return directoryName; }
            set
            {
                directoryName = value;
                RaisePropertyChanged(() => DatabaseName);
            }
        }

        private string results;

        public string Results
        {
            get { return results; }
            set
            {
                results = value;
                RaisePropertyChanged(() => Results);
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        private void CreateDocumentationEvent(object parameter)
        {
            var param = (Tuple<object, object, object, object, object>)parameter;

            var serverValue = param.Item1.ToString();
            var dbValue = param.Item2.ToString();
            var isIntegratedValue = (bool)param.Item3;
            var userId = param.Item4 != null ? param.Item4.ToString() : null;
            var password = param.Item5 != null ? param.Item5.ToString() : null;

            DbDocSettings dbDocSettings = new DbDocSettings()
            {
                DatabaseName = dbValue,
            };
            dbDocSettings.LoadDefaults();
            HtmlFunctions htmlFunctions = new HtmlFunctions(dbDocSettings.OutputFolder, dbDocSettings);

            SqlServerConnectionStringBuilder connStringBuilder = null;

            if (isIntegratedValue && userId == null)
            {
                connStringBuilder = new SqlServerConnectionStringBuilder()
                {
                    Database = dbValue,
                    IntegratedSecurity = isIntegratedValue,
                    Server = serverValue,
                };
            }
            else
            {
                connStringBuilder = new SqlServerConnectionStringBuilder()
                {
                    Database = dbValue,
                    IntegratedSecurity = isIntegratedValue,
                    Server = serverValue,
                    UserID = userId,
                    Password = password
                };
            }
            SqlServerSchemaProvider sp = new SqlServerSchemaProvider()
            {
                ConnStringBuilder = connStringBuilder
            };
            SchemaProviderFactory spf = new SchemaProviderFactory();

            BuildPageOutput buildPageOutput = new BuildPageOutput(htmlFunctions, dbDocSettings, spf, sp);
            ApplicationFunctions applicationFunctions = new ApplicationFunctions(buildPageOutput, dbDocSettings, spf, sp, htmlFunctions);

            IsBusy = true;

            Task.Run(() =>
            {
                Results += applicationFunctions.Build();
                IsBusy = false;
            });
        }

        public MainViewModel()
        {
        }
    }
}