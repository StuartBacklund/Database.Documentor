using System.Data;
using System.IO;
using Database.Documentor.Properties;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    public class ContentsPage : BasePage
    {
        private DataSet ds;

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

            if (!(Ds.Tables[Resources.TablesText] == null) && Ds.Tables[Resources.TablesText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetTablesFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Tables</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in Ds.Tables[Resources.TablesText].Rows)
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

            if (!(Ds.Tables[Resources.ViewsText] == null) && Ds.Tables[Resources.ViewsText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetViewsFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Views</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in Ds.Tables[Resources.ViewsText].Rows)
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

            if (!(Ds.Tables[Resources.ProceduresText] == null) && Ds.Tables[Resources.ProceduresText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetProceduresFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Stored Procedures</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in Ds.Tables[Resources.ProceduresText].Rows)
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

            if (!(Ds.Tables[Resources.FunctionsText] == null) && Ds.Tables[Resources.FunctionsText].Rows.Count > 0)
            {
                s.WriteLine("            <div class='treeNode'>");
                s.WriteLine("              <img src='treenodeplus.gif' class='treeLinkImage' onclick='expandCollapse(this.parentNode)'>");
                s.WriteLine("              <a href='" + GetFunctionsFileName() + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + MySettings.DatabaseName + " Functions</a>");
                s.WriteLine("                <div class='treeSubnodesHidden'>");

                foreach (DataRow row in Ds.Tables[Resources.FunctionsText].Rows)
                {
                    string tableName = System.Convert.ToString(row["function_name"]);
                    s.WriteLine("                <div class='treeNode'>");
                    s.WriteLine("                  <img src='treenodedot.gif' class='treeNoLinkImage'>");
                    s.WriteLine("                  <a href='" + GetFileName(tableName, "Functions") + "' target='main' class='treeUnselected' onclick='clickAnchor(this)'>" + tableName + " Function</a>");
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

        public void WriteHTML(DataSet dsinput, DbDocSettings mySettings)
        {
            this.Ds = dsinput;
            this.MySettings = mySettings;
            this.WriteHTML();
        }
    }
}