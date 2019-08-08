using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Documentor.Settings
{
    public class HelpFileContentCollection
    {
        public List<HelpFileContentStructure> HelpFileContentStructureList { get; set; }

        public HelpFileContentCollection()
        {
            HelpFileContentStructureList = new List<HelpFileContentStructure>();
        }

        public void AddElementToStructureList(HelpFileContentStructure contentFile)
        {
            HelpFileContentStructureList.Add(contentFile);
        }
    }

    public class HelpFileContentStructure
    {
        public string HeadingName { get; set; }
        public string HtmlFilename { get; set; }
        public int ImageNumber { get; set; }
    }
}