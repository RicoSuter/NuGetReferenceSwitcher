using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyToolkit.Collections;
using VSLangProj;

namespace RicoSuter.NuGetReferenceSwitcher.Domain
{
    public class MyProject
    {
        public MyProject(VSProject project)
        {
            Project = project;
            Name = project.Project.Name;

            References = new ExtendedObservableCollection<MyReference>();
            NuGetReferences = new ExtendedObservableCollection<MyReference>();

            try
            {
                foreach (var reference in Project.References.OfType<Reference>())
                {
                    var x = new MyReference(reference);
                    References.Add(x);
                    if (reference.Path.Contains("/packages/") || reference.Path.Contains("\\packages\\"))
                        NuGetReferences.Add(x);
                }
            }
            catch (Exception)
            {

            }
        }

        public string Name { get; set; }

        public VSProject Project { get; set; }

        public ExtendedObservableCollection<MyReference> References { get; private set; }
        public ExtendedObservableCollection<MyReference> NuGetReferences { get; private set; }

        public string ConfigurationPath
        {
            get
            {
                var projectPath = Project.Project.FileName;
                var projectDirectory = Path.GetDirectoryName(projectPath);
                return Path.Combine(projectDirectory, "NuGetReferenceSwitcher.cfg");
            }
        }

        public IEnumerable<ProjectToAssemblySwitch> ProjectToAssemblySwitches
        {
            get
            {
                if (File.Exists(ConfigurationPath))
                {
                    var lines = File.ReadAllLines(ConfigurationPath)
                        .Select(l => l.Split('\t'))
                        .Where(l => l.Length == 2).ToArray();

                    foreach (var line in lines)
                        yield return new ProjectToAssemblySwitch { FromProjectName = line[0], ToAssemblyPath = line[1] };
                }
            }
        }

        public void DeleteConfigurationFile()
        {
            if (File.Exists(ConfigurationPath))
                File.Delete(ConfigurationPath);
        }
    }
}