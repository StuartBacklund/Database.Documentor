using System.IO;
using Database.Documentor.Properties;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    /// <summary>Base Class for all the classes that produce an HTML Page.</summary>
    public abstract class BasePage
    {
        private string filename;
        private string filepath;
        private DbDocSettings mysettings;
        private int tableCount;
        private int viewCount;
        private int procedureCount;

        protected FileStream fs;

        /// <summary>The name of the output file.</summary>
        /// <value>String containing file name</value>
        public string FileName
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }

        /// <summary>The file output path.</summary>
        /// <value>String containing file path</value>
        public string FilePath
        {
            get
            {
                return filepath;
            }
            set
            {
                filepath = value;
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

        public int TableCount
        {
            get
            {
                return tableCount;
            }
            set
            {
                tableCount = value;
            }
        }

        public int ViewCount
        {
            get
            {
                return viewCount;
            }
            set
            {
                viewCount = value;
            }
        }

        public int ProcedureCount
        {
            get
            {
                return procedureCount;
            }
            set
            {
                procedureCount = value;
            }
        }

        /// <summary>Open/Create the output file.</summary>
        public void OpenFile()
        {
            string path = System.IO.Path.Combine(this.FilePath, this.FileName);

            // Open the stream and read it back.
            fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
        }

        /// <summary>Write html to output file.</summary>
        public abstract void WriteHTML();

        /// <summary>Close the output file.</summary>
        public void CloseFile()
        {
            //fs.Close();
        }

        public string GetFileName(string fileName)
        {
            return (this.MySettings.DatabaseName + "." + fileName + ".htm").Replace(" ", "");
        }

        public string GetTablesFileName()
        {
            return GetFileName(Resources.TablesText);
        }

        public string GetViewsFileName()
        {
            return GetFileName(Resources.ViewsText);
        }

        public string GetProceduresFileName()
        {
            return GetFileName(Resources.ProceduresText);
        }

        public string GetFileName(string fileName, string fileprefix)
        {
            return (this.MySettings.DatabaseName + "." + fileprefix + "." + fileName + ".htm").Replace(" ", "");
        }
    }
}