using System;
using System.Data;
using System.IO;
using System.Text;
using Database.Documentor.htmPages;
using Database.Documentor.Properties;
using Database.Documentor.Providers;
using Database.Documentor.Settings;

namespace Database.Documentor.Utility
{
    public class BuildPageOutput
    {
        private HtmlFunctions htmlFunctions;
        private DbDocSettings dbDocSettings;
        private int tableCount = 0;
        private int viewCount = 0;
        private int procedureCount, functionCount = 0;
        private SchemaProviderFactory spf;
        private SqlServerSchemaProvider sp;

        public BuildPageOutput(HtmlFunctions htmlFunctions,
                                DbDocSettings dbDocSettings,
                                SchemaProviderFactory spf,
                                SqlServerSchemaProvider sp)
        {
            this.htmlFunctions = htmlFunctions;
            this.dbDocSettings = dbDocSettings;
            this.spf = spf;
            this.sp = sp;
        }

        public void CreateHomePage(string filepath, DataSet ds)
        {
            var hp = new HomePage();

            string fileName = (dbDocSettings.DatabaseName + ".Default.htm").Replace(" ", "");
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(ds, dbDocSettings);
            hp.CloseFile();

            htmlFunctions.AddFileToProject(fullPath);
            htmlFunctions.AddFileToContents(dbDocSettings.DatabaseName, fullPath);
        }

        public void CreateIndexPage(string filepath)
        {
            var hp = new IndexPage();

            string fileName = (dbDocSettings.DatabaseName + ".Index.htm").Replace(" ", "");
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(dbDocSettings);
            hp.CloseFile();
        }

        public void CreateContentsPage(string filepath,
                                        DataSet ds)
        {
            var hp = new ContentsPage();
            string fileName = BuildPageFileName(".Contents.htm");
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(ds, dbDocSettings);
            hp.CloseFile();
        }

        public void CreateTables(string filepath,
                                    DataTable dt,
                                    int tableCount,
                                    int viewCount,
                                    int procedureCount,
                                    int functionCount)
        {
            var hp = new TablesPage();

            string fileName = (dbDocSettings.DatabaseName + "." + Resources.TablesText + ".htm").Replace(" ", "");
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(dt, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
            hp.CloseFile();

            htmlFunctions.AddFileToProject(fullPath);
            htmlFunctions.AddFileToContents(Resources.TablesText, fullPath);
        }

        public void CreateTableDetails(string filepath,
                                        DataTable dt,
                                        int tableCount,
                                        int viewCount,
                                        int procedureCount,
                                         int functionCount)
        {
            int pSteps = dt.Rows.Count;
            int pCount = 0;

            string sectionName = (dbDocSettings.DatabaseName + "." + Resources.TablesText + ".htm").Replace(" ", "");
            string sectionPath = Path.Combine(filepath, sectionName);

            foreach (DataRow row in dt.Rows)
            {
                var hp = new TableDetailsPage();

                string objName = System.Convert.ToString(row["TABLE_NAME"]);
                string fileName = (dbDocSettings.DatabaseName + "." + Resources.TableText + "." + objName + ".htm").Replace(" ", "");
                string fullPath = Path.Combine(filepath, fileName);

                hp.FileName = fileName;
                hp.FilePath = filepath;

                DataTable dtColumns = sp.GetColumns(objName);
                DataTable dtKeys = sp.GetPrimaryKeyColumns(objName);
                DataTable dtIndexes = sp.GetIndexes(objName);
                DataTable dtRelationships = sp.GetRelationships(objName);

                hp.OpenFile();
                hp.WriteHTML(row, dtColumns, dtKeys, dtIndexes, dtRelationships, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
                hp.CloseFile();

                htmlFunctions.AddFileToProject(fullPath);

                htmlFunctions.OpenBookInContents();
                htmlFunctions.AddFileToContents(objName, fileName, 11);
                htmlFunctions.CloseBookInContents();
                pCount += 1;
            }
        }

        public void CreateViews(string filepath,
                                DataTable dt,
                                int tableCount,
                                int viewCount,
                                int procedureCount,
                                 int functionCount)
        {
            var hp = new ViewsPage();

            string fileName = dbDocSettings.DatabaseName + "." + Resources.ViewsText + ".htm";
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(dt, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
            hp.CloseFile();

            htmlFunctions.AddFileToProject(fullPath);
            htmlFunctions.AddFileToContents(Resources.ViewsText, fullPath);
        }

        public void CreateViewDetails(string filepath,
                                        DataTable dt,
                                        int tableCount,
                                        int viewCount,
                                        int procedureCount,
                                         int functionCount)

        {
            int pSteps = dt.Rows.Count;
            int pCount = 0;

            string sectionName = (dbDocSettings.DatabaseName + "." + Resources.ViewsText + ".htm").Replace(" ", "");
            string sectionPath = Path.Combine(filepath, sectionName);

            foreach (DataRow row in dt.Rows)
            {
                var hp = new ViewDetailsPage();

                string objName = System.Convert.ToString(row["TABLE_NAME"]);
                string fileName = (dbDocSettings.DatabaseName + "." + Resources.ViewText + "." + objName + ".htm").Replace(" ", "");
                string fullPath = Path.Combine(filepath, fileName);

                hp.FileName = fileName;
                hp.FilePath = filepath;

                DataTable dtViewDetails = sp.GetColumns(objName);

                hp.OpenFile();
                hp.WriteHTML(row, dtViewDetails, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
                hp.CloseFile();

                htmlFunctions.AddFileToProject(fullPath);
                htmlFunctions.OpenBookInContents();
                htmlFunctions.AddFileToContents(objName, fileName, 11);
                htmlFunctions.CloseBookInContents();

                pCount += 1;
            }
        }

        public void CreateProcedures(string filepath, DataTable dt, int tableCount, int viewCount, int procedureCount, int functionCount)
        {
            var hp = new ProceduresPage();

            string fileName = (dbDocSettings.DatabaseName + "." + Resources.ProceduresText + ".htm").Replace(" ", "");
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(dt, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
            hp.CloseFile();

            htmlFunctions.AddFileToProject(fullPath);
            htmlFunctions.AddFileToContents(Resources.ProceduresText, fullPath);
        }

        public void CreateProcedureDetails(string filepath,
            DataTable dt,
            int tableCount,
            int viewCount,
            int procedureCount,
             int functionCount)
        {
            int pSteps = dt.Rows.Count;
            int pCount = 0;

            string sectionName = (dbDocSettings.DatabaseName + "." + Resources.ProceduresText + ".htm").Replace(" ", "");
            string sectionPath = Path.Combine(filepath, sectionName);

            foreach (DataRow row in dt.Rows)
            {
                var hp = new ProcedureDetailsPage();

                string objName = System.Convert.ToString(row["PROCEDURE_NAME"]);
                string fileName = (dbDocSettings.DatabaseName + "." + Resources.ProcedureText + "." + objName + ".htm").Replace(" ", "");
                string fullPath = Path.Combine(filepath, fileName);

                hp.FileName = fileName;
                hp.FilePath = filepath;

                DataTable dtProcedureDetails = sp.GetParameters(objName);

                hp.OpenFile();
                hp.WriteHTML(row, dtProcedureDetails, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
                hp.CloseFile();

                htmlFunctions.AddFileToProject(fullPath);
                htmlFunctions.OpenBookInContents();
                htmlFunctions.AddFileToContents(objName, fileName, 11);
                htmlFunctions.CloseBookInContents();
                pCount += 1;
            }
        }

        public void CreateFunctions(string filepath,
            DataTable dt,
            int tableCount,
            int viewCount,
            int procedureCount,
            int functionCount)
        {
            var hp = new FunctionsPage();

            string fileName = (dbDocSettings.DatabaseName + "." + Resources.FunctionsText + ".htm").Replace(" ", "");
            string fullPath = Path.Combine(filepath, fileName);

            hp.FileName = fileName;
            hp.FilePath = filepath;

            hp.OpenFile();
            hp.WriteHTML(dt, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
            hp.CloseFile();

            htmlFunctions.AddFileToProject(fullPath);
            htmlFunctions.AddFileToContents(Resources.FunctionsText, fullPath);
        }

        public void CreateFunctionsDetails(string filepath,
                                            DataTable dt,
                                            int tableCount,
                                            int viewCount,
                                            int procedureCount,
                                            int functionCount)
        {
            int pSteps = dt.Rows.Count;
            int pCount = 0;

            string sectionName = (dbDocSettings.DatabaseName + "." + Resources.FunctionsText + ".htm").Replace(" ", "");
            string sectionPath = Path.Combine(filepath, sectionName);

            foreach (DataRow row in dt.Rows)
            {
                var hp = new FunctionsDetailsPage();

                string objName = System.Convert.ToString(row["function_name"]);
                string schemaName = System.Convert.ToString(row["schema_name"]);
                string fileName = (dbDocSettings.DatabaseName + "." + Resources.FunctionsText + "." + objName + ".htm").Replace(" ", "");
                string fullPath = Path.Combine(filepath, fileName);

                hp.FileName = fileName;
                hp.FilePath = filepath;

                DataTable dtFunctionsDetails = sp.GetFunction($"[{schemaName}].[{objName}]");

                hp.OpenFile();
                hp.WriteHTML(row, dtFunctionsDetails, dbDocSettings, tableCount, viewCount, procedureCount, functionCount);
                hp.CloseFile();

                htmlFunctions.AddFileToProject(fullPath);
                htmlFunctions.OpenBookInContents();
                htmlFunctions.AddFileToContents(objName, fileName, 11);
                htmlFunctions.CloseBookInContents();
                pCount += 1;
            }
        }

        private string BuildPageFileName(string pageName)
        {
            return string.Concat(dbDocSettings.DatabaseName, pageName).Replace(" ", "");
        }

        private string BuildPageFilePath(string fileName)
        {
            return Path.Combine(dbDocSettings.OutputFolder, fileName);
        }
    }
}