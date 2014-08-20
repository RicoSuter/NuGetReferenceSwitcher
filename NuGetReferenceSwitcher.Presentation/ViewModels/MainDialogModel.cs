//-----------------------------------------------------------------------
// <copyright file="MainDialogModel.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EnvDTE;
using MyToolkit.Collections;
using MyToolkit.Mvvm;
using NuGetReferenceSwitcher.Presentation.Domain;
using VSLangProj;

namespace NuGetReferenceSwitcher.Presentation.ViewModels
{
    public class MainDialogModel : ViewModelBase
    {
        public MainDialogModel()
        {
            Projects = new ExtendedObservableCollection<ProjectModel>();
            Switches = new ExtendedObservableCollection<FromAssemblyToProjectSwitch>();

            RemoveProjects = true;
            SaveProjects = true;
        }

        public ExtendedObservableCollection<ProjectModel> Projects { get; private set; }

        public ExtendedObservableCollection<FromAssemblyToProjectSwitch> Switches { get; private set; }

        public bool SaveProjects { get; set; }

        public bool RemoveProjects { get; set; }

        public DTE Application { get; set; }

        public Dispatcher Dispatcher { get; set; }

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
            Switches.Initialize(projects
                .SelectMany(p => p.NuGetReferences)
                .GroupBy(r => r.Name)
                .Select(g => new FromAssemblyToProjectSwitch(projects, g.First())));
        }

        public async Task SwitchToProjectReferencesAsync()
        {
            await RunTaskAsync(token => Task.Run(() =>
            {
                // add required projects to solution
                var order = ProjectDependencyResolver.GetBuildOrder(Switches.Where(p => !p.IsDeactivated).Select(p => p.ProjectPath)).ToList();
                var switches = Switches.Where(p => !p.IsDeactivated).OrderBy(p => order.IndexOf(p.ProjectPath)).ToList();

                foreach (var assemblyToProjectSwitch in switches)
                {
                    AddProjectToSolution(assemblyToProjectSwitch);
                }

                // add references
                foreach (var project in Projects.ToArray())
                {
                    var assemblyReferenceSwitchesForProject = "";
                    foreach (var assemblyToProjectSwitch in Switches.Where(p => !p.IsDeactivated))
                    {
                        var reference = project.NuGetReferences
                            .FirstOrDefault(r => r.Path == assemblyToProjectSwitch.FromAssemblyPath);

                        if (reference != null)
                        {
                            reference.Remove();

                            if (assemblyToProjectSwitch.IsProjectPathSelected)
                            {
                                project.AddProject(assemblyToProjectSwitch.ToProjectFromPath);

                                assemblyReferenceSwitchesForProject +=
                                    assemblyToProjectSwitch.ToProjectFromPath.Name + "\t" +
                                    assemblyToProjectSwitch.ToProjectFromPath.Path + "\t" +
                                    assemblyToProjectSwitch.FromAssemblyPath + "\n";
                            }
                            else
                            {
                                project.AddProject(assemblyToProjectSwitch.ToProject);
                                assemblyReferenceSwitchesForProject +=
                                    assemblyToProjectSwitch.ToProject.Name + "\t" +
                                    assemblyToProjectSwitch.ToProject.Path + "\t" +
                                    assemblyToProjectSwitch.FromAssemblyPath + "\n";
                            }

                            if (SaveProjects)
                                project.Save();
                        }
                    }

                    if (!string.IsNullOrEmpty(assemblyReferenceSwitchesForProject))
                        File.AppendAllText(project.CurrentConfigurationPath, assemblyReferenceSwitchesForProject);
                }
            }, token));
        }

        private void AddProjectToSolution(FromAssemblyToProjectSwitch fromAssemblyToProjectSwitch)
        {
            if (fromAssemblyToProjectSwitch.IsProjectPathSelected)
            {
                var project = Application.Solution.AddFromFile(fromAssemblyToProjectSwitch.ProjectPath);
                var myProject = new ProjectModel((VSProject)project.Object);
                fromAssemblyToProjectSwitch.ToProjectFromPath = myProject;
            }
        }

        public async Task SwitchToNuGetReferencesAsync()
        {
            await RunTaskAsync(token => Task.Run(() =>
            {
                var projectsToDelete = Projects
                    .SelectMany(p => p.CurrentSwitches.Select(s => s.FromProjectName))
                    .Select(name => Projects.SingleOrDefault(p => p.Name == name))
                    .ToList();

                foreach (var project in Projects)
                {
                    foreach (var line in project.CurrentSwitches)
                    {
                        var reference = project.References
                            .FirstOrDefault(r => r.ProjectName == line.FromProjectName);

                        if (reference != null)
                        {
                            reference.Remove();
                            project.AddReference(line.ToAssemblyPath);

                            if (SaveProjects)
                                project.Save();
                        }
                    }

                    project.DeleteConfigurationFile();
                }

                if (RemoveProjects)
                {
                    var order = ProjectDependencyResolver.GetBuildOrder(projectsToDelete.Select(p => p.Path)).ToList();
                    foreach (var project in projectsToDelete.OrderBy(p => order.IndexOf(p.Path) * -1))
                    {
                        if (project != null)
                            project.RemoveFromSolution(Application.Solution);
                    }
                }
            }, token));
        }

        /// <summary>Handles an exception which occured in the <see cref="M:MyToolkit.Mvvm.ViewModelBase.RunTaskAsync(System.Func{System.Threading.CancellationToken,System.Threading.Tasks.Task})"/> method. </summary>
        /// <param name="exception">The exception. </param>
        public override void HandleException(Exception exception)
        {
            MessageBox.Show("Error: " + exception.Message); // TODO: Exception handling        
        }
    }
}
