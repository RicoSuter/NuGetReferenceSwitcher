//-----------------------------------------------------------------------
// <copyright file="MainDialogModel.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EnvDTE;
using MyToolkit.Collections;
using MyToolkit.Mvvm;
using MyToolkit.Utilities;
using NuGetReferenceSwitcher.Presentation.Domain;
using VSLangProj;

namespace NuGetReferenceSwitcher.Presentation.ViewModels
{
    public class MainDialogModel : ViewModelBase
    {
        private Assembly _extensionAssembly;

        public MainDialogModel()
        {
            Projects = new ExtendedObservableCollection<ProjectModel>();
            Transformations = new ExtendedObservableCollection<FromNuGetToProjectTransformation>();

            RemoveProjects = true;
            SaveProjects = true;
        }

        /// <summary>Gets the projects of the current solution. </summary>
        public ExtendedObservableCollection<ProjectModel> Projects { get; private set; }

        /// <summary>Gets the NuGet to project switches which are shown in the first tab. </summary>
        public ExtendedObservableCollection<FromNuGetToProjectTransformation> Transformations { get; private set; }

        /// <summary>Gets or sets a value indicating whether the changed projects should be saved. </summary>
        public bool SaveProjects { get; set; }

        /// <summary>Gets or sets a value indicating whether the previously referenced projects should be removed. </summary>
        public bool RemoveProjects { get; set; }

        /// <summary>Gets or sets the Visual Studio application object. </summary>
        public DTE Application { get; set; }

        /// <summary>Gets or sets the used UI dispatcher. </summary>
        public Dispatcher Dispatcher { get; set; }

        /// <summary>Gets or sets the assembly of the extension. </summary>
        public Assembly ExtensionAssembly
        {
            get { return _extensionAssembly; }
            set 
            { 
                if (Set(ref _extensionAssembly, value))
                    RaisePropertyChanged(() => ExtensionVersion);
            }
        }

        /// <summary>Gets the current extension version.</summary>
        public string ExtensionVersion
        {
            get { return ExtensionAssembly.GetVersionWithBuildTime(); }
        }

        /// <summary>Initializes the view model. Must only be called once per view model instance 
        /// (after the InitializeComponent method of a <see cref="!:UserControl"/>). </summary>
        public async override void Initialize()
        {
            List<ProjectModel> projects = null;
            await RunTaskAsync(token => Task.Run(() =>
            {
                if (Application != null)
                {
                    if (Application.Solution != null)
                    {
                        projects = Application.Solution.Projects.OfType<Project>()
                            .Where(p => p.Object is VSProject)
                            .Select(p => (VSProject)p.Object)
                            .Select(project => new ProjectModel(project)).ToList();
                    }
                }
            }, token));

            Projects.Initialize(projects);
            Transformations.Initialize(projects
                .SelectMany(p => p.NuGetReferences)
                .GroupBy(r => r.Name)
                .Select(g => new FromNuGetToProjectTransformation(projects, g.First()))
                .OrderBy(s => s.FromAssemblyName));
        }

        /// <summary>Switches the NuGet DLL references to the selected project references. </summary>
        /// <returns>The task. </returns>
        public async Task SwitchToProjectReferencesAsync()
        {
            await RunTaskAsync(token => Task.Run(() =>
            {
                // add required projects to solution
                var activeProjectPaths = Transformations.Where(p => !p.IsDeactivated).Select(p => p.ToProjectPath);
                var projectBuildOrder = ProjectDependencyResolver.GetBuildOrder(activeProjectPaths).ToList();
                var activeTransformations = Transformations
                    .Where(p => !p.IsDeactivated)
                    .OrderBy(p => projectBuildOrder.IndexOf(p.ToProjectPath))
                    .ToList();

                foreach (var transformation in activeTransformations)
                    AddProjectToSolutionIfNeeded(transformation);

                // add references
                foreach (var project in Projects.ToArray())
                {
                    var nuGetReferenceTransformationsForProject = "";
                    foreach (var assemblyToProjectSwitch in Transformations.Where(p => !p.IsDeactivated))
                    {
                        var reference = project.NuGetReferences
                            .FirstOrDefault(r => r.Path == assemblyToProjectSwitch.FromAssemblyPath);

                        if (reference != null)
                        {
                            reference.Remove();

                            project.AddProjectReference(assemblyToProjectSwitch.ToProject);
                            nuGetReferenceTransformationsForProject +=
                                assemblyToProjectSwitch.ToProject.Name + "\t" +
                                assemblyToProjectSwitch.ToProject.Path + "\t" +
                                assemblyToProjectSwitch.FromAssemblyPath + "\n";

                            if (SaveProjects)
                                project.Save();
                        }
                    }

                    if (!string.IsNullOrEmpty(nuGetReferenceTransformationsForProject))
                        File.AppendAllText(project.CurrentConfigurationPath, nuGetReferenceTransformationsForProject);
                }
            }, token));
        }

        /// <summary>Handles an exception which occured in the <see cref="M:MyToolkit.Mvvm.ViewModelBase.RunTaskAsync(System.Func{System.Threading.CancellationToken,System.Threading.Tasks.Task})"/> method. </summary>
        /// <param name="exception">The exception. </param>
        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message, "An error occurred");
        }

        /// <summary>Switches the project references to the previously referenced NuGet DLLs. </summary>
        /// <returns>The task. </returns>
        public async Task SwitchToNuGetReferencesAsync()
        {
            await RunTaskAsync(token => Task.Run(() =>
            {
                var projectsToRemove = GetCurrentProjectsToRemove();
                foreach (var project in Projects)
                {
                    foreach (var transformation in project.CurrentToNuGetTransformations)
                    {
                        var reference = project.References
                            .FirstOrDefault(r => r.ProjectName == transformation.FromProjectName);

                        if (reference != null)
                        {
                            reference.Remove();
                            project.AddReference(transformation.ToAssemblyPath);

                            if (SaveProjects)
                                project.Save();
                        }
                    }

                    project.DeleteConfigurationFile();
                }

                RemoveProjectsFromSolution(projectsToRemove);
            }, token));
        }

        private void AddProjectToSolutionIfNeeded(FromNuGetToProjectTransformation fromNuGetToProjectTransformation)
        {
            if (fromNuGetToProjectTransformation.IsProjectPathSelected && !string.IsNullOrEmpty(fromNuGetToProjectTransformation.ToProjectPath))
            {
                var project = Application.Solution.AddFromFile(fromNuGetToProjectTransformation.ToProjectPath);
                var myProject = new ProjectModel((VSProject)project.Object);
                fromNuGetToProjectTransformation.ToProject = myProject;
            }
        }

        private List<ProjectModel> GetCurrentProjectsToRemove()
        {
            return Projects
                .SelectMany(p => p.CurrentToNuGetTransformations.Select(s => s.FromProjectName))
                .Select(name => Projects.SingleOrDefault(p => p.Name == name))
                .ToList();
        }

        private void RemoveProjectsFromSolution(List<ProjectModel> projectsToDelete)
        {
            if (RemoveProjects)
            {
                var pathsOfProjectsToRemove = projectsToDelete.Select(p => p.Path);
                var projectBuildOrder = ProjectDependencyResolver.GetBuildOrder(pathsOfProjectsToRemove).ToList();
                var orderedProjectsToRemove = projectsToDelete
                    .OrderByDescending(p => projectBuildOrder.IndexOf(p.Path)).ToList();

                foreach (var project in orderedProjectsToRemove)
                {
                    if (project != null)
                        project.RemoveFromSolution(Application.Solution);
                }
            }
        }
    }
}
