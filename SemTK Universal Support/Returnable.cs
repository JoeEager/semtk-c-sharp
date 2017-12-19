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

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public abstract class Returnable
    {
        protected String sparqlID = null;                           // instance-specific name used in query generation and manipulation
        protected Boolean isReturned = false;                       // is this going to show up in the result set
        protected Boolean isRuntimeConstrained = false;             // can this be overrided at runtime?

        // the constraints to be applied
        protected ValueConstraint constraints = null;

        public String GetSparqlID()
        {
            return this.sparqlID != null ? this.sparqlID : "";
        }

        public abstract void SetSparqlID(String sparqlId);    

        public Boolean GetIsReturned()
        {
            return this.isReturned;
        }

        public String GetRuntimeConstraintID()
        {
            return this.GetSparqlID();
        }

        public Boolean GetIsRuntimeConstrained()
        {
            return this.isRuntimeConstrained;
        }

        public void SetIsRuntimeConstrained(Boolean constrained)
        {
            this.isRuntimeConstrained = constrained;
        }

        public void SetValueConstraint(ValueConstraint v)
        {
            this.constraints = v;
        }
        public ValueConstraint GetValueConstraint()
        {
            return this.constraints;
        }

        public abstract String GetValueType();

    }
}
