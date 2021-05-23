using ExpressSettingsCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace ExpressVersioning
{
    internal class Program
    {
        private static SettingsConfiguration settingsConfiguration;

        private static string ApplyVersionScheme(string scheme, string version)
        {
            //Time
            if (scheme.Contains("{hh}"))
            {
                scheme = scheme.Replace("{hh}", DateTime.Now.ToString("hh"));
            }
            if (scheme.Contains("{HH}"))
            {
                scheme = scheme.Replace("{HH}", DateTime.Now.ToString("HH"));
            }
            if (scheme.Contains("{mm}"))
            {
                scheme = scheme.Replace("{mm}", DateTime.Now.ToString("mm"));
            }
            if (scheme.Contains("{ss}"))
            {
                scheme = scheme.Replace("{ss}", DateTime.Now.ToString("ss"));
            }
            if (scheme.Contains("{tt}"))
            {
                scheme = scheme.Replace("{tt}", DateTime.Now.ToString("tt"));
            }
            //Date
            if (scheme.Contains("{yy}"))
            {
                scheme = scheme.Replace("{yy}", DateTime.Now.ToString("yy"));
            }
            if (scheme.Contains("{MM}"))
            {
                scheme = scheme.Replace("{MM}", DateTime.Now.ToString("MM"));
            }
            //Extra
            if (scheme.Contains("{doy}"))
            {
                scheme = scheme.Replace("{doy}", DateTime.Now.DayOfYear.ToString());
            }
            if (scheme.Contains("{dom}"))
            {
                scheme = scheme.Replace("{dom}", DateTime.Now.ToString("dd"));
            }
            if (scheme.Contains("{weekno}"))
            {
                var currentCulture = CultureInfo.CurrentCulture;
                var weekNo = currentCulture.Calendar.GetWeekOfYear(
                                DateTime.Now,
                                currentCulture.DateTimeFormat.CalendarWeekRule,
                                currentCulture.DateTimeFormat.FirstDayOfWeek);
                scheme = scheme.Replace("{weekno}", weekNo.ToString());
            }
            if (scheme == "")
            {
                return version;
            }
            return scheme;
        }

        private static void VersionProject(Project project)
        {
            if (project.EnableVersioning)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(project.File);

                var fileVersion = "1.0.0.0";
                var isFileVersionExists = false;
                var assemblyVersion = "1.0.0.0";
                var isAssemblyVersionExists = false;

                foreach (XmlNode item in xml.SelectSingleNode("Project//PropertyGroup").ChildNodes)
                {
                    if (item.Name == "AssemblyVersion")
                    {
                        isAssemblyVersionExists = true;
                        assemblyVersion = item.InnerText;
                    }
                    if (item.Name == "FileVersion")
                    {
                        isFileVersionExists = true;
                        fileVersion = item.InnerText;
                    }
                }

                var major = assemblyVersion.Split(".")[0];
                var minor = assemblyVersion.Split(".")[1];
                var patch = assemblyVersion.Split(".")[2];
                var revision = assemblyVersion.Split(".")[3];

                var newMajor = ApplyVersionScheme(project.Scheme.Major, major);
                var newMinor = ApplyVersionScheme(project.Scheme.Minor, minor);
                var newPatch = ApplyVersionScheme(project.Scheme.Patch, patch);
                var newRevision = ApplyVersionScheme(project.Scheme.Revision, revision);

                var oldVersion = $"{major}.{minor}.{patch}.{revision}";
                var scheme = $"{project.Scheme.Major}.{project.Scheme.Minor}.{project.Scheme.Patch}.{project.Scheme.Revision}";
                var newVersion = $"{newMajor}.{newMinor}.{newPatch}.{newRevision}";

                if (isAssemblyVersionExists)
                {
                    XmlNode assemblyNode = xml.SelectSingleNode("Project//PropertyGroup//AssemblyVersion");
                    assemblyNode.InnerText = newVersion;
                }
                else
                {
                    XmlNode assemblyNode = xml.CreateNode(XmlNodeType.Element, "AssemblyVersion", null);
                    assemblyNode.InnerText = newVersion;
                    xml.SelectSingleNode("Project//PropertyGroup").AppendChild(assemblyNode);
                }

                if (isFileVersionExists)
                {
                    XmlNode fileNode = xml.SelectSingleNode("Project//PropertyGroup//FileVersion");
                    fileNode.InnerText = newVersion;
                }
                else
                {
                    XmlNode fileNode = xml.CreateNode(XmlNodeType.Element, "FileVersion", null);
                    fileNode.InnerText = newVersion;
                    xml.SelectSingleNode("Project//PropertyGroup").AppendChild(fileNode);
                }

                Console.WriteLine($"Project: {project.Identifier} | New Version: {newVersion} | Old Version: {oldVersion} | Scheme: '{scheme}'");

                xml.Save(project.File);
            }
            else
            {
                Console.WriteLine($"Project: {project.Identifier} | [Skipped] | Versioning not enabled");
            }
        }

        private static void VersionAllProjects(List<Project> projects)
        {
            foreach (var project in projects)
            {
                VersionProject(project);
            }
        }

        private static void Main(string[] args)
        {
            settingsConfiguration = new SettingsBuilder().Build();
            Console.WriteLine("EXPRESS VERSIONING v2");

            var config = Settings<Config>.Read(settingsConfiguration);

            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == "-a")
                    {
                        Console.WriteLine($"Project Details");
                        Console.WriteLine($"---------------");
                        VersionAllProjects(config.Projects);
                        Console.WriteLine($"Versioned all configured project (versioning enabled projects only)");
                    }
                    else if (args[0] == "-info")
                    {
                        Console.WriteLine($"Projects Details");
                        Console.WriteLine($"----------------");
                        foreach (var project in config.Projects)
                        {
                            Console.WriteLine($"Project: {project.Identifier} | File: {project.File}");
                        }
                    }
                    else if (args[0] == "-p")
                    {
                        var project = config.Projects.Where(p => p.Identifier == args[1]).FirstOrDefault();
                        if (project != null)
                        {
                            Console.WriteLine($"Project Details");
                            Console.WriteLine($"---------------");
                            VersionProject(project);
                            Console.WriteLine($"Versioned Project - {project.Identifier}");
                        }
                        else
                        {
                            Console.WriteLine($"Unable to detect project with identifier - '{args[1]}'. Run '-info' to list all configured projects");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unsupported command");
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("1. Basic Commands");
                    Console.WriteLine(@"-info : List down all configured projects");
                    Console.WriteLine(@"-a : Versions all the configured projects");
                    Console.WriteLine(@"-p [PROJECT_IDENTIFIER] : Versions the specified project based on configured identifier");

                    Console.WriteLine(@"");

                    Console.WriteLine("2. Compose Version Schemes With");
                    Console.WriteLine(@"{hh} - Hour in 12hrs
{HH} - Hour in 24hrs
{mm} - Minute
{ss} - Second
{tt} - AM / PM
---------------------------
{yy} - Year 2 digits
{MM} - Month 2 digits
---------------------------
{doy} - Day of year
{dom} - Day of month
{weekno} - Week number");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Process unable to complete or partialy completed");
            }
        }
    }
}