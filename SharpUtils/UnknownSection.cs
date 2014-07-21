using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpUtils
{
    class UnknownSection : Exception
    {
        //private string section;

        public UnknownSection(string section) : base(section)
        {                       
        }
    }
}
