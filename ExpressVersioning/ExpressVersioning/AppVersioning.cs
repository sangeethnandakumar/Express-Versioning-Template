using System;
using System.Collections.Generic;
using System.Reflection;
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
            major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            switch(_schema)
            {
                case VersioningSchema.MINORVERSIONING:
                    switch(_entity)
                    {
                        case VersioningEntity.APPLICATION:
                            /*Minor versioning scheme for Application will not touch major version. It need to be set by programmer when there is a ground breaking change*/
                            break;
                        case VersioningEntity.WEBAPI:
                            /*Minor versioning scheme for WebAPI will not touch major version. It need to be set by programmer when there is a ground breaking change*/
                            break;
                    }
                    break;
            }

            var verion = $"{major}.{minor}.{build}.{revision}";
            return verion;
        }
    }
}
