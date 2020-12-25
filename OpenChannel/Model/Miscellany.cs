using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannel.Model
{
    public class ManningsNLine
    {
        public ManningsNLine(string[] words)
        {
            if (words.Length == 0)
            {
                Material = "";
                Min = "";
                Typical = "";
                Max = "";
                //Comment = "";
            }
            else if (words.Length == 1)
            {
                Material = words[0];
                Min = "";
                Typical = "";
                Max = "";
                //Comment = "";
            }
            else if (words.Length == 2)
            {
                Material = words[0];
                Min = words[1];
                Typical = "";
                Max = "";
                //Comment = "";
            }
            else if (words.Length == 3)
            {
                Material = words[0];
                Min = words[1];
                Typical = words[2];
                Max = "";
                //Comment = "";
            }
            else if (words.Length == 4)
            {
                Material = words[0];
                Min = words[1];
                Typical = words[2];
                Max = words[3];
                //Comment = "";
            }
            else if (words.Length == 5)
            {
                Material = words[0];
                Min = words[1];
                Typical = words[2];
                Max = words[3];
                //Comment = words[4];
            }
        }
        public string Material { get; set; }
        public string Min { get; set; }
        public string Typical { get; set; }
        public string Max { get; set; }
    }
}
