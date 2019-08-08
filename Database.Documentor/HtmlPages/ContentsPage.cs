using System.Data;
using System.IO;
using Database.Documentor.Properties;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    /// <summary>Produces the Contents HTML Page.</summary>
    public class ContentsPage : BasePage
    {
        private DataSet _ds;

        /// <summary>The dataset used to generate the HTML output.</summary>
        /// <value>DataSet containing data needed to produce the output Home Page.</value>
        public DataSet ds
        {
            get
            {
                return _ds;
            }
            set
            {
                _ds = value;
            }
        }

        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            string contentsFile = (MySettings.DatabaseName + ".Contents.htm").Replace(" ", "");
            string defaultFile = (MySettings.DatabaseName + ".Default.htm").Replace(" ", "");

            StreamWriter s = new StreamWriter(fs);

            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            s.WriteLine("    <META http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            s.WriteLine("    <title>Contents</title>");
            s.WriteLine("    <meta name='GENERATOR' content='Microsoft Visual Studio.NET 7.0'>");
            s.WriteLine("    <meta name='vs_targetSchema' content='http://schemas.microsoft.com/intellisense/ie5'>");
            s.WriteLine("    <link rel='stylesheet' type='text/css' href='tree.css'>");
            s.WriteLine("    <script src='tree.js' language='javascript' type='text/javascript'>");
            s.WriteLine("    </script>");
            s.WriteLine("  </head>");
            s.WriteLine("  <body id='docBody' style='background-color: #6699CC; color: White; margin: 0px 0px 0px 0px;' onload='resizeTree()' onresize='resizeTree()' onselectstart='return false;'>");
            s.WriteLine("    <div style='font-family: verdana; font-size: 8pt; cursor: pointer; margin: 6 4 8 2; text-align: right' onmouseover='this.style.textDecoration='underline;'' onmouseout='this.style.textDecoration='none;'' onclick='syncTree(window.parent.frames[1].document.URL)'>sync toc</div>");
            s.WriteLine("    <div id='tree' style='top: 35px; left: 0px;' class='treeDiv'>");
            s.WriteLine("      <div id='treeRoot' onselectstart='return false' ondragstart='return false'>");
            s.WriteLine("        <div class='treeNode'>");
            s.WriteLine("          <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
            s.WriteLine("          <a href='" + defaultFile + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + "</a>");

            s.WriteLine("          <div class='treeSubnodesHidden'>");

            if (!(ds.Tables[Resources.TablesText] == null) && ds.Tables[Resources.TablesText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetTablesFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Tables</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in ds.Tables[Resources.TablesText].Rows)
                {
                    string tableName = System.Convert.ToString(row["TABLE_NAME"]);
                    s.WriteLine("                <div class='treeNode'>");
                    s.WriteLine("                  <img src='treenodedot.gif' class='treeNoLinkImage'>");
                    s.WriteLine("                  <a href='" + GetFileName(tableName, "Table") + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + tableName + " Table</a>");
                    s.WriteLine("                </div>");
                }
                s.WriteLine("              </div>");
                s.WriteLine("            </div>");
            }

            if (!(ds.Tables[Resources.ViewsText] == null) && ds.Tables[Resources.ViewsText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetViewsFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Views</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in ds.Tables[Resources.ViewsText].Rows)
                {
                    string tableName = System.Convert.ToString(row["TABLE_NAME"]);
                    s.WriteLine("                <div class='treeNode'>");
                    s.WriteLine("                  <img src='treenodedot.gif' class='treeNoLinkImage'>");
                    s.WriteLine("                  <a href='" + GetFileName(tableName, "View") + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + tableName + " View</a>");
                    s.WriteLine("                </div>");
                }
                s.WriteLine("              </div>");
                s.WriteLine("            </div>");
            }

            if (!(ds.Tables[Resources.ProceduresText] == null) && ds.Tables[Resources.ProceduresText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetViewsFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Stored Procedures</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in ds.Tables[Resources.ProceduresText].Rows)
                {
                    string tableName = System.Convert.ToString(row["PROCEDURE_NAME"]);
                    s.WriteLine("                <div class='treeNode'>");
                    s.WriteLine("                  <img src='treenodedot.gif' class='treeNoLinkImage'>");
                    s.WriteLine("                  <a href='" + GetFileName(tableName, "Procedure") + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + tableName + " Stored Procedure</a>");
                    s.WriteLine("                </div>");
                }
                s.WriteLine("              </div>");
                s.WriteLine("            </div>");
            }

            s.WriteLine("          </div>");
            s.WriteLine("        </div>");
            s.WriteLine("      </div>");
            s.WriteLine("    </div>");
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