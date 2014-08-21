//-----------------------------------------------------------------------
// <copyright file="FromNuGetToProjectSwitch.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyToolkit.Model;

namespace NuGetReferenceSwitcher.Presentation.Domain
{
    public class FromNuGetToProjectSwitch : ObservableObject
    {
        private string _projectPath;
        private bool _isProjectPathSelected;
        private bool _isDeactivated;

        public FromNuGetToProjectSwitch(List<ProjectModel> projects, ReferenceModel assemblyReference)
        {
            FromAssemblyName = assemblyReference.Name;
            FromAssemblyPath = assemblyReference.Path;

            var swi = projects.SelectMany(p => p.DefaultSwitches).SingleOrDefault(s => s.ToAssemblyPath == FromAssemblyPath);
            if (swi != null)
            {
                var targetProject = projects.FirstOrDefault(p => p.DefaultSwitches.Contains(swi));
                if (targetProject == null)
                {
                    ProjectPath = swi.FromProjectPath;
                    IsProjectPathSelected = true;
                }
                else
                {
                    ToProject = projects.First(p => p.DefaultSwitches.Contains(swi));
                    IsProjectPathSelected = false;
                }
            }
            else
                IsDeactivated = true;
        }

        public string FromAssemblyName { get; set; }

        public string FromAssemblyPath { get; set; }

        public ProjectModel ToProject { get; set; }

        /// <summary>Gets or sets the projectPath. </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set { Set(ref _projectPath, value); }
        }

        /// <summary>Gets or sets a value indicating whether a project path is selected. </summary>
        public bool IsProjectPathSelected
        {
            get { return _isProjectPathSelected; }
            set { Set(ref _isProjectPathSelected, value); }
        }
        
        /// <summary>Gets or sets a value indicating whether the switch is deactivated. </summary>
        public bool IsDeactivated
        {
            get { return _isDeactivated; }
            set { Set(ref _isDeactivated, value); }
        }

        public ProjectModel ToProjectFromPath { get; set; }

        // TODO: Add ToProjectPath => add to solution first => either ToProjectPath or ToProject
    }
}