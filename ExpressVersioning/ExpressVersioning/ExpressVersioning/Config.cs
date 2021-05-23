using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressVersioning
{
    public class Config
    {
        public List<Project> Projects { get; set; }

        public Config()
        {
            Projects = new List<Project>();
        }
    }
}