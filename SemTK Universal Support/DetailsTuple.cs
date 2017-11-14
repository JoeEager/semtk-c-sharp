using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.Logging
{
    public class DetailsTuple
    {
        private String detailName;
        private String detailValue;

        public DetailsTuple(String name, String value)
        {
            this.detailName = name;
            if(value == null) { this.detailValue = ""; }
            else{
                this.detailValue = value.Replace("\"", "\\\"");
            }
        }

        public String GetValue() { return this.detailValue; }
        public String GetName() { return this.detailName; }

        // create a collection of DetailTuples from a name and collection of detail values.
        public static List<DetailsTuple> CreateGroupFromInputArray(String name, List<String> values, List<DetailsTuple> retval)
        {   // use the values in the list
            if(retval == null) { retval = new List<DetailsTuple>(); }
            // populate
            foreach(String i in values)
            {
                retval.Add(new DetailsTuple(name, i));
            }

            return retval;
        }

        
        public override Boolean Equals(Object o)
        {
            if(this == o) { return true; }
            if(o == null || this.GetType() != o.GetType()) { return false; }
            Boolean retval = false;

            DetailsTuple that = (DetailsTuple)o;

            retval = (this.detailName.Equals(that.detailName)) && (this.detailValue.Equals(that.detailValue));

            return retval;
        }

        public override int GetHashCode()
        {
            return (this.detailName + this.detailValue).GetHashCode();
        }
    }
}
