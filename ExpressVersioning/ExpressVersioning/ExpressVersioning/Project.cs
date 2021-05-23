using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressVersioning
{
    public class Project
    {
        public string Identifier { get; set; }
        public bool EnableVersioning { get; set; }
        public string File { get; set; }
        public VersioningScheme Scheme { get; set; }
    }
}