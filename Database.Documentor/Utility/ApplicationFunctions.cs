using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Database.Documentor.Properties;
using Database.Documentor.Providers;
using Database.Documentor.Settings;

namespace Database.Documentor.Utility
{
    public class ApplicationFunctions
    {
        private string localEnviromentPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private string filepath;
        private HtmlFunctions htmlhelp;
        private BuildPageOutput buildPageOutput;
        private DbDocSettings mysettings;
        private SchemaProviderFactory spf;
        private SchemaProvider sp;
        private int tableCount = 0;
        private int viewCount = 0;
        private int procedureCount, functionCount = 0;
        private StringBuilder sb = new StringBuilder();

        public ApplicationFunctions(BuildPageOutput buildPageOutput,
                                    DbDocSettings dbDocSettings,
                                    SchemaProviderFactory spf,
                                    SqlServerSchemaProvider sp,
                                    HtmlFunctions htmlhelp)
        {
            this.buildPageOutput = buildPageOutput;
            this.mysettings = dbDocSettings;
            this.htmlhelp = htmlhelp;
            this.spf = spf;
            this.sp = sp;
        }

        public string Build()
        {
            sb.AppendLine("Application started");

            try
            {
                if (mysettings.OutputFolder == "")
                {
                    throw new Exception("Output folder not set.");
                }

                filepath = mysettings.OutputFolder;

                if (FileFunctions.PrepareProjectDirectory(filepath, mysettings))
                {
                    sb.AppendLine("Äpplication configuration complete.");
                }
                else
                {
                    throw new Exception("Error with application configuration.");
                }

                DataSet ds = new DataSet("Data");
                sp.OpenConnection();

                DataTable dtTables = sp.GetTables();
                if (!(dtTables == null))
                {
                    dtTables.TableName = Resources.TablesText;
                    ds.Tables.Add(dtTables);
                }

                DataTable dtViews = sp.GetViews();
                if (dtViews != null)
                {
                    dtViews.TableName = Resources.ViewsText;
                    ds.Tables.Add(dtViews);
                }

                DataTable dtProcedures = sp.GetProcedures();
                if (dtProcedures != null)
                {
                    dtProcedures.TableName = Resources.ProceduresText;
                    ds.Tables.Add(dtProcedures);
                }

                DataTable dtFunctions = sp.GetFunctions();
                if (dtFunctions != null)
                {
                    dtFunctions.TableName = Resources.FunctionsText;
                    ds.Tables.Add(dtFunctions);
                }

                string htmlhelpcompiler = Path.Combine(localEnviromentPath, "Html Help Workshop", "hhc.exe");

                htmlhelp.OpenProjectFile();
                htmlhelp.OpenContentsFile();
                htmlhelp.OpenBookInContents();

                buildPageOutput.CreateHomePage(filepath, ds);

                buildPageOutput.CreateIndexPage(filepath);
                buildPageOutput.CreateContentsPage(filepath, ds);

                if (!(ds.Tables[Resources.TablesText] == null))
                {
                    tableCount = ds.Tables[Resources.TablesText].Rows.Count;
                    sb.AppendLine($"Table count : {tableCount}");
                }

                if (!(ds.Tables[Resources.ViewsText] == null))
                {
                    viewCount = ds.Tables[Resources.ViewsText].Rows.Count;
                    sb.AppendLine($"View count : {viewCount}");
                }

                if (!(ds.Tables[Resources.ProceduresText] == null))
                {
                    procedureCount = ds.Tables[Resources.ProceduresText].Rows.Count;
                    sb.AppendLine($"Stored procedure count : {procedureCount}");
                }

                if (!(ds.Tables[Resources.FunctionsText] == null))
                {
                    functionCount = ds.Tables[Resources.FunctionsText].Rows.Count;
                    sb.AppendLine($"User defined function count : {functionCount}");
                }
                // Tables
                if (!(ds.Tables[Resources.TablesText] == null) && ds.Tables[Resources.TablesText].Rows.Count > 0)
                {
                    buildPageOutput.CreateTables(filepath, ds.Tables[Resources.TablesText], tableCount, viewCount, procedureCount, functionCount);
                    buildPageOutput.CreateTableDetails(filepath, ds.Tables[Resources.TablesText], tableCount, viewCount, procedureCount, functionCount);
                    sb.AppendLine($"Table pages created.");
                }

                // Views
                if (!(ds.Tables[Resources.ViewsText] == null) && ds.Tables[Resources.ViewsText].Rows.Count > 0)
                {
                    buildPageOutput.CreateViews(filepath, ds.Tables[Resources.ViewsText], tableCount, viewCount, procedureCount, functionCount);
                    buildPageOutput.CreateViewDetails(filepath, ds.Tables[Resources.ViewsText], tableCount, viewCount, procedureCount, functionCount);
                    sb.AppendLine($"View pages created.");
                }

                // Stored Procedures
                if (!(ds.Tables[Resources.ProceduresText] == null) && ds.Tables[Resources.ProceduresText].Rows.Count > 0)
                {
                    buildPageOutput.CreateProcedures(filepath, ds.Tables[Resources.ProceduresText], tableCount, viewCount, procedureCount, functionCount);
                    buildPageOutput.CreateProcedureDetails(filepath, ds.Tables[Resources.ProceduresText], tableCount, viewCount, procedureCount, functionCount);
                    sb.AppendLine($"Stored prodcedure pages created.");
                }

                // Stored Procedures
                if (!(ds.Tables[Resources.FunctionsText] == null) && ds.Tables[Resources.FunctionsText].Rows.Count > 0)
                {
                    buildPageOutput.CreateFunctions(filepath, ds.Tables[Resources.FunctionsText], tableCount, viewCount, procedureCount, functionCount);
                    buildPageOutput.CreateFunctionsDetails(filepath, ds.Tables[Resources.FunctionsText], tableCount, viewCount, procedureCount, functionCount);
                    sb.AppendLine($"Function pages created.");
                }

                htmlhelp.CloseProjectFile();
                htmlhelp.CloseBookInContents();
                htmlhelp.CloseContentsFile();
                htmlhelp.WriteEmptyIndexFile();
                htmlhelp.CompileProject();

                sb.AppendLine($"Chm document created.");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Application error: {ex.InnerException.ToString()}");
            }
            finally
            {
                sp.CloseConnection();
                htmlhelp = null;
            }

            return sb.ToString();
        }

        public string ZipFiles()
        {
            sb.Clear();

            var path = new DirectoryInfo(this.mysettings.OutputFolder);
            var htmOutputFiles = path.GetFiles().Where(x => x.Extension == ".htm" &&
                                 x.Name.ToUpper().Contains(this.mysettings.DatabaseName.Replace(" ", "").ToUpper()))
                                 .OrderBy(n => n.Name).ToList();

            var zipFile = $"{this.mysettings.DatabaseName}.zip";

            sb.AppendLine($"Creating zip archive {zipFile}");
            var targetFile = Path.Combine(this.mysettings.OutputFolder, zipFile);

            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            using (ZipArchive zip = ZipFile.Open($"{zipFile}", ZipArchiveMode.Create))
            {
                foreach (var item in htmOutputFiles)
                {
                    zip.CreateEntryFromFile($"{this.mysettings.OutputFolder}\\{item.Name}", item.Name);
                }
            }

            sb.AppendLine("Files zipped.");

            foreach (FileInfo oFile in htmOutputFiles)
            {
                if (File.Exists(oFile.FullName))
                {
                    File.Delete(oFile.FullName);
                }
            }
            sb.AppendLine("File clean up complete.");
            return sb.ToString();
        }
    }
}