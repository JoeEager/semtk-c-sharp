/**
 ** Copyright 2017 General Electric Company
 **
 **
 ** Licensed under the Apache License, Version 2.0 (the "License");
 ** you may not use this file except in compliance with the License.
 ** You may obtain a copy of the License at
 ** 
 **     http://www.apache.org/licenses/LICENSE-2.0
 ** 
 ** Unless required by applicable law or agreed to in writing, software
 ** distributed under the License is distributed on an "AS IS" BASIS,
 ** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 ** See the License for the specific language governing permissions and
 ** limitations under the License.
 */

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
