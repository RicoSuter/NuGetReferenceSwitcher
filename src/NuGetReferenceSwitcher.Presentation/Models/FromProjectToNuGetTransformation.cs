//-----------------------------------------------------------------------
// <copyright file="FromProjectToNuGetTransformation.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NuGetReferenceSwitcher.Presentation.Models
{
    public class FromProjectToNuGetTransformation
    {
        /// <summary>Gets or sets the project name to switch. </summary>
        public string FromProjectName { get; set; }

        /// <summary>Gets or sets the project path to switch. </summary>
        public string FromProjectPath { get; set; }

        /// <summary>Gets or sets the NuGet assembly path name to switch to. </summary>
        public string ToAssemblyPath { get; set; }
    }
}