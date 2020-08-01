using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressVersioning
{
    public class AppVersioning
    {
        private int major = 0;
        private int minor = 0;
        private int build = 0;
        private int revision = 0;

        private readonly VersioningSchema _schema;
        private readonly VersioningEntity _entity;
        
        public AppVersioning(VersioningEntity entity, VersioningSchema schema)
        {
            _schema = schema;
            _entity = entity;
        }

        public string VersionRunningAssembly()
        {
            var verion = $"{major}.{minor}.{build}.{revision}";
            return verion;
        }
    }
}
