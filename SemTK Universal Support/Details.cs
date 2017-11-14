using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.Logging
{
    public class Details
    {
        private List<DetailsTuple> detailsTuples;

        public List<DetailsTuple> AsList() { return this.detailsTuples; }

        public Details() { this.detailsTuples = new List<DetailsTuple>(); }

        public Details(List<DetailsTuple> tuples) { this.detailsTuples = tuples; }

        public Details AddDetails(DetailsTuple tuple)
        {
            detailsTuples.Add(tuple);
            return this;
        }

        public Details AddDetails(String key, String value)
        {
            return this.AddDetails(new DetailsTuple(key, value));
        }
    }
}
