using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressVersioning
{
    public class VersioningScheme
    {
        public string Major { get; set; }
        public string Minor { get; set; }
        public string Patch { get; set; }
        public string Revision { get; set; }
    }
}