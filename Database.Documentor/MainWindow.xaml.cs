using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Prism.Events;
using GalaSoft.MvvmLight.Ioc;
using Database.Documentor.Commands;
using System;

namespace Database.Documentor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();           
        }


        private void Child01_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}