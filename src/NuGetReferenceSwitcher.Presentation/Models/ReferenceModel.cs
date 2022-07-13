//-----------------------------------------------------------------------
// <copyright file="ReferenceModel.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using VSLangProj;


namespace NuGetReferenceSwitcher.Presentation.Models
{
    public class ReferenceModel
    {
        private readonly Reference _reference;

        /// <summary>Initializes a new instance of the <see cref="ReferenceModel"/> class. </summary>
        /// <param name="reference">The native reference object. </param>
        public ReferenceModel(Reference reference)
        {
            _reference = reference;
            
            Name = reference.Name;
            Path = reference.Path;
            ProjectName = _reference.SourceProject != null ? _reference.SourceProject.Name : null;
        }

        /// <summary>Gets the name of the reference. </summary>
        public string Name { get; private set; }

        /// <summary>Gets or sets the referenced path. </summary>
        public string Path { get; set; }

        /// <summary>Gets the referenced project name if this is a project reference otherwise null. </summary>
        public string ProjectName { get; private set; }

        /// <summary>Removes the reference from the project. </summary>
        public void Remove()
        {
            _reference.Remove();
        }
    }
}