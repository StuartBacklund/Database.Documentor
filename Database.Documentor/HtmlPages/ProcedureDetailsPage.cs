﻿using System;
using System.Data;
using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    /// <summary>Produces the Procedure Details HTML Page.</summary>
    public class ProcedureDetailsPage : BasePage
    {
        private DataRow dtRow;
        private DataTable dtParameters;

        /// <summary>Datarow containg details of Procedure Information.</summary>
        public DataRow Dtrow
        {
            get
            {
                return dtRow;
            }
            set
            {
                dtRow = value;
            }
        }

        /// <summary>Datarow containg details of Procedure Parameters.</summary>
        public DataTable DtParameters
        {
            get
            {
                return dtParameters;
            }
            set
            {
                dtParameters = value;
            }
        }

        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            StreamWriter s = new StreamWriter(fs);

            string tempDescription = "";
            if (Dtrow["DESCRIPTION"] == DBNull.Value || System.Convert.ToString(Dtrow["DESCRIPTION"]).Length == 0)
            {
                tempDescription = "&nbsp;";
            }
            else
            {
                tempDescription = System.Convert.ToString(Dtrow["DESCRIPTION"]);
            }

            string currentProcedureName = System.Convert.ToString(Dtrow["PROCEDURE_NAME"]);

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            // s.WriteLine("     <meta name='generator' content='" & metaContent & "'>")
            s.WriteLine("     <title>" + currentProcedureName + "</title>");
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
            s.WriteLine("           <h1 class='dtH1'>" + currentProcedureName + " Stored Procedure</h1>");
            s.WriteLine("        </div>");
            s.WriteLine("     </div>");
            s.WriteLine("     <div id='nstext' valign='bottom'>");
            s.WriteLine("        <h4 class='dtH4'>Description</h4>");
            s.WriteLine("		<p>");
            s.WriteLine("			" + tempDescription);
            s.WriteLine("		</p>");

            s.WriteLine("     <h4 class='dtH4'>Parameters</h4>");

            if (!(DtParameters == null) && DtParameters.Rows.Count > 0)
            {
                s.WriteLine("		<div class='tablediv'>");
                s.WriteLine("		<table cellspacing='0' class='dtTABLE' width='100%' border='1'>");
                s.WriteLine("			<tr valign='top' bgcolor='#cccccc'>");
                s.WriteLine("				<th>Name</th>");
                s.WriteLine("				<th>Type</th>");
                s.WriteLine("				<th>Direction</th>");
                s.WriteLine("				<th width='100%'>Description</th>");
                s.WriteLine("			</tr>");

                foreach (DataRow row in DtParameters.Rows)
                {
                    string cName = System.Convert.ToString(row["PARAMETER_NAME"]);
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

                    string cAttributes = row["PARAMETER_DIRECTION"].ToString().ToLower() == "in" ? "Input" : "Output";
                    string cDescription = row["DESCRIPTION"] == DBNull.Value ? "&nbsp;" : row["DESCRIPTION"].ToString();//System.Convert.ToString(IIf(row["DESCRIPTION"] == DBNull.Value, "&nbsp;", row["DESCRIPTION"]));

                    s.WriteLine("			<tr valign='top'>");
                    s.WriteLine("				<td nowrap><img class='midvalign' src='Parameter.gif'>&nbsp;<b>" + cName + "</b></td>");
                    s.WriteLine("				<td nowrap>" + cDataType + "</td>");
                    s.WriteLine("				<td nowrap align='center'>" + cAttributes + "</td>");
                    s.WriteLine("				<td>" + cDescription + "</td>");
                    s.WriteLine("			</tr>");
                }

                s.WriteLine("		</table>");
                s.WriteLine("		</div>");
            }
            else
            {
                s.WriteLine("		<p>");
                s.WriteLine("			&nbsp;");
                s.WriteLine("		</p>");
            }

            s.WriteLine("		<h4 class='dtH4'>Definition</h4>");
            s.WriteLine("		<p><code>");
            s.WriteLine("     " + System.Convert.ToString(Dtrow["PROCEDURE_DEFINITION"]).Replace(System.Environment.NewLine, "<br/>"));
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

            if (FunctionCount > 0)
            {
                s.WriteLine(" | ");
                s.WriteLine("<a href='" + GetFunctionsFileName() + "'>Functions</a>");
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
        /// <param name="ds_input">Datarow containg details of Procedure Information.</param>
        /// <param name="dtParameters_input">DataRow containing Parameter data needed to produce the output Page.</param>
        public void WriteHTML(DataRow dtrowinput,
                                DataTable dtParametersinput,
                                DbDocSettings mySettings,
                                int tableCount,
                                int viewCount,
                                int procedureCount,
                                int functionCount)
        {
            this.Dtrow = dtrowinput;
            this.DtParameters = dtParametersinput;
            this.MySettings = mySettings;
            this.TableCount = tableCount;
            this.ViewCount = viewCount;
            this.ProcedureCount = procedureCount;
            this.FunctionCount = functionCount;

            this.WriteHTML();
        }
    }
}