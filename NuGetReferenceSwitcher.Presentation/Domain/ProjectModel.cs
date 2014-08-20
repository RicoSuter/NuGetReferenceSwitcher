using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using MyToolkit.Collections;
using VSLangProj;

namespace NuGetReferenceSwitcher.Presentation.Domain
{
    public class ProjectModel
    {
        private readonly VSProject _vsProject;

        public ProjectModel(VSProject project)
        {
            _vsProject = project;
            
            Name = project.Project.Name;
            LoadReferences();
        }

        public string Name { get; set; }

        public ExtendedObservableCollection<ReferenceModel> References { get; private set; }

        public ExtendedObservableCollection<ReferenceModel> NuGetReferences { get; private set; }

        public string ConfigurationPath
        {
            get
            {
                var projectPath = _vsProject.Project.FileName;
                var projectDirectory = System.IO.Path.GetDirectoryName(projectPath);
                if (projectDirectory != null)
                    return System.IO.Path.Combine(projectDirectory, "NuGetReferenceSwitcher.cfg");
                throw new IOException();
            }
        }

        public IEnumerable<FromProjectToAssemblySwitch> ProjectToAssemblySwitches
        {
            get
            {
                if (File.Exists(ConfigurationPath))
                {
                    var lines = File.ReadAllLines(ConfigurationPath)
                        .Select(l => l.Split('\t'))
                        .Where(l => l.Length == 2).ToArray();

                    foreach (var line in lines)
                        yield return new FromProjectToAssemblySwitch { FromProjectName = line[0], ToAssemblyPath = line[1] };
                }
            }
        }

        public string Path
        {
            get { return _vsProject.Project.FileName; }
        }

        public void DeleteConfigurationFile()
        {
            if (File.Exists(ConfigurationPath))
                File.Delete(ConfigurationPath);
        }

        private void LoadReferences()
        {
            References = new ExtendedObservableCollection<ReferenceModel>();
            NuGetReferences = new ExtendedObservableCollection<ReferenceModel>();

            foreach (var reference in _vsProject.References.OfType<Reference>())
            {
                var x = new ReferenceModel(reference);
                References.Add(x);
                if (reference.Path.Contains("/packages/") || reference.Path.Contains("\\packages\\"))
                    NuGetReferences.Add(x);
            }
        }

        public void AddProject(ProjectModel project)
        {
            _vsProject.References.AddProject(project._vsProject.Project);
        }

        public void Save()
        {
            _vsProject.Project.Save();
        }

        public void AddReference(string assemblyPath)
        {
            _vsProject.References.Add(assemblyPath);
        }

        public void RemoveFromSolution(Solution solution)
        {
            solution.Remove(_vsProject.Project);
        }
    }
}