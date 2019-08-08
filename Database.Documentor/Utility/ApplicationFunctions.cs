using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private string currentProject = "";
        private BuildPageOutput buildPageOutput;
        private DbDocSettings mysettings;
        private SchemaProviderFactory spf;
        private SchemaProvider sp;
        private List<HelpFileContentStructure> listStructure;
        private int tableCount = 0;
        private int viewCount = 0;
        private int procedureCount = 0;
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

            if (mysettings.OutputFolder == "")
            {
                throw new Exception("Output folder not set.");
            }

            filepath = mysettings.OutputFolder;

            try
            {
                if (FileFunctions.PrepareProjectDirectory(filepath, mysettings))
                {
                }
                else
                {
                    sb.AppendLine("Error with application configuration."); ;
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
                }

                if (!(ds.Tables[Resources.ViewsText] == null))
                {
                    viewCount = ds.Tables[Resources.ViewsText].Rows.Count;
                }

                if (!(ds.Tables[Resources.ProceduresText] == null))
                {
                    procedureCount = ds.Tables[Resources.ProceduresText].Rows.Count;
                }

                // Tables
                if (!(ds.Tables[Resources.TablesText] == null) && ds.Tables[Resources.TablesText].Rows.Count > 0)
                {
                    buildPageOutput.CreateTables(filepath, ds.Tables[Resources.TablesText], tableCount, viewCount, procedureCount);
                    buildPageOutput.CreateTableDetails(filepath, ds.Tables[Resources.TablesText], tableCount, viewCount, procedureCount);
                }

                // Views
                if (!(ds.Tables[Resources.ViewsText] == null) && ds.Tables[Resources.ViewsText].Rows.Count > 0)
                {
                    buildPageOutput.CreateViews(filepath, ds.Tables[Resources.ViewsText], tableCount, viewCount, procedureCount);
                    buildPageOutput.CreateViewDetails(filepath, ds.Tables[Resources.ViewsText], tableCount, viewCount, procedureCount);
                }

                // Stored Procedures
                if (!(ds.Tables[Resources.ProceduresText] == null) && ds.Tables[Resources.ProceduresText].Rows.Count > 0)
                {
                    buildPageOutput.CreateProcedures(filepath, ds.Tables[Resources.ProceduresText], tableCount, viewCount, procedureCount);
                    buildPageOutput.CreateProcedureDetails(filepath, ds.Tables[Resources.ProceduresText], tableCount, viewCount, procedureCount);
                }

                htmlhelp.CloseProjectFile();
                htmlhelp.CloseBookInContents();
                htmlhelp.CloseContentsFile();
                htmlhelp.WriteEmptyIndexFile();
                htmlhelp.CompileProject();
            }
            catch (Exception ex)
            {
                throw new Exception("Error builing document." + ex.InnerException);
            }
            finally
            {
                sp.CloseConnection();
                htmlhelp = null;
            }
            return sb.ToString();
        }
    }
}