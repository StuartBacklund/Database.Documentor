using System;
using System.Data;
using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    public class ViewDetailsPage : BasePage
    {
        private DataRow dtrow;
        private DataTable dtColumns;

        public DataRow Dtrow
        {
            get
            {
                return dtrow;
            }
            set
            {
                dtrow = value;
            }
        }

        public DataTable DtColumns
        {
            get
            {
                return dtColumns;
            }
            set
            {
                dtColumns = value;
            }
        }

        public override void WriteHTML()
        {
            StreamWriter s = new StreamWriter(fs);

            string tempDescription = "";
            if (dtrow["Description"] == DBNull.Value || System.Convert.ToString(dtrow["Description"]).Length == 0)
            {
                tempDescription = "&nbsp;";
            }
            else
            {
                tempDescription = System.Convert.ToString(dtrow["Description"]);
            }

            string CurrentViewName = System.Convert.ToString(Dtrow["Table_Name"]);

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            // s.WriteLine("     <meta name='generator' content='" & metaContent & "'>")
            s.WriteLine("     <title>" + CurrentViewName + "</title>");
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
            s.WriteLine("           <h1 class='dtH1'>" + CurrentViewName + " View</h1>");
            s.WriteLine("        </div>");
            s.WriteLine("     </div>");
            s.WriteLine("     <div id='nstext' valign='bottom'>");
            s.WriteLine("        <h4 class='dtH4'>Description</h4>");
            s.WriteLine("		<p>");
            s.WriteLine("			" + tempDescription);
            s.WriteLine("		</p>");

            if (DtColumns.Rows.Count > 0)
            {
                s.WriteLine("     <h4 class='dtH4'>Fields</h4>");
                s.WriteLine("		<div class='tablediv'>");
                s.WriteLine("		<table cellspacing='0' class='dtTABLE' width='100%' border='1'>");
                s.WriteLine("			<tr valign='top' bgcolor='#cccccc'>");
                s.WriteLine("				<th>Name</th>");
                s.WriteLine("				<th>Type</th>");
                s.WriteLine("				<th>Attributes</th>");
                s.WriteLine("				<th width='100%'>Description</th>");
                s.WriteLine("			</tr>");

                foreach (DataRow row in DtColumns.Rows)
                {
                    string cName = System.Convert.ToString(row["COLUMN_NAME"]);

                    string cDataType = "";

                    if (row["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value)
                        cDataType = System.Convert.ToString(row["DATA_TYPE"]);
                    else
                        switch (row["DATA_TYPE"].ToString().ToLower())
                        {
                            case "ntext":
                            case "image":
                            case "text":
                                {
                                    cDataType = System.Convert.ToString(row["DATA_TYPE"]);
                                    break;
                                }

                            default:
                                {
                                    cDataType = System.Convert.ToString(row["DATA_TYPE"]) + "(" + System.Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]) + ")";
                                    break;
                                }
                        }

                    string cAttributes = row["IS_NULLABLE"].ToString().ToLower() == "yes" ? "Nullable" : "&nbsp;";
                    string cDescription = row["DESCRIPTION"] == DBNull.Value ? "&nbsp;" : row["DESCRIPTION"].ToString();

                    s.WriteLine("			<tr valign='top'>");
                    s.WriteLine("				<td nowrap><img class='midvalign' src='Column.gif'>&nbsp;<b>" + cName + "</b></td>");
                    s.WriteLine("				<td nowrap>" + cDataType + "</td>");
                    s.WriteLine("				<td nowrap align='center'>" + cAttributes + "</td>");
                    s.WriteLine("				<td>" + cDescription + "</td>");
                    s.WriteLine("			</tr>");
                }

                s.WriteLine("		</table>");
                s.WriteLine("		</div>");
            }

            s.WriteLine("		<h4 class='dtH4'>Definition</h4>");
            s.WriteLine("		<p><code>");
            s.WriteLine("     " + System.Convert.ToString(Dtrow["VIEW_DEFINITION"]).Replace(System.Environment.NewLine, "<br/>"));
            s.WriteLine("		</code></p>");

            // Tables, Views, Procedures Links
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

        public void WriteHTML(DataRow dtrowinput,
                                DataTable dtColumnsinput,
                                DbDocSettings mySettings,
                                int tableCount,
                                int viewCount,
                                int procedureCount,
                                int functionCount)
        {
            this.Dtrow = dtrowinput;
            this.DtColumns = dtColumnsinput;
            this.MySettings = mySettings;
            this.TableCount = tableCount;
            this.ViewCount = viewCount;
            this.ProcedureCount = procedureCount;
            this.FunctionCount = functionCount;

            this.WriteHTML();
        }
    }
}