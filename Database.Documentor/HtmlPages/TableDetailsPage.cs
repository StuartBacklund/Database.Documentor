using System;
using System.Data;
using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    /// <summary>Base Class for all the classes that produce an HTML Page.</summary>
    public class TableDetailsPage : BasePage
    {
        private DataRow dtrow;
        private DataTable dtColumns;
        private DataTable dtKeys;
        private DataTable dtIndexes;
        private DataTable dtRelationships;

        /// <summary>Datarow with Information for the Table.</summary>
        /// <value>Datarow containing data needed to produce the output Page.</value>
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

        /// <summary>DataTable with details of the Table's columns.</summary>
        /// <value>DataTable containing data needed to produce the output Page.</value>
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

        /// <summary>DataTable with details of the Table's Primary Key columns.</summary>
        /// <value>DataTable containing data needed to produce the output Page.</value>
        public DataTable DtKeys
        {
            get
            {
                return dtKeys;
            }
            set
            {
                dtKeys = value;
            }
        }

        /// <summary>DataTable with details of the Table's Indexes.</summary>
        /// <value>DataTable containing data needed to produce the output Page.</value>
        public DataTable DtIndexes
        {
            get
            {
                return dtIndexes;
            }
            set
            {
                dtIndexes = value;
            }
        }

        /// <summary>DataTable with details of the Table's Relationships.</summary>
        /// <value>DataTable containing data needed to produce the output Page.</value>
        public DataTable DtRelationships
        {
            get
            {
                return dtRelationships;
            }
            set
            {
                dtRelationships = value;
            }
        }

        /// <overloads>This method has 2 overloads</overloads>
        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            StreamWriter s = new StreamWriter(fs);

            string tempDescription = "";
            /* if (dtrow["Description"] == DBNull.Value || System.Convert.ToString(dtrow["DESCRIPTION"]).Length == 0)
                 tempDescription = "&nbsp;";
             else
                 tempDescription = System.Convert.ToString(dtrow["DESCRIPTION"]);*/

            string currentTableName = System.Convert.ToString(Dtrow["TABLE_NAME"]);

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            // s.WriteLine("     <meta name='generator' content='" & metaContent & "'>")
            s.WriteLine("     <title>" + currentTableName + "</title>");
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
            s.WriteLine("           <h1 class='dtH1'>" + currentTableName + " Table</h1>");
            s.WriteLine("        </div>");
            s.WriteLine("     </div>");

            s.WriteLine("     <div id='nstext' valign='bottom'>");
            s.WriteLine("        <h4 class='dtH4'>Description</h4>");
            s.WriteLine("		<p>");
            s.WriteLine("			" + tempDescription);
            s.WriteLine("		</p>");

            if (!(DtColumns == null) && DtColumns.Rows.Count > 0)
            {
                s.WriteLine("     <h4 class='dtH4'>Fields</h4>");
                s.WriteLine("		<div class='tablediv'>");
                s.WriteLine("		<table cellspacing='0' class='dtTABLE' width='100%' border='1'>");
                s.WriteLine("			<tr valign='top' bgcolor='#cccccc'>");
                s.WriteLine("				<th>Name</th>");
                s.WriteLine("				<th>Type</th>");
                s.WriteLine("				<th>Attributes</th>");
                s.WriteLine("				<th>Default</th>");
                s.WriteLine("				<th width='100%'>Description</th>");
                s.WriteLine("			</tr>");

                foreach (DataRow row in DtColumns.Rows)
                {
                    string cImage = "Column";
                    string cName = System.Convert.ToString(row["COLUMN_NAME"]);

                    foreach (DataRow rowKey in DtKeys.Rows)
                    {
                        if (cName == System.Convert.ToString(rowKey["COLUMN_NAME"]))
                        {
                            cImage = "PkColumn";
                        }
                    }

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

                    string cAttributes;
                    switch (true)
                    {
                        case object _ when (row["IS_NULLABLE"].ToString().ToLower() == "yes" && Convert.ToInt32(row["IS_IDENTITY"].ToString()) == 1):
                            {
                                cAttributes = "Auto Increment <br>Nullable";
                                break;
                            }

                        case object _ when (row["IS_NULLABLE"].ToString().ToLower() == "yes"):
                            {
                                cAttributes = "Nullable";
                                break;
                            }

                        case object _ when System.Convert.ToInt32(row["IS_IDENTITY"]) == 1:
                            {
                                cAttributes = "Auto Increment";
                                break;
                            }

                        default:
                            {
                                cAttributes = "&nbsp;";
                                break;
                            }
                    }

                    string cDefault = row["COLUMN_DEFAULT"] == DBNull.Value ? "&nbsp;" : row["COLUMN_DEFAULT"].ToString();
                    string cDescription = "";// row["DESCRIPTION"] == DBNull.Value ? "&nbsp;" : row["DESCRIPTION"].ToString();//System.Convert.ToString(IIf(row["DESCRIPTION"] == DBNull.Value, "&nbsp;", row["DESCRIPTION"]));

                    s.WriteLine("			<tr valign='top'>");
                    s.WriteLine("				<td nowrap><img class='midvalign' src='" + cImage + ".gif'>&nbsp;<b>" + cName + "</b></td>");
                    s.WriteLine("				<td nowrap>" + cDataType + "</td>");
                    s.WriteLine("				<td nowrap align='center'>" + cAttributes + "</td>");
                    s.WriteLine("				<td nowrap>" + cDefault + "</td>");
                    s.WriteLine("				<td>" + cDescription + "</td>");
                    s.WriteLine("			</tr>");
                }

                s.WriteLine("		</table>");
                s.WriteLine("		</div>");
            }

            // INDEX SECTION
            if (!(DtIndexes == null) && DtIndexes.Rows.Count > 0)
            {
                s.WriteLine("        <h4 class='dtH4'>Indexes</h4>");
                s.WriteLine("        <div class='tablediv'>");
                s.WriteLine("        <table cellspacing='0' class='dtTABLE' width='100%' border='1'>");
                s.WriteLine("           <tr valign='top' bgcolor='#cccccc'>");
                s.WriteLine("         	   <th>Name</th>");
                s.WriteLine("         	   <th nowrap>Columns</th>");
                s.WriteLine("         	   <th nowrap>Unique</th>");
                s.WriteLine("         	   <th nowrap>Clustered</th>");
                s.WriteLine("           </tr>");

                foreach (DataRow row in DtIndexes.Rows)
                {
                    string iName = System.Convert.ToString(row["INDEX_NAME"]);
                    string iColumns = System.Convert.ToString(row["COLUMN_NAME"]);
                    string iUnique = System.Convert.ToString(row["UNIQUE"]);
                    string iClustered = System.Convert.ToString(row["CLUSTERED"]);

                    s.WriteLine("           <tr>");
                    s.WriteLine("         	   <td><img class='midvalign' src='Index.gif'>&nbsp;<b>" + iName + "</b></td>");
                    s.WriteLine("        	   <td>" + iColumns + "</td>");
                    s.WriteLine("         	   <td>" + iUnique + "</td>");
                    s.WriteLine("         	   <td>" + iClustered + "</td>");
                    s.WriteLine("           </tr>");
                }

                s.WriteLine("         </table>");
                s.WriteLine("         </div>");
            }

            if (!(DtRelationships == null) && DtRelationships.Rows.Count > 0)
            {
                s.WriteLine("		<h4 class='dtH4'>Relationships</h4>");
                s.WriteLine("		<div class='tablediv'>");
                s.WriteLine("		<table cellspacing='0' class='dtTABLE' width='100%' border='1'>");
                s.WriteLine("			<tr valign='top' bgcolor='#cccccc'>");
                s.WriteLine("				<th colspan=2 nowrap>Master</th>");
                s.WriteLine("				<th colspan=2 nowrap>Detail</th>");
                s.WriteLine("			</tr>");
                s.WriteLine("			<tr valign='top' bgcolor='#cccccc'>");
                s.WriteLine("				<th>Table</th>");
                s.WriteLine("				<th>Columns</th>");
                s.WriteLine("				<th>Table</th>");
                s.WriteLine("				<th>Columns</th>");
                s.WriteLine("			</tr>");

                foreach (DataRow row in DtRelationships.Rows)
                {
                    string masterTable = System.Convert.ToString(row["PK_TABLE_NAME"]);
                    string masterColumn = System.Convert.ToString(row["PK_COLUMN_NAME"]);
                    string detailsTable = System.Convert.ToString(row["FK_TABLE_NAME"]);
                    string detailsColumn = System.Convert.ToString(row["FK_COLUMN_NAME"]);

                    s.WriteLine("			<tr valign='top'>");

                    if (masterTable == currentTableName)
                        s.WriteLine("				<td nowrap>" + masterTable + "</td>");
                    else
                        s.WriteLine("				<td nowrap><a href='" + GetFileName(masterTable, "Table") + "'>" + masterTable + "</a></td>");
                    s.WriteLine("				<td nowrap>" + masterColumn + "</td>");

                    if (detailsTable == currentTableName)
                        s.WriteLine("				<td nowrap>" + detailsTable + "</td>");
                    else
                        s.WriteLine("				<td nowrap><a href='" + GetFileName(detailsTable, "Table") + "'>" + detailsTable + "</a></td>");

                    s.WriteLine("				<td nowrap>" + detailsColumn + "</td>");
                    s.WriteLine("			</tr>");
                }

                s.WriteLine("		</table>");
                s.WriteLine("		</div>");
            }

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

        /// <summary>Sets class properties then produces the output HTML.</summary>
        /// <param name="dtrowinput">Datarow with Information for the Table.</param>
        /// <param name="dtColumnsinput">DataTable with details of the Table's columns.</param>
        /// <param name="dtKeysinput">DataTable with details of the Table's Primary Key columns.</param>
        /// <param name="dtIndexesinput">DataTable with details of the Table's Indexes.</param>
        /// <param name="dtRelationshipsinput">DataTable with details of the Table's Relationships.</param>
        public void WriteHTML(DataRow dtrowInput,
                            DataTable dtColumnsInput,
                            DataTable dtKeysInput,
                            DataTable dtIndexesInput,
                            DataTable dtRelationshipsInput,
                            DbDocSettings mySettings,
                            int tableCount,
                            int viewCount,
                            int procedureCount)
        {
            this.Dtrow = dtrowInput;
            this.DtColumns = dtColumnsInput;
            this.DtKeys = dtKeysInput;
            this.DtIndexes = dtIndexesInput;
            this.DtRelationships = dtRelationshipsInput;
            this.MySettings = mySettings;
            this.TableCount = tableCount;
            this.ViewCount = viewCount;
            this.ProcedureCount = procedureCount;

            this.WriteHTML();
        }
    }
}