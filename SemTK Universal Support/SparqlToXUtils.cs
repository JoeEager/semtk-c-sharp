using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.SparqlX
{
    class SparqlToXUtils
    {
         public static String SafeSparqlString(String s)
        {
            String outPut = "";

            for(int i = 0; i < s.Length; i++)
            {
                // get the char
                char c = s[i];

                if (c == '\"') { outPut += "\\\""; }
                else if (c == '\'') { outPut += "\\'"; }
                else if (c == '\n') { outPut += "\\n"; }
                else if (c == '\r') { outPut += "\\r"; }
                else if (c == '\\')
                {
                    if(i+1 < s.Length)      // blackslash requires readahead
                    {
                        char c2 = s[i + 1];
                        if(c2 == 'n' || c2 == 't') { outPut += c; } // preserve the current ordering.
                        else { outPut += "\\\\"; }

                    }
                    else
                    {
                        outPut += "\\\\";
                    }
                }
                else
                {
                    outPut += c;
                }

            }
            return outPut;

        }

    }
}
