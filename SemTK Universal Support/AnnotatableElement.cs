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
