using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Database.Documentor.Commands;
using Database.Documentor.Providers;
using Database.Documentor.Settings;
using Database.Documentor.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Prism.Commands;
using Prism.Events;

namespace Database.Documentor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string DataDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private IEventAggregator eventAggregator;
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

        private bool CanExecute(object arg)
        {
            var param = (Tuple<object, object, object, object, object>)arg;
            return (param != null && param?.Item1?.ToString() != null && param?.Item2?.ToString() != null && param?.Item3?.ToString() != null);
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

        private string textOutput;

        public string TextOutput
        {
            get { return textOutput; }
            set
            {
                textOutput = value;
                RaisePropertyChanged(() => TextOutput);
            }
        }
        private string dialogTitle;

        public string DialogTitle
        {
            get { return dialogTitle; }
            set
            {
                dialogTitle = value;
                RaisePropertyChanged(() => DialogTitle);
            }
        }
        
        private string dialogTextOutput;

        public string DialogTextOutput
        {
            get { return dialogTextOutput; }
            set
            {
                dialogTextOutput = value;
                RaisePropertyChanged(() => DialogTextOutput);
            }
        }

        private void CreateDocumentationEvent(object parameter)
        {
            var param = (Tuple<object, object, object, object, object>)parameter;
            DialogTextOutput = "";
            DialogTitle = "BUILDING DOCUMENTATION";

            if (!CanExecute(param))
            {
                TextOutput += @"Missing information to connect to a database.";
                return;
            }

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
            DialogTextOutput = @"Building documentation, please be patient..";

            var task = Task.Run(() =>
               {
                   TextOutput += applicationFunctions.Build();
                   TextOutput += applicationFunctions.ZipFiles();
                   IsBusy = false;
                   TextOutput += ReadLogFile(dbDocSettings.OutputFolder, dbDocSettings.DatabaseName);
               });
        }

        public MainViewModel(IEventAggregator eventAggregator)             
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<ShowProgressDialogEvent>().Subscribe(ShowProgressDialog);
        }

        private void ShowProgressDialog(object obj)
        {
            if ((bool)obj)
            {
                IsBusy = true;
                DialogTextOutput = @"Searching For Database Instances..";
            }
            else
            {
                IsBusy = false;
            }
           
        }

        private string ReadLogFile(string workingFolder, string projectName)
        {
            string logFile = Path.Combine(workingFolder, projectName.Replace(" ", "") + ".log");
            string line = "Error Opening Log File";

            if (File.Exists(logFile))
            {
                using (StreamReader sr = new StreamReader(logFile))
                {
                    line = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return line;
        }
    }
}