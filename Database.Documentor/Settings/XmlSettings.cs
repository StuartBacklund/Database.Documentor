using System;
using System.Diagnostics;
using System.Xml;

namespace Database.Documentor.Settings
{
    public class XmlSettings
    {
        private string m_XmlFileName;
        private XmlDocument m_xmlDocument;
        private const string NOTFOUND = "<<nothing>>";

        /// <summary>Gets or sets the value of the XMLSettings file name</summary>
        public string XmlFileName
        {
            get
            {
                return m_XmlFileName;
            }
            set
            {
                m_XmlFileName = value.Trim();
            }
        }

        /// <summary>On instantiation, save XML filename and open the XmlDocument</summary>
        /// <param name="appName">Name of the XML Settings file to open</param>
        public XmlSettings(string filepath)
        {
            XmlFileName = filepath;
            _OpenXMLFile();
        }

        /// <summary>Deletes the specified key, deleting with empty KeyName deletes the specified section</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        public void DeleteKey(string sectionName, string keyName)
        {
            string sKeyValue;
            XmlNode xnSection;
            XmlNode xnKey;
            xnSection = m_xmlDocument.SelectSingleNode("//Section[@Name='" + sectionName + "']");
            if (xnSection == null)
                return;
            else if (keyName.Length == 0)
            {
                // delete the section
                XmlNode xnRoot = m_xmlDocument.DocumentElement;
                xnRoot.RemoveChild(xnSection);
            }
            else
            {
                // delete the key
                xnKey = xnSection.SelectSingleNode("descendant::Key[@Name='" + keyName + "']");
                if (xnKey == null)
                    return;
                else
                    xnSection.RemoveChild(xnKey);
            }
            m_xmlDocument.Save(XmlFileName);
            xnKey = null;
            xnSection = null;
        }

        /// <summary>Retrieves section names from XML Settings file</summary>
        /// <returns>Array of Strings containing the all the section names in the XML File</returns>
        public string[] GetSectionNames()
        {
            string sSectionName = string.Empty;
            string[] sSections = null;
            int iCnt = 0;
            XmlNodeList xnSections = m_xmlDocument.SelectNodes("//Section");

            if (!(xnSections == null))
            {
                sSections = new string[xnSections.Count - 1 + 1];
                foreach (XmlNode xnSection in xnSections)
                {
                    sSections[iCnt] = xnSection.Attributes["Name"].Value;
                    iCnt += 1;
                }
            }

            xnSections = null;
            return sSections;
        }

        /// <summary>Retrieves an array of Keys and Values for the SectionName specified</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <returns>Two Dimensional aray containing all Keys and Values for specified Section</returns>
        public string[,] GetAllSetting(string sectionName)
        {
            string[,] sKeys = null;
            int iCnt = 0;
            XmlNode xnSection;

            XmlNodeList xnKeys;
            xnSection = m_xmlDocument.SelectSingleNode("//Section[@Name='" + sectionName + "']");
            if (!(xnSection == null))
            {
                xnKeys = xnSection.SelectNodes("descendant::Key");
                if (!(xnKeys == null))
                {
                    sKeys = new string[xnKeys.Count - 1 + 1, 2];
                    foreach (XmlNode xnKey in xnKeys)
                    {
                        sKeys[iCnt, 0] = xnKey.Attributes["Name"].Value;
                        sKeys[iCnt, 1] = xnKey.Attributes["Value"].Value;
                        iCnt += 1;
                    }
                }
            }
            xnKeys = null;
            xnSection = null;
            return sKeys;
        }

        /// <summary>Finds the Matching Section/Key pair and returns an Integer value</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        /// <param name="defaultValue">The value that will be returned if the Section/Key pair is not found</param>
        /// <returns>Returns corresponding integer value if found, if not function returns specified DefaultValue</returns>
        public int GetIntegerSetting(string sectionName, string keyName, int defaultValue)
        {
            int iKeyValue;
            string sKeyValue;
            sKeyValue = _GetSetting(sectionName, keyName);
            if (sKeyValue == NOTFOUND)
                iKeyValue = defaultValue;
            else
                try
                {
                    iKeyValue = System.Convert.ToInt32(sKeyValue);
                }
                catch
                {
                    // return zero if non-integer value found
                    iKeyValue = 0;
                }
            return iKeyValue;
        }

        /// <summary>Finds the Matching Section/Key pair and returns a String value</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        /// <param name="defaultValue">The value that will be returned if the Section/Key pair is not found</param>
        /// <returns>Returns corresponding string value if found, if not function returns specified DefaultValue</returns>
        public string GetStringSetting(string sectionName, string keyName, string defaultValue)
        {
            string sKeyValue;
            sKeyValue = _GetSetting(sectionName, keyName);
            if (sKeyValue == NOTFOUND)
                sKeyValue = defaultValue;
            return sKeyValue;
        }

        /// <summary>Creates or Updates a String Setting</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        /// <param name="setting">The string value to set</param>
        public void SaveStringSetting(string sectionName, string keyName, string setting)
        {
            _SaveSetting(sectionName, keyName, setting);
        }

        /// <summary>Creates or updates an Integer setting</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        /// <param name="setting">The string value to set</param>
        public void SaveIntegerSetting(string sectionName, string keyName, int setting)
        {
            string sSetting;
            sSetting = System.Convert.ToString(setting);
            _SaveSetting(sectionName, keyName, sSetting);
        }

        /// <summary>Private function used by <see cref="FunctionLibrary.XmlSettings.GetIntegerSetting"/> and <see cref="FunctionLibrary.XmlSettings.GetStringSetting"/> to handle Setting lookup</summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        /// <returns>Returns value as String if found, Returns "NOTFOUND" if Section/Key pair does not exist</returns>
        private string _GetSetting(string sectionName, string keyName)
        {
            string sKeyValue;
            XmlNode xnSection;
            XmlNode xnKey;
            xnSection = m_xmlDocument.SelectSingleNode("//Section[@Name='" + sectionName + "']");
            if (xnSection == null)
                sKeyValue = NOTFOUND;
            else
            {
                xnKey = xnSection.SelectSingleNode("descendant::Key[@Name='" + keyName + "']");
                if (xnKey == null)
                    sKeyValue = NOTFOUND;
                else
                    sKeyValue = xnKey.Attributes["Value"].Value;
            }
            xnKey = null;
            xnSection = null;
            return sKeyValue;
        }

        /// <summary>Opens XML settings file</summary>
        private void _OpenXMLFile()
        {
            try
            {
                XmlTextReader xmlTR = new XmlTextReader(XmlFileName);
                m_xmlDocument = new XmlDocument();
                m_xmlDocument.Load(xmlTR);
                xmlTR.Close();
                xmlTR = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Settings Constructor Error: " + e.ToString());
            }
        }

        /// <summary>Private function used by <see cref="FunctionLibrary.XmlSettings.SaveIntegerSetting"/> and <see cref="FunctionLibrary.XmlSettings.SaveStringSetting"/> to handle creation
        /// or updating of Section/Key pairs </summary>
        /// <param name="sectionName">The name of the XML section</param>
        /// <param name="keyName">The name of the Key</param>
        /// <param name="setting">The string value to set</param>
        private void _SaveSetting(string sectionName, string keyName, string setting)
        {
            XmlNode xnSection;
            XmlNode xnKey;
            XmlAttribute xnAttr;
            // check the document exists, create if not
            if (m_xmlDocument.DocumentElement == null)
            {
                try
                {
                    m_xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<ApplicationSettings>" + "</ApplicationSettings>");// CrLf
                }
                catch (Exception e1)
                {
                    Debug.WriteLine("_SaveSetting - Error creating Document: " + e1.ToString());
                }
            }
            xnSection = m_xmlDocument.SelectSingleNode("//Section[@Name='" + sectionName + "']");
            // check the Section exists, create if not
            if (xnSection == null)
            {
                try
                {
                    // create the new Section node...
                    xnSection = m_xmlDocument.CreateNode(XmlNodeType.Element, "Section", "");
                    // add the Name attribute
                    xnAttr = m_xmlDocument.CreateAttribute("Name");
                    xnAttr.Value = sectionName;
                    xnSection.Attributes.SetNamedItem(xnAttr);
                    // get the root XML node and add the new node to the document
                    XmlNode xnRoot = m_xmlDocument.DocumentElement;
                    xnRoot.AppendChild(xnSection);
                    xnRoot = null;
                }
                catch (Exception e2)
                {
                    Debug.WriteLine("_SaveSetting - Error creating Section: " + e2.ToString());
                }
            }
            xnKey = xnSection.SelectSingleNode("descendant::Key[@Name='" + keyName + "']");
            // check the Key exists, create if not
            if (xnKey == null)
            {
                try
                {
                    // create the new Key node...
                    xnKey = m_xmlDocument.CreateNode(XmlNodeType.Element, "Key", "");
                    // add the Name attribute
                    xnAttr = m_xmlDocument.CreateAttribute("Name");
                    xnAttr.Value = keyName;
                    xnKey.Attributes.SetNamedItem(xnAttr);
                    // add the Value attribute
                    xnAttr = m_xmlDocument.CreateAttribute("Value");
                    xnAttr.Value = setting;
                    xnKey.Attributes.SetNamedItem(xnAttr);
                    // add the new node to its Section
                    xnSection.AppendChild(xnKey);
                }
                catch (Exception e3)
                {
                    Debug.WriteLine("_SaveSetting - Error creating Key: " + e3.ToString());
                }
            }
            else
                xnKey.Attributes["Value"].Value = setting;
            // save changes
            try
            {
                m_xmlDocument.Save(XmlFileName);
            }
            catch (Exception e4)
            {
                Debug.WriteLine("_SaveSetting - Error Saving File: " + e4.ToString());
            }
            xnKey = null;
            xnSection = null;
        }
    }
}