//-----------------------------------------------------------------------
// <copyright file="ProjectModel.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Xml;
using EnvDTE;
using MyToolkit.Collections;
using VSLangProj;

namespace NuGetReferenceSwitcher.Presentation.Models
{
    public class ProjectModel
    {
        private readonly VSProject _vsProject;

        public FileInfo SolutionFile { get; set; }

        /// <summary>Initializes a new instance of the <see cref="ProjectModel"/> class. </summary>
        /// <param name="project">The native project object. </param>
        /// <param name="application">The native application object. </param>
        public ProjectModel(VSProject project, DTE application)
        {
            _vsProject = project;

			Name = project.Project.Name;
            SolutionFile = new FileInfo(application.Solution.FileName);
            LoadReferences();
        }

        /// <summary>Gets the name of the project. </summary>
        public string Name { get; private set; }

        /// <summary>Gets the project's references. </summary>
        public ExtendedObservableCollection<ReferenceModel> References { get; private set; }

        /// <summary>Gets the project's NuGet references (subset of References). </summary>
        public ExtendedObservableCollection<ReferenceModel> NuGetReferences { get; private set; }

        /// <summary>Gets the path of the current configuration. </summary>
        public string CurrentConfigurationPath
        {
            get { return GetConfigurationPath(".nugetreferenceswitcher"); }
        }

        /// <summary>Gets the path of the previous configuration. </summary>
        public string PreviousConfigurationPath
        {
            get { return GetConfigurationPath(".previous.nugetreferenceswitcher"); }
        }
        
        /// <summary>Gets the current project reference to NuGet reference transformations. </summary>
        public List<FromProjectToNuGetTransformation> CurrentToNuGetTransformations
        {
            get { return LoadTransformationsFromFile(CurrentConfigurationPath); }
        }

        /// <summary>Gets the previous project reference to NuGet reference transformations. </summary>
        public List<FromProjectToNuGetTransformation> PreviousToNuGetTransformations
        {
            get { return LoadTransformationsFromFile(PreviousConfigurationPath); }
        }

        /// <summary>Gets the project file path. </summary>
        public string Path
        {
            get { return _vsProject.Project.FileName; }
        }

        /// <summary>Deletes the previous configuration file and renames the current 
        /// configuration file to the path of the previous configuration file.  </summary>
        public void DeleteConfigurationFile()
        {               
            if (File.Exists(CurrentConfigurationPath))
            {
                if (File.Exists(PreviousConfigurationPath))
                    File.Delete(PreviousConfigurationPath);

                File.Move(CurrentConfigurationPath, PreviousConfigurationPath);
            }
        }

        /// <summary>Adds a project reference to the project. </summary>
        /// <param name="project">The project to add. </param>
        public void AddProjectReference(ProjectModel project)
        {
            _vsProject.References.AddProject(project._vsProject.Project);
        }

        /// <summary>Saves the project. </summary>
        public void Save()
        {
            // TODO: This may lock up the UI => check and fix
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _vsProject.Project.Save();
            });
        }

        /// <summary>Adds an assembly reference to the project.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns><c>true</c> when the file could be added.</returns>
        public bool AddReference(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                var reference = _vsProject.References.Add(assemblyPath) as Reference4;
                if (reference != null)
                    reference.SpecificVersion = true;

                return true;
            }
            return false; 
        }

        /// <summary>Removes the project from the solution. </summary>
        /// <param name="solution">The solution to remove the project from. </param>
        public void RemoveFromSolution(Solution solution)
        {
            _vsProject.Refresh();
            solution.Remove(_vsProject.Project);
        }

        private string GetConfigurationPath(string fileExtension)
        {
            var projectPath = _vsProject.Project.FileName;
            var projectDirectory = System.IO.Path.GetDirectoryName(projectPath);
            var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
            return System.IO.Path.Combine(projectDirectory, projectName + fileExtension);
        }

        private List<FromProjectToNuGetTransformation> LoadTransformationsFromFile(string configurationPath)
        {
            var list = new List<FromProjectToNuGetTransformation>();
            if (File.Exists(configurationPath))
            {
                var lines = File.ReadAllLines(configurationPath)
                    .Select(l => l.Split('\t'))
                    .Where(l => l.Length == 3).ToArray();

                

                foreach (var line in lines)
                    list.Add(new FromProjectToNuGetTransformation {
                        FromProjectName = line[0],
                        FromProjectPath = PathUtilities.MakeAbsolute(line[1], configurationPath),
                        ToAssemblyPath = PathUtilities.MakeAbsolute(line[2], configurationPath)
                    });
            }
            return list;
        }

        private void LoadReferences()
        {
            References = new ExtendedObservableCollection<ReferenceModel>();
            NuGetReferences = new ExtendedObservableCollection<ReferenceModel>();

            List<string> packageDirs = new List<string>();
            packageDirs.Add("/packages/");
            packageDirs.Add("\\packages\\");

            if (SolutionFile.Exists)
            {
                string nuGetRepositoryPath = GetNuGetRepositoryPath(SolutionFile.Directory);
                if (!string.IsNullOrEmpty(nuGetRepositoryPath))
                {
                    packageDirs.Add(nuGetRepositoryPath);
                }
            }

            foreach (var vsReference in _vsProject.References.OfType<Reference>())
            {
                var reference = new ReferenceModel(vsReference);
                References.Add(reference);
                string vsReferencePath = vsReference.Path.ToLower();
                foreach (string packageDir in packageDirs)
                {
                    if (vsReferencePath.Contains(packageDir))
                    {
                        NuGetReferences.Add(reference);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Looks for NuGet.Config files in every parent directories and returns the first repositoryPath found
        /// </summary>
        /// <param name="dir">The starting dir to look for a NuGet.config file</param>
        /// <returns>repositoryPath if a path is found and exists</returns>
        private string GetNuGetRepositoryPath(DirectoryInfo dir)
        {
            FileInfo nuGetConfigFile = dir.GetFiles("NuGet.Config").FirstOrDefault();

            if (nuGetConfigFile != null)
            {
                XmlDocument nuGetConfig = new XmlDocument();
                nuGetConfig.Load(nuGetConfigFile.FullName);
                var pathNode = nuGetConfig.SelectSingleNode("//config//add[contains(@key,'repositoryPath')]");

                if (pathNode?.Attributes?["value"] != null)
                {
                    string repositoryPath = pathNode.Attributes["value"].Value;
                    if (System.IO.Path.IsPathRooted(repositoryPath))
                    {
                        return repositoryPath;
                    }
                    else
                    {
                        if (SolutionFile?.DirectoryName != null)
                        {
                            DirectoryInfo repositoryDirectory = new DirectoryInfo(System.IO.Path.Combine(SolutionFile.DirectoryName, repositoryPath));
                            if (repositoryDirectory.Exists)
                            {
                                return repositoryDirectory.FullName.ToLower();
                            }
                        }
                    }
                }
            }

            if (dir.Parent != null)
            {
                return GetNuGetRepositoryPath(dir.Parent);
            }

            return null;
        }
    }
}