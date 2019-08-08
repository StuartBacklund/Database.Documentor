using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace Database.Documentor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
        }

        public static string CurrentUser { get; private set; }

        static App()
        {
            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            BootStrapper.Instance.Bootstrap(this, e);

            base.OnStartup(e);

            MainWindow window = new MainWindow();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            BootStrapper.Instance.ShutDown(this, e);
            base.OnExit(e);
            Application.Current.Shutdown(0);
        }

        private void ExitApp(object sender, ExitEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
        }
    }
}