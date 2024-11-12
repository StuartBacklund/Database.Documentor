using System;

namespace Database.Documentor.Utility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SchemaAttribute : Attribute
    {
        private string fullName;
        private string shortName;

        public string Fullname
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
            }
        }

        public string ShortName
        {
            get
            {
                return shortName;
            }
            set
            {
                shortName = value;
            }
        }

        public SchemaAttribute(string fullName, string shortName)
        {
            this.Fullname = fullName;
            this.ShortName = shortName;
        } // New

        public virtual bool MatchesShortName(string shortName)
        {
            return ShortName.ToLower().Equals(shortName.ToLower());
        } // MatchesShortName
    } // SchemaAttribute
}