using System;

namespace Database.Documentor.Utility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SchemaAttribute : Attribute
    {
        private string _fullName;
        private string _shortName;

        public string Fullname
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }

        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
            }
        }

        public SchemaAttribute(string _fullName, string _shortName)
        {
            this.Fullname = _fullName;
            this.ShortName = _shortName;
        } // New

        public virtual bool MatchesShortName(string _shortName)
        {
            return ShortName.ToLower().Equals(_shortName.ToLower());
        } // MatchesShortName
    } // SchemaAttribute
}