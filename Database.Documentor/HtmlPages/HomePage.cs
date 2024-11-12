using System.Data;
using System.IO;
using Database.Documentor.Properties;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    /// <summary>Produces the Home HTML Page.</summary>
    public class HomePage : BasePage
    {
        private DataSet ds;

        /// <summary>The dataset used to generate the HTML output.</summary>
        /// <value>DataSet containing data needed to produce the output Home Page.</value>
        public DataSet Ds
        {
            get
            {
                return ds;
            }
            set
            {
                ds = value;
            }
        }

        /// <overloads>This method has 2 overloads</overloads>
        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            StreamWriter s = new StreamWriter(fs);

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            // s.WriteLine("     <meta name='generator' content='" & metaContent & "'>")
            s.WriteLine("     <title>" + MySettings.DatabaseName + "</title>");
            s.WriteLine("     <link href='msdn.css' type='text/css' rel='stylesheet' />");
            s.WriteLine("  </head>");
            s.WriteLine("  <body id='bodyID' class='dtBODY' topmargin='0' leftmargin='0' bottommargin='0' rightmargin='0' marginwidth='0' marginheight='0'>");
            s.WriteLine("     <div id='nsbanner'>");
            s.WriteLine("        <div id='bannerrow1'>");
            s.WriteLine("           <table class='bannerparthead' cellspacing=0>");
            s.WriteLine("              <tr id='hdr'>");
            s.WriteLine("                 <td class='runninghead' nowrap>" + MySettings.DatabaseName + " Database Documentation</td>");
            s.WriteLine("                 <td class='product' nowrap>&nbsp;</td>");
            s.WriteLine("              </tr>");
            s.WriteLine("           </table>");
            s.WriteLine("        </div>");
            s.WriteLine("        <div id='TitleRow'>");
            s.WriteLine("           <h1 class='dtH1'>" + MySettings.DatabaseName + " Database Overview</h1>");
            s.WriteLine("        </div>");
            s.WriteLine("     </div>");

            s.WriteLine("     <div id='nstext' valign='bottom'>");
            s.WriteLine("        <h4 class='dtH4'>Description</h4>");

            s.WriteLine("        <ul>");

            if (!(Ds.Tables[Resources.TablesText] == null) && Ds.Tables[Resources.TablesText].Rows.Count > 0)
            {
                s.WriteLine("           <li><a href='" + GetTablesFileName() + "'>Tables</a></li>");
                s.WriteLine("              <ul>");
                foreach (DataRow row in Ds.Tables[Resources.TablesText].Rows)
                {
                    string tableName = System.Convert.ToString(row["TABLE_NAME"]);
                    s.WriteLine("                 <li><a href='" + GetFileName(tableName, "Table") + "'>" + tableName + "</a></li>");
                }
                s.WriteLine("              </ul>");
            }

            if (!(Ds.Tables[Resources.ViewsText] == null) && Ds.Tables[Resources.ViewsText].Rows.Count > 0)
            {
                s.WriteLine("           <li><a href='" + GetViewsFileName() + "'>Views</a></li>");
                s.WriteLine("              <ul>");
                foreach (DataRow row in Ds.Tables[Resources.ViewsText].Rows)
                {
                    string tableName = System.Convert.ToString(row["TABLE_NAME"]);
                    s.WriteLine("                 <li><a href='" + GetFileName(tableName, "View") + "'>" + tableName + "</a></li>");
                }
                s.WriteLine("              </ul>");
            }

            if (!(Ds.Tables[Resources.ProceduresText] == null) && Ds.Tables[Resources.ProceduresText].Rows.Count > 0)
            {
                s.WriteLine("           <li><a href='" + GetProceduresFileName() + "'>Stored Procedures</a></li>");
                s.WriteLine("              <ul>");
                foreach (DataRow row in Ds.Tables[Resources.ProceduresText].Rows)
                {
                    string tableName = System.Convert.ToString(row["PROCEDURE_NAME"]);
                    s.WriteLine("                 <li><a href='" + GetFileName(tableName, "Procedure") + "'>" + tableName + "</a></li>");
                }
                s.WriteLine("              </ul>");
            }

            if (!(Ds.Tables[Resources.FunctionsText] == null) && Ds.Tables[Resources.FunctionsText].Rows.Count > 0)
            {
                s.WriteLine("           <li><a href='" + GetFunctionsFileName() + "'>Functions</a></li>");
                s.WriteLine("              <ul>");
                foreach (DataRow row in Ds.Tables[Resources.FunctionsText].Rows)
                {
                    string tableName = System.Convert.ToString(row["function_name"]);
                    s.WriteLine("                 <li><a href='" + GetFileName(tableName, "Function") + "'>" + tableName + "</a></li>");
                }
                s.WriteLine("              </ul>");
            }

            s.WriteLine("           </ul>");

            s.WriteLine("        <div class='footer'>");
            s.WriteLine("           <hr>");

            s.WriteLine("        </div>");
            s.WriteLine("     </div>");
            s.WriteLine("  </body>");
            s.WriteLine("</html>");

            s.Close();
        }

        /// <summary>Sets class properties then produces the output HTML.</summary>
        /// <param name="ds_input">Dataset to use when writing HTML.</param>
        public void WriteHTML(DataSet dsinput, DbDocSettings mySettings)
        {
            this.ds = dsinput;
            this.MySettings = mySettings;
            this.WriteHTML();
        }
    }
}