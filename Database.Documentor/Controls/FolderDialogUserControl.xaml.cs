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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Database.Documentor.Controls
{
    /// <summary>
    /// Interaction logic for FolderDialogUserControl.xaml
    /// </summary>
    public partial class FolderDialogUserControl : UserControl
    {
        public FolderDialogUserControl()
        {
            InitializeComponent();
        }

        private void ButtonTaskDialogIconFix_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new TaskDialog
            {
                Caption = "Caption",
                DetailsCollapsedLabel = "DetailsCollapsedLabel",
                DetailsExpanded = true,
                DetailsExpandedLabel = "DetailsExpandedLabel",
                DetailsExpandedText = "DetailsExpandedText",
                ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandContent,
                FooterCheckBoxChecked = true,
                FooterCheckBoxText = "FooterCheckBoxText",
                FooterIcon = TaskDialogStandardIcon.Information,
                FooterText = "FooterText",
                HyperlinksEnabled = true,
                Icon = TaskDialogStandardIcon.Shield,
                InstructionText = "InstructionText",
                ProgressBar = new TaskDialogProgressBar { Value = 100 },
                StandardButtons =
                    TaskDialogStandardButtons.Ok | TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No |
                    TaskDialogStandardButtons.Cancel | TaskDialogStandardButtons.Close | TaskDialogStandardButtons.Retry,
                StartupLocation = TaskDialogStartupLocation.CenterScreen,
                Text = "Text"
            })
            {
                dialog.Show();
            }
        }

        private void ButtonTaskDialogCustomButtonClose_Click(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(sender as Window);
            var td = new TaskDialog { OwnerWindowHandle = helper.Handle };

            var closeLink = new TaskDialogCommandLink("close", "Close", "Closes the task dialog");
            closeLink.Click += (o, ev) => td.Close(TaskDialogResult.CustomButtonClicked);
            var closeButton = new TaskDialogButton("closeButton", "Close");
            closeButton.Click += (o, ev) => td.Close(TaskDialogResult.CustomButtonClicked);

            // Enable one or the other; can't have both at the same time
            td.Controls.Add(closeLink);
            //td.Controls.Add(closeButton);

            // needed since none of the buttons currently closes the TaskDialog
            td.Cancelable = true;

            switch (td.Show())
            {
                case TaskDialogResult.CustomButtonClicked:
                    MessageBox.Show("The task dialog was closed by a custom button");
                    break;

                case TaskDialogResult.Cancel:
                    MessageBox.Show("The task dialog was canceled");
                    break;

                default:
                    MessageBox.Show("The task dialog was closed by other means");
                    break;
            }
        }
    }
}