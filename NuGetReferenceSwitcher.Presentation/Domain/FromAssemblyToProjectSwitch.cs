using MyToolkit.Model;

namespace NuGetReferenceSwitcher.Presentation.Domain
{
    public class FromAssemblyToProjectSwitch : ObservableObject
    {
        private string _projectPath;
        private bool _isProjectPathSelected;
        private bool _isDeactivated;

        public FromAssemblyToProjectSwitch(ReferenceModel assemblyReference)
        {
            FromAssemblyName = assemblyReference.Name;
            FromAssemblyPath = assemblyReference.Path;

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

        /// <summary>Gets or sets a value indicating whether TODO. </summary>
        public bool IsProjectPathSelected
        {
            get { return _isProjectPathSelected; }
            set { Set(ref _isProjectPathSelected, value); }
        }
        
        /// <summary>Gets or sets a value indicating whether TODO. </summary>
        public bool IsDeactivated
        {
            get { return _isDeactivated; }
            set { Set(ref _isDeactivated, value); }
        }

        public ProjectModel ToProjectFromPath { get; set; }

        // TODO: Add ToProjectPath => add to solution first => either ToProjectPath or ToProject
    }
}