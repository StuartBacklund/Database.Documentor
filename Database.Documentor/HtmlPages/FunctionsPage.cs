﻿using System;
using System.Data;
using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    public class FunctionsPage : BasePage
    {
        private DataTable dt;

        /// <summary>Datatable containg details of Functions for the database.</summary>
        public DataTable Dt
        {
            get
            {
                return dt;
            }
            set
            {
                dt = value;
            }
        }

        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            StreamWriter s = new StreamWriter(fs);

            string databaseName = MySettings.DatabaseName;

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            // s.WriteLine("     <meta name='generator' content='" & metaContent & "'>")
            s.WriteLine("     <title>" + databaseName + " Functions</title>");
            s.WriteLine("     <link href='msdn.css' type='text/css' rel='stylesheet' />");
            s.WriteLine("  </head>");
            s.WriteLine("  <body id='bodyID' class='dtBODY' topmargin='0' leftmargin='0' bottommargin='0' rightmargin='0' marginwidth='0' marginheight='0'>");
            s.WriteLine("     <div id='nsbanner'>");
            s.WriteLine("        <div id='bannerrow1'>");
            s.WriteLine("           <table class='bannerparthead' cellspacing=0>");
            s.WriteLine("              <tr id='hdr'>");
            s.WriteLine("                 <td class='runninghead' nowrap>" + databaseName + " Database Documentation</td>");
            s.WriteLine("                 <td class='product' nowrap>&nbsp;</td>");
            s.WriteLine("              </tr>");
            s.WriteLine("           </table>");
            s.WriteLine("        </div>");
            s.WriteLine("        <div id='TitleRow'>");
            s.WriteLine("           <h1 class='dtH1'>" + databaseName + " Database Functions</h1>");
            s.WriteLine("        </div>");
            s.WriteLine("     </div>");
            s.WriteLine("     <div id='nstext' valign='bottom'>");
            s.WriteLine("        <h4 class='dtH4'>Functions</h4>");

            s.WriteLine("		<div class='tablediv'>");
            s.WriteLine("		<table cellspacing='0' class='dtTABLE'>");
            s.WriteLine("			<tr bgcolor='#cccccc'>");
            s.WriteLine("				<th>Name</th>");
            s.WriteLine("				<th width='100%'>Description</th>");
            s.WriteLine("			</tr>");

            foreach (DataRow row in Dt.Rows)
            {
                string RoutineName = System.Convert.ToString(row["function_name"]);
                string Description = "";
                if (!(row["DESCRIPTION"] == DBNull.Value))
                    Description = System.Convert.ToString(row["DESCRIPTION"]);
                s.WriteLine("			<tr valign='top'>");
                s.WriteLine("				<td nowrap><img class='midvalign' src='Procedure.gif'>&nbsp;<a href='" + GetFileName(RoutineName, "Functions") + "'>" + RoutineName + "</a></td>");
                s.WriteLine("				<td>" + Description + "</td>");
                s.WriteLine("			</tr>");
            }

            s.WriteLine("		</table>");
            s.WriteLine("		</div>");

            // Tables, Views, Functions Links
            s.WriteLine("		<h4 class='dtH4'>See Also</h4>");
            s.WriteLine("		<p>");
            if (TableCount > 0)
            {
                s.WriteLine("<a href='" + GetTablesFileName() + "'>Tables</a>");
            }

            if (ViewCount > 0)
            {
                s.WriteLine(" | ");
                s.WriteLine("<a href='" + GetViewsFileName() + "'>Views</a>");
            }

            if (ProcedureCount > 0)
            {
                s.WriteLine(" | ");
                s.WriteLine("<a href='" + GetProceduresFileName() + "'>Procedures</a>");
            }
            s.WriteLine("</p>");
            // End Links

            s.WriteLine("		<div class='footer'>");
            s.WriteLine("			<hr>");
            s.WriteLine("		</div>");

            s.WriteLine("		</div>");
            s.WriteLine("	</body>");
            s.WriteLine("</html>");

            s.Close();
        }

        /// <summary>Sets class properties then produces the output HTML.</summary>
        /// <param name="dt_input">Datatable containg details of Functions for the database.</param>
        public void WriteHTML(DataTable dt_input,
                                DbDocSettings mySettings,
                                int tableCount,
                                int viewCount,
                                int procedureCount,
                                int functionCount)
        {
            this.Dt = dt_input;
            this.MySettings = mySettings;
            this.TableCount = tableCount;
            this.ViewCount = viewCount;
            this.ProcedureCount = procedureCount;
            this.FunctionCount = functionCount;
            this.WriteHTML();
        }
    }
}