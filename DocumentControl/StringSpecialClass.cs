using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentControl
{
    public class StringSpecialClass
    {
        public string StringSpecial(string Text)
        {
            string[] ReplaceArray = { @"\", @"/", @":", @"?", @"'", @"""", @"<", @">", @"|", @"," };
            foreach (string special in ReplaceArray)
            {
                Text = Text.Replace(special, " ");
            }
            return Text;
        }
    }
}