using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Prism.Events;
using System.Windows;

namespace Database.Documentor
{
    public class BootStrapper
    {
        private static BootStrapper _instance;

        private BootStrapper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IEventAggregator, EventAggregator>();
        }

        public static BootStrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BootStrapper();
                }
                return (_instance);
            }
        }

        public void Bootstrap(App app, StartupEventArgs e)
        {
            //Do bootstap here
        }

        public void ShutDown(App app, System.Windows.ExitEventArgs e)
        {
            //Do shutdown cleanup here
        }
    }
}