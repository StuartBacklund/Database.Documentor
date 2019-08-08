using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Prism.Events;

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

        public void Bootstrap(App app, System.Windows.StartupEventArgs e)
        {
            //Do bootstap here
        }

        public void ShutDown(App app, System.Windows.ExitEventArgs e)
        {
            //Do shutdown cleanup here
        }
    }
}