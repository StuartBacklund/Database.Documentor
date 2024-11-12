using System;
using System.ComponentModel;
using System.IO;
using Database.Documentor.Settings;

namespace Database.Documentor.Providers
{
    public abstract class ConnectionStringBuilder
    {
        public string type;
        private string sectionName = "Database";

        public abstract string ConnectionString();

        [Browsable(false)]
        public string Type
        {
            get
            {
                return type;
            }
        }

        private string DataDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Data");
        }

        public virtual bool SaveProject(string filename)
        {
            if (File.Exists(filename))
            {
                XmlSettings x = new XmlSettings(filename);

                x.DeleteKey(sectionName, "");
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor p in properties)
                {
                    x.SaveStringSetting(sectionName, p.Name, System.Convert.ToString(p.GetValue(this)));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool LoadProject(string filename)
        {
            if (File.Exists(filename))
            {
                XmlSettings x = new XmlSettings(filename);

                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor p in properties)
                {
                    if (p.IsReadOnly == false)
                    {
                        string temp = x.GetStringSetting(sectionName, p.Name, "");
                        System.Type _type = p.PropertyType;
                        try
                        {
                            switch (_type.ToString())
                            {
                                case object _ when _type.ReflectedType.Name == "System.Boolean":
                                    {
                                        p.SetValue(this, System.Convert.ToBoolean(temp));
                                        break;
                                    }

                                default:
                                    {
                                        p.SetValue(this, temp);
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}