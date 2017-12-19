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

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class AnnotatableElement
    {
        private List<String> comments = new List<String>();
        private List<String> labels = new List<String>();

        public AnnotatableElement() { }

        public void AddAnnotationComment(String comment)
        {
            if (comment != null && comment.Length > 0 && !this.comments.Contains(comment))
            {
                this.comments.Add(comment);
            }
        }

        public void AddAnnotationLabel(String label)
        {
            if(label != null && label.Length > 0 && !this.labels.Contains(label))
            {
                this.labels.Add(label);
            }
        }

        public List<String> GetAnnotationComments() { return this.comments; }

        public List<String> GetAnnotationLabels() { return this.labels; }


    }
}
