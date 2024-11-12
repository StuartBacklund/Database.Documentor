using System.IO;
using Database.Documentor.Properties;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    public abstract class BasePage
    {
        private string filename;
        private string filepath;
        private DbDocSettings mysettings;
        private int tableCount;
        private int viewCount;
        private int procedureCount;
        private int functionCount;

        protected FileStream fs;

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

        public int FunctionCount
        {
            get { return functionCount; }
            set { functionCount = value; }
        }

        public void OpenFile()
        {
            string path = System.IO.Path.Combine(this.FilePath, this.FileName);
            fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public abstract void WriteHTML();

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

        public string GetFunctionsFileName()
        {
            return GetFileName(Resources.FunctionsText);
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