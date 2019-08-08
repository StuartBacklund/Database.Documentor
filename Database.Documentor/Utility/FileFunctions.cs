using System;
using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.Utility
{
    public static class FileFunctions
    {
        public static bool PrepareProjectDirectory(string directoryName, DbDocSettings mysettings)
        {
            if (!Directory.Exists(directoryName))
            {
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch
                {
                    return false;
                }
            }
            string appDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            Directory.SetCurrentDirectory(mysettings.OutputFolder);
            try
            {
                CopyFile(Path.Combine(appDirectory, "Resources", mysettings.CssStyleSheet), "Msdn.css");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.ColumnImage), "Column.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.DatabaseImage), "Database.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.IndexImage), "Index.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.ParameterImage), "Parameter.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.PKColumnImage), "PKColumn.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.ProcedureImage), "Procedure.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.TableImage), "Table.Gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.ViewImage), "View.Gif");

                CopyFile(Path.Combine(appDirectory, "Resources", mysettings.TreeViewJavascript), "tree.js");
                CopyFile(Path.Combine(appDirectory, "Resources", mysettings.TreeViewStyleSheet), "tree.css");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.TreeNodeDotImage), "treenodedot.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.TreeNodeOpenImage), "treenodeminus.gif");
                CopyFile(Path.Combine(appDirectory, "Images", mysettings.TreeNodeClosedImage), "treenodeplus.gif");
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private static bool CopyFile(string source, string target)
        {
            try
            {
                File.Copy(source, target, true);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static string FormatOutputforHTML(string source)
        {
            // source = System.Web.HttpUtility.htmEncode(source)
            // source = source.Replace(System.Environment.NewLine, "<br>")
            // source = source.Replace(vbTab, "&nbsp;&nbsp;&nbsp;")
            return source;
        }
    }
}