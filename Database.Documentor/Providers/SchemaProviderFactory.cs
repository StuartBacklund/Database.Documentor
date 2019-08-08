using System;
using System.Data;
using Database.Documentor.Utility;

namespace Database.Documentor.Providers
{
    public class SchemaProviderFactory
    {
        private DataTable schemaProviders;

        public DataTable SchemaProviders
        {
            get
            {
                return schemaProviders;
            }
            set
            {
                schemaProviders = value;
            }
        }

        public SchemaProviderFactory()
        {
            schemaProviders = this.GetCreateSchemaProviderList(this);
        } // New

        public SchemaProvider CreateSchemaProvider(string shortName)
        {
            foreach (DataRow row in schemaProviders.Rows)
            {
                if (System.Convert.ToString(row["ShortName"]) == shortName)
                {
                    Type impl = System.Type.GetType(System.Convert.ToString(row["Type"]));
                    return (SchemaProvider)System.Activator.CreateInstance(impl);
                }
            }

            throw new SchemaProviderFactoryException("Could not find a SchemaProvider implementation for this ShortName");
        }

        public DataTable GetCreateSchemaProviderList(object @this)
        {
            System.Reflection.Assembly ea = this.GetType().Assembly;//.GetSatelliteAssembly();//.GetEntryAssembly();
            System.Type[] types = ea.GetExportedTypes();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("FullName"));
            dt.Columns.Add(new DataColumn("ShortName"));
            dt.Columns.Add(new DataColumn("Type"));

            foreach (System.Type typ in types)
            {
                SchemaAttribute[] attrlist = (SchemaAttribute[])typ.GetCustomAttributes(typeof(SchemaAttribute), true);

                foreach (SchemaAttribute attr in attrlist)
                {
                    DataRow row;
                    row = dt.NewRow();
                    row["ShortName"] = attr.ShortName;
                    row["FullName"] = attr.Fullname;
                    row["Type"] = typ.FullName;
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }
    }
}