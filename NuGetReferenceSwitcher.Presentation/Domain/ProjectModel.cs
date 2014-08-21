//-----------------------------------------------------------------------
// <copyright file="ProjectModel.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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

        public string CurrentConfigurationPath
        {
            get { return GetConfigurationPath(".nugetreferenceswitcher"); }
        }

        public string DefaultConfigurationPath
        {
            get { return GetConfigurationPath(".default.nugetreferenceswitcher"); }
        }
        
        public List<FromProjectToNuGetSwitch> CurrentSwitches
        {
            get { return GetSwitches(CurrentConfigurationPath); }
        }

        public List<FromProjectToNuGetSwitch> DefaultSwitches
        {
            get { return GetSwitches(DefaultConfigurationPath); }
        }

        public string Path
        {
            get { return _vsProject.Project.FileName; }
        }

        public void DeleteConfigurationFile()
        {               
            if (File.Exists(CurrentConfigurationPath))
            {
                if (File.Exists(DefaultConfigurationPath))
                    File.Delete(DefaultConfigurationPath);

                File.Move(CurrentConfigurationPath, DefaultConfigurationPath);
            }
        }

        private string GetConfigurationPath(string fileExtension)
        {
            var projectPath = _vsProject.Project.FileName;
            var projectDirectory = System.IO.Path.GetDirectoryName(projectPath);
            var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
            return System.IO.Path.Combine(projectDirectory, projectName + fileExtension);
        }

        private List<FromProjectToNuGetSwitch> GetSwitches(string configurationPath)
        {
            var list = new List<FromProjectToNuGetSwitch>();
            if (File.Exists(configurationPath))
            {
                var lines = File.ReadAllLines(configurationPath)
                    .Select(l => l.Split('\t'))
                    .Where(l => l.Length == 3).ToArray();

                foreach (var line in lines)
                    list.Add(new FromProjectToNuGetSwitch { FromProjectName = line[0], FromProjectPath = line[1], ToAssemblyPath = line[2] });
            }
            return list;
        }

        private void LoadReferences()
        {
            References = new ExtendedObservableCollection<ReferenceModel>();
            NuGetReferences = new ExtendedObservableCollection<ReferenceModel>();

            foreach (var vsReference in _vsProject.References.OfType<Reference>())
            {
                var reference = new ReferenceModel(vsReference);
                References.Add(reference);
                if (vsReference.Path.Contains("/packages/") || vsReference.Path.Contains("\\packages\\"))
                    NuGetReferences.Add(reference);
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
            _vsProject.Refresh();
            solution.Remove(_vsProject.Project);
        }
    }
}