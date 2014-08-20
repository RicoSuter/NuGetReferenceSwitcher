//-----------------------------------------------------------------------
// <copyright file="ReferenceModel.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using EnvDTE;
using VSLangProj;

namespace NuGetReferenceSwitcher.Presentation.Domain
{
    public class ReferenceModel
    {
        private readonly Reference _reference;

        public ReferenceModel(Reference reference)
        {
            _reference = reference;
            
            Name = reference.Name;
            Path = reference.Path;
            ProjectName = _reference.SourceProject != null ? _reference.SourceProject.Name : null;
        }

        public string Name { get; private set; }

        public string Path { get; set; }

        public string ProjectName { get; private set; }

        public void Remove()
        {
            _reference.Remove();
        }
    }
}