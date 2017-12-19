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
