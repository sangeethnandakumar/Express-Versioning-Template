using ExpressVersioning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    public static class Program
    {
        public static void Main()
        {
            var app = new AppVersioning(VersioningEntity.APPLICATION, VersioningSchema.DEFAULT);
            var version = app.VersionRunningAssembly();
            Console.WriteLine($"Application version is - " + version);
            Console.Read();
        }
    }
}
