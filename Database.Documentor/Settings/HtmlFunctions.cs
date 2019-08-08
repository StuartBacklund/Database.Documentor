using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Database.Documentor.Settings
{
    public class HtmlFunctions
    {
        private string directoryName = null;
        private string defaultTopic = null;
        private string htmlHelpCompiler = null;
        private bool includeFavorites = false;
        private DbDocSettings mysettings;
        private string projectName;
        private StreamWriter sw = null;
        public XmlTextWriter Toc { get; set; }

        /// <summary>Gets the directory name containing the HTML Help files.</summary>
        public string DirectoryName
        {
            get
            {
                return directoryName;
            }
            set
            {
                directoryName = value;
            }
        }

        /// <summary>Gets or sets the path to the HTML Help Compiler.</summary>
        public string HtmlHelpCompiler
        {
            get { return @"C:\Program Files (x86)\HTML Help Workshop\HHC.EXE"; }
            // Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Html Help Workshop\hhc.exe"; }
        }

        /// <summary>Gets or sets the default topic for the compiled HTML Help file.</summary>
        public string DefaultTopic
        {
            get
            {
                return defaultTopic;
            }
            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    defaultTopic = string.Concat(MySettings.DatabaseName, ".Default.htm").Replace(" ", "");
                }
                else
                {
                    defaultTopic = value;
                }
            }
        }

        /// <summary>Gets or sets the IncludeFavorites property.</summary>
        /// <remarks>Setting this to true will include the "favorites" tab
        /// in the compiled HTML Help file.</remarks>
        public bool IncludeFavorites
        {
            get
            {
                return includeFavorites;
            }
            set
            {
                includeFavorites = value;
            }
        }

        public DbDocSettings MySettings
        {
            get
            {
                return mysettings;
            }
            set
            {
                mysettings = value;
            }
        }

        /// <summary>Initializes a new instance of the HtmlHelp class.</summary>
        /// <param name="directoryName">The directory to write the HTML Help files to.</param>
        /// <param name="projectName">The name of the HTML Help project.</param>
        /// <param name="defaultTopic">The default topic for the compiled HTML Help file.</param>
        /// <param name="htmlHelpCompiler">The path to the HTML Help compiler.</param>
        public HtmlFunctions(string directoryName,
                            DbDocSettings mysettings)
        {
            DirectoryName = directoryName;
            MySettings = mysettings;
            projectName = MySettings.DatabaseName.Replace(" ", "");
            sw = new StreamWriter(File.Open(PathToProjectFile(), FileMode.Create));
            Toc = new XmlTextWriter(PathToContentsFile(), null);
        } // New

        private string ProjectFilename()
        {
            return projectName + ".hhp";
        }

        private string ContentsFilename()
        {
            return projectName + ".hhc";
        }

        private string IndexFilename()
        {
            return projectName + ".hhk";
        }

        private string LogFilename()
        {
            return projectName + ".log";
        }

        private string CompiledHtmlFilename()
        {
            return projectName + ".chm";
        }

        private string PathToProjectFile()
        {
            return Path.Combine(directoryName, ProjectFilename());
        }

        private string PathToContentsFile()
        {
            return Path.Combine(directoryName, ContentsFilename());
        }

        private string PathToIndexFile()
        {
            return Path.Combine(directoryName, IndexFilename());
        }

        private string PathToLogFile()
        {
            return Path.Combine(directoryName, LogFilename());
        }

        /// <summary>Gets the path the the CHM file.</summary>
        /// <returns>The path to the CHM file.</returns>
        public string PathToCompiledHtmlFile()
        {
            return Path.Combine(directoryName, CompiledHtmlFilename());
        } // GetPathToCompiledHtmlFile

        /// <summary>Opens project file for writing.</summary>
        public void OpenProjectFile()
        {
            string options;

            if (includeFavorites)
            {
                options = "0x63520,220,0x383e,[86,51,872,558],,,,,,,0";
            }
            else
            {
                options = "0x62520,220,0x383e,[86,51,872,558],,,,,,,0";
            }

            sw.WriteLine("[OPTIONS]");
            sw.WriteLine("Auto Index=Yes");
            sw.WriteLine("Title=" + MySettings.DatabaseName);
            sw.WriteLine("Compatibility=1.1 or later");
            sw.WriteLine("Compiled file=" + CompiledHtmlFilename());
            sw.WriteLine("Contents file=" + ContentsFilename());
            sw.WriteLine("Default Window=MsdnHelp");
            sw.WriteLine("Default topic=" + Path.GetFileName(DefaultTopic));
            sw.WriteLine("Display compile progress=No");
            sw.WriteLine("Error log file=" + LogFilename());
            sw.WriteLine("Full-text search=Yes");
            sw.WriteLine("Index file=" + IndexFilename());
            sw.WriteLine("Language=0x409 English (United States)");
            sw.WriteLine("");
            sw.WriteLine("[WINDOWS]");
            sw.WriteLine("MsdnHelp=\"" + projectName + " Help\",\"" + ContentsFilename() + "\",\"" + IndexFilename() + "\",,,,,,," + options);

            sw.WriteLine("");
            sw.WriteLine("[FILES]");
        }

        /// <summary>Adds a file to project file.</summary>
        /// <param name="filename">The filename to add.</param>
        public void AddFileToProject(string filename)
        {
            sw.WriteLine(filename);
        }

        /// <summary>Closes project file.</summary>
        public void CloseProjectFile()
        {
            sw.WriteLine("");
            sw.WriteLine("[INFOTYPES]");
            sw.Close();
        }

        /// <summary>Opens a contents file for writing.</summary>
        public void OpenContentsFile()
        {
            Toc.WriteStartElement("UL");
        }

        /// <summary>Creates a new "book" in contents file.</summary>
        public void OpenBookInContents()
        {
            Toc.WriteStartElement("UL");
        }

        /// <summary>Adds a file to contents file.</summary>
        /// <param name="headingName">The name as it should appear in the contents.</param>
        /// <param name="htmlFilename">The filename for this entry.</param>
        /// <param name="ImageNumber">The number of the image to be used for this item</param>
        public void AddFileToContents(string headingName, string htmlFilename, int ImageNumber = -1)
        {
            Toc.WriteStartElement("LI");
            Toc.WriteStartElement("OBJECT");
            Toc.WriteAttributeString("type", "text/sitemap");
            Toc.WriteStartElement("param");
            Toc.WriteAttributeString("name", "Name");
            Toc.WriteAttributeString("value", headingName.Replace('$', '.'));
            Toc.WriteEndElement();
            Toc.WriteStartElement("param");
            Toc.WriteAttributeString("name", "Local");
            Toc.WriteAttributeString("value", Path.GetFileName(htmlFilename));
            Toc.WriteEndElement();

            if (ImageNumber >= 0)
            {
                Toc.WriteStartElement("param");
                Toc.WriteAttributeString("name", "ImageNumber");
                Toc.WriteAttributeString("value", ImageNumber.ToString());
                Toc.WriteEndElement();
            }

            Toc.WriteEndElement();
            Toc.WriteEndElement();
        }

        public void CloseBookInContents()
        {
            Toc.WriteEndElement();
        }

        public void CloseContentsFile()
        {
            Toc.WriteEndElement();
            Toc.Flush();
            Toc.Close();
        }

        /// <summary>Creates an empty index file.</summary>
        /// <remarks>HTML Help Compiler needs this file.</remarks>
        public void WriteEmptyIndexFile()
        {
            XmlTextWriter i = new XmlTextWriter(PathToIndexFile(), null/* TODO Change to default() if this is not a reference type */);

            i.WriteStartElement("HTML");
            i.WriteStartElement("BODY");
            i.WriteEndElement();
            i.WriteEndElement();

            i.Close();
        }

        /// <summary>Compiles the HTML Help project.</summary>
        public void CompileProject()
        {
            Process helpCompileProcess = new Process();

            try
            {
                string path = PathToCompiledHtmlFile();
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception e)
                {
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = HtmlHelpCompiler;
                processStartInfo.Arguments = "\"" + Path.GetFullPath(PathToProjectFile()) + "\"";
                processStartInfo.ErrorDialog = false;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.UseShellExecute = false;
                helpCompileProcess.StartInfo = processStartInfo;

                try
                {
                    helpCompileProcess.Start();
                    helpCompileProcess.WaitForExit();
                }
                catch (Exception e)
                {
                    string msg = String.Format("The HTML Help compiler '{0}' was not found.", htmlHelpCompiler);
                }
            }
            finally
            {
                helpCompileProcess.Close();
            }
        }
    }
}