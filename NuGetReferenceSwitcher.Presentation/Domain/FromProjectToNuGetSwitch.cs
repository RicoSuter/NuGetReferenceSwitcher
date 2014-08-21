//-----------------------------------------------------------------------
// <copyright file="FromProjectToNuGetSwitch.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NuGetReferenceSwitcher.Presentation.Domain
{
    public class FromProjectToNuGetSwitch
    {
        public string FromProjectName { get; set; }
        public string FromProjectPath { get; set; }
        public string ToAssemblyPath { get; set; }
    }
}