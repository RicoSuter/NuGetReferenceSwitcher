//-----------------------------------------------------------------------
// <copyright file="FromNuGetToProjectTransformation.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyToolkit.Model;

namespace NuGetReferenceSwitcher.Presentation.Models
{
    public enum NuGetToProjectMode
    {
        Deactivated,
        ProjectPath,
        Project
    }

    public class FromNuGetToProjectTransformation : ObservableObject
    {
        private string _toProjectPath;
        private ProjectModel _toProject;

        /// <summary>Initializes a new instance of the <see cref="FromNuGetToProjectTransformation"/> class. </summary>
        /// <param name="projects">The projects. </param>
        /// <param name="assemblyReference">The assembly reference. </param>
        public FromNuGetToProjectTransformation(List<ProjectModel> projects, ReferenceModel assemblyReference)
        {
            FromAssemblyName = assemblyReference.Name;
            FromAssemblyPath = assemblyReference.Path;

            var swi = projects
                .SelectMany(p => p.PreviousToNuGetTransformations)
                .FirstOrDefault(s => Path.GetFileNameWithoutExtension(s.ToAssemblyPath) == FromAssemblyName);

            if (swi != null)
            {
                var targetProject = projects.FirstOrDefault(p => p.Path == swi.FromProjectPath);
                if (targetProject == null)
                    ToProjectPath = swi.FromProjectPath;
                else
                    ToProject = targetProject;
            }
            else
                SelectedMode = NuGetToProjectMode.Deactivated;
        }

        /// <summary>Gets or sets the NuGet assembly name to switch. </summary>
        public string FromAssemblyName { get; set; }

        /// <summary>Gets or sets the NuGet assembly path to switch. </summary>
        public string FromAssemblyPath { get; set; }

        private NuGetToProjectMode _selectedMode;

        /// <summary>Gets or sets a value indicating whether the switch is deactivated. </summary>
        public NuGetToProjectMode SelectedMode
        {
            get { return _selectedMode; }
            set { Set(ref _selectedMode, value); }
        }

        /// <summary>Gets or sets the projectPath. </summary>
        public string ToProjectPath
        {
            get { return _toProjectPath; }
            set
            {
                if (Set(ref _toProjectPath, value))
                {
                    if (_toProjectPath == null)
                        SelectedMode = NuGetToProjectMode.Project;
                    else
                        SelectedMode = NuGetToProjectMode.ProjectPath;
                }
            }
        }

        /// <summary>Gets or sets the target project to switch to. </summary>
        public ProjectModel ToProject
        {
            get { return _toProject; }
            set
            {
                if (Set(ref _toProject, value))
                {
                    if (_toProject == null)
                        SelectedMode = NuGetToProjectMode.ProjectPath;
                    else
                        SelectedMode = NuGetToProjectMode.Project;
                }
            }
        }

        /// <summary>Gets the evaluated to project path. </summary>
        public string EvaluatedToProjectPath
        {
            get { return SelectedMode == NuGetToProjectMode.ProjectPath ? ToProjectPath : ToProject.Path; }
        }
    }
}