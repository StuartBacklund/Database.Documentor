using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interactivity;

namespace Database.Documentor.Controls
{
    public class FolderDialogBehavior : Behavior<System.Windows.Controls.Button>
    {
        public string SetterName { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnClick;
        }

        public static readonly DependencyProperty FolderName =
            DependencyProperty.RegisterAttached("FolderName",
            typeof(string), typeof(FolderDialogBehavior));

        public static string GetFolderName(DependencyObject obj)
        {
            return (string)obj.GetValue(FolderName);
        }

        public static void SetFolderName(DependencyObject obj, string value)
        {
            obj.SetValue(FolderName, value);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var currentPath = GetValue(FolderName) as string;
            dialog.SelectedPath = currentPath;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SetValue(FolderName, dialog.SelectedPath);
            }
        }
    }
}