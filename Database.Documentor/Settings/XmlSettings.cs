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

        public XmlSettings(string filepath)
        {
            XmlFileName = filepath;
            OpenXmlFile();
        }

        public void DeleteKey(string sectionName, string keyName)
        {
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

        public int GetIntegerSetting(string sectionName, string keyName, int defaultValue)
        {
            int iKeyValue;
            string sKeyValue;
            sKeyValue = GetSetting(sectionName, keyName);
            if (sKeyValue == NOTFOUND)
                iKeyValue = defaultValue;
            else
                try
                {
                    iKeyValue = System.Convert.ToInt32(sKeyValue);
                }
                catch
                {
                    iKeyValue = 0;
                }
            return iKeyValue;
        }

        public string GetStringSetting(string sectionName, string keyName, string defaultValue)
        {
            string sKeyValue;
            sKeyValue = GetSetting(sectionName, keyName);
            if (sKeyValue == NOTFOUND)
                sKeyValue = defaultValue;
            return sKeyValue;
        }

        public void SaveStringSetting(string sectionName, string keyName, string setting)
        {
            SaveSetting(sectionName, keyName, setting);
        }

        public void SaveIntegerSetting(string sectionName, string keyName, int setting)
        {
            string sSetting;
            sSetting = System.Convert.ToString(setting);
            SaveSetting(sectionName, keyName, sSetting);
        }

        private string GetSetting(string sectionName, string keyName)
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

        private void OpenXmlFile()
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

        private void SaveSetting(string sectionName, string keyName, string setting)
        {
            XmlNode xnSection;
            XmlNode xnKey;
            XmlAttribute xnAttr;

            if (m_xmlDocument.DocumentElement == null)
            {
                try
                {
                    m_xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<ApplicationSettings>" + "</ApplicationSettings>");// CrLf
                }
                catch (Exception e1)
                {
                    Debug.WriteLine("SaveSetting - Error creating Document: " + e1.ToString());
                }
            }
            xnSection = m_xmlDocument.SelectSingleNode("//Section[@Name='" + sectionName + "']");

            if (xnSection == null)
            {
                try
                {
                    xnSection = m_xmlDocument.CreateNode(XmlNodeType.Element, "Section", "");
                    xnAttr = m_xmlDocument.CreateAttribute("Name");
                    xnAttr.Value = sectionName;
                    xnSection.Attributes.SetNamedItem(xnAttr);
                    XmlNode xnRoot = m_xmlDocument.DocumentElement;
                    xnRoot.AppendChild(xnSection);
                    xnRoot = null;
                }
                catch (Exception e2)
                {
                    Debug.WriteLine("SaveSetting - Error creating Section: " + e2.ToString());
                }
            }
            xnKey = xnSection.SelectSingleNode("descendant::Key[@Name='" + keyName + "']");

            if (xnKey == null)
            {
                try
                {
                    xnKey = m_xmlDocument.CreateNode(XmlNodeType.Element, "Key", "");
                    xnAttr = m_xmlDocument.CreateAttribute("Name");
                    xnAttr.Value = keyName;
                    xnKey.Attributes.SetNamedItem(xnAttr);
                    xnAttr = m_xmlDocument.CreateAttribute("Value");
                    xnAttr.Value = setting;
                    xnKey.Attributes.SetNamedItem(xnAttr);
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
                Debug.WriteLine("SaveSetting - Error Saving File: " + e4.ToString());
            }
            xnKey = null;
            xnSection = null;
        }
    }
}