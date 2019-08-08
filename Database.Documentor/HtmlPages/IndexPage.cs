using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.htmPages
{
    /// <summary>Produces the Index HTML Page.</summary>
    public class IndexPage : BasePage
    {
        /// <summary>Produces the output HTML.</summary>
        public override void WriteHTML()
        {
            string contentsFile = (MySettings.DatabaseName + ".Contents.htm").Replace(" ", "");
            string defaultFile = (MySettings.DatabaseName + ".Default.htm").Replace(" ", "");

            StreamWriter s = new StreamWriter(fs);

            s.WriteLine("<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Frameset//EN'>");
            s.WriteLine("<html>");
            s.WriteLine("  <head>");
            s.WriteLine("     <meta name='Robots' content='noindex'>");
            s.WriteLine("     <title>" + MySettings.DatabaseName + "</title>");
            s.WriteLine("     <script language='JavaScript'>");
            s.WriteLine("     // ensure this page is not loaded inside another frame");
            s.WriteLine("     if (top.location != self.location)");
            s.WriteLine("     {");
            s.WriteLine("     	top.location = self.location;");
            s.WriteLine("     }");
            s.WriteLine("     </script>");
            s.WriteLine("  </head>");

            s.WriteLine("	<frameset cols='250,*' framespacing='6' bordercolor='#6699CC'>");
            s.WriteLine("		<frame name='contents' src='" + contentsFile + "' frameborder='0' scrolling='no'>");
            s.WriteLine("		<frame name='main' src='" + defaultFile + "' frameborder='1'>");
            s.WriteLine("		<noframes>");
            s.WriteLine("			<p>This page requires frames, but your browser does not support them.</p>");
            s.WriteLine("		</noframes>");
            s.WriteLine("	</frameset>");
            s.WriteLine("</html>");

            s.Close();
        }

        /// <summary>Sets class properties then produces the output HTML.</summary>
        public void WriteHTML(DbDocSettings mySettings)
        {
            this.MySettings = mySettings;
            this.WriteHTML();
        }
    }
}