namespace Database.Documentor.htmPages
{
    using System;
    using System.Data;
    using System.IO;
    using Database.Documentor.Settings;

    /// <summary>Produces the Tables HTML Page.</summary>
    public class TablesPage : BasePage
    {
        private DataTable dt;

        /// <summary>DataTable containing information about all the tables in the database.</summary>
        /// <value>Datatable containing data needed to produce the output Home Page.</value>
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

        /// <overloads>This method has 2 overloads</overloads>
        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            StreamWriter s = new StreamWriter(fs);

            string databaseName = MySettings.DatabaseName;

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            // s.WriteLine("     <meta name='generator' content='" & metaContent & "'>")
            s.WriteLine("     <title>" + databaseName + " Tables</title>");
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
            s.WriteLine("           <h1 class='dtH1'>" + databaseName + " Database Tables</h1>");
            s.WriteLine("        </div>");
            s.WriteLine("     </div>");
            s.WriteLine("     <div id='nstext' valign='bottom'>");
            s.WriteLine("        <h4 class='dtH4'>Tables</h4>");

            s.WriteLine("		<div class='tablediv'>");
            s.WriteLine("		<table cellspacing='0' class='dtTABLE'>");
            s.WriteLine("			<tr bgcolor='#cccccc'>");
            s.WriteLine("				<th>Name</th>");
            s.WriteLine("				<th width='100%'>Description</th>");
            s.WriteLine("			</tr>");

            foreach (DataRow row in Dt.Rows)
            {
                string tableName = System.Convert.ToString(row["TABLE_NAME"]);

                string tempDescription = "";
                if (row["DESCRIPTION"] == DBNull.Value || System.Convert.ToString(row["DESCRIPTION"]).Length == 0)
                {
                    tempDescription = "&nbsp;";
                }
                else
                {
                    tempDescription = System.Convert.ToString(row["DESCRIPTION"]);
                }

                s.WriteLine("			<tr valign='top'>");
                s.WriteLine("				<td nowrap><img class='midvalign' src='Table.gif'>&nbsp;<a href='" + GetFileName(tableName, "Table") + "'>" + tableName + "</a></td>");
                s.WriteLine("				<td>" + tempDescription + "</td>");
                s.WriteLine("			</tr>");
            }

            s.WriteLine("		</table>");
            s.WriteLine("		</div>");

            // Tables, Views, Procedures Links
            s.WriteLine("		<h4 class='dtH4'>See Also</h4>");
            s.WriteLine("		<p>");

            if (ViewCount > 0)
            {
                s.WriteLine("<a href='" + GetViewsFileName() + "'>Views</a>");
            }

            if (ProcedureCount > 0)
            {
                s.WriteLine(" | ");
                s.WriteLine("<a href='" + GetProceduresFileName() + "'>Stored Procedures</a>");
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
        /// <param name="dt_input">DataTable containing information about all the tables in the database.</param>
        public void WriteHTML(DataTable dtinput,
                                DbDocSettings mySettings,
                                int tableCount,
                                int viewCount,
                                int procedureCount)
        {
            this.Dt = dtinput;
            this.MySettings = mySettings;
            this.TableCount = tableCount;
            this.ViewCount = viewCount;
            this.ProcedureCount = procedureCount;

            this.WriteHTML();
        }
    }
}