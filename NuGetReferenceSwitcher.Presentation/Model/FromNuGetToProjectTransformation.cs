//-----------------------------------------------------------------------
// <copyright file="FromNuGetToProjectTransformation.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyToolkit.Model;

namespace NuGetReferenceSwitcher.Presentation.Model
{
    public class FromNuGetToProjectTransformation : ObservableObject
    {
        private string _toProjectPath;
        private bool _isProjectPathSelected;
        private bool _isDeactivated;

        /// <summary>Initializes a new instance of the <see cref="FromNuGetToProjectTransformation"/> class. </summary>
        /// <param name="projects">The projects. </param>
        /// <param name="assemblyReference">The assembly reference. </param>
        public FromNuGetToProjectTransformation(List<ProjectModel> projects, ReferenceModel assemblyReference)
        {
            FromAssemblyName = assemblyReference.Name;
            FromAssemblyPath = assemblyReference.Path;

            var swi = projects.SelectMany(p => p.PreviousToNuGetTransformations).SingleOrDefault(s => s.ToAssemblyPath == FromAssemblyPath);
            if (swi != null)
            {
                var targetProject = projects.FirstOrDefault(p => p.PreviousToNuGetTransformations.Contains(swi));
                if (targetProject == null)
                {
                    ToProjectPath = swi.FromProjectPath;
                    IsProjectPathSelected = true;
                }
                else
                {
                    ToProject = projects.First(p => p.PreviousToNuGetTransformations.Contains(swi));
                    IsProjectPathSelected = false;
                }
            }
            else
                IsDeactivated = true;
        }

        /// <summary>Gets or sets the NuGet assembly name to switch. </summary>
        public string FromAssemblyName { get; set; }

        /// <summary>Gets or sets the NuGet assembly path to switch. </summary>
        public string FromAssemblyPath { get; set; }

        /// <summary>Gets or sets a value indicating whether the switch is deactivated. </summary>
        public bool IsDeactivated
        {
            get { return _isDeactivated; }
            set { Set(ref _isDeactivated, value); }
        }
        
        /// <summary>Gets or sets a value indicating whether a project path is selected. </summary>
        public bool IsProjectPathSelected
        {
            get { return _isProjectPathSelected; }
            set
            {
                if (Set(ref _isProjectPathSelected, value))
                    ToProject = null;
            }
        }
        
        /// <summary>Gets or sets the projectPath. </summary>
        public string ToProjectPath
        {
            get { return _toProjectPath; }
            set { Set(ref _toProjectPath, value); }
        }

        /// <summary>Gets or sets the target project to switch to. </summary>
        public ProjectModel ToProject { get; set; }
    }
}