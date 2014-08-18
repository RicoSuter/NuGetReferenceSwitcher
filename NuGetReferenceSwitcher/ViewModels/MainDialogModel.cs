using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using EnvDTE;
using MyToolkit.Collections;
using MyToolkit.Mvvm;
using RicoSuter.NuGetReferenceSwitcher.Domain;
using VSLangProj;

namespace RicoSuter.NuGetReferenceSwitcher.ViewModels
{
    public class MainDialogModel : ViewModelBase
    {
        public MainDialogModel()
        {
            Projects = new ExtendedObservableCollection<MyProject>();
            Switches = new ExtendedObservableCollection<AssemblyToProjectSwitch>();

            RemoveProjects = true;
            SaveProjects = true;
        }

        public DTE Application { get; set; }
        public Dispatcher Dispatcher { get; set; }

        public ExtendedObservableCollection<MyProject> Projects { get; private set; }

        public ExtendedObservableCollection<AssemblyToProjectSwitch> Switches { get; private set; }

        public bool SaveProjects { get; set; }

        public bool RemoveProjects { get; set; }

        /// <summary>Initializes the view model. Must only be called once per view model instance 
        /// (after the InitializeComponent method of a <see cref="!:UserControl"/>). </summary>
        public async override void Initialize()
        {
            List<MyProject> projects = null;

            await RunTaskAsync(token => Task.Run(() =>
            {
                if (Application != null)
                {
                    if (Application.Solution != null)
                    {
                        projects = Application.Solution.Projects.OfType<Project>()
                            .Where(p => p.Object is VSProject)
                            .Select(p => (VSProject)p.Object)
                            .Select(project => new MyProject(project)).ToList();
                    }
                }
            }, token));

            Projects.Initialize(projects);
            Switches.Initialize(projects
                .SelectMany(p => p.NuGetReferences)
                .GroupBy(r => r.Name)
                .Select(g => new AssemblyToProjectSwitch(g.First())));
        }

        public async Task SwitchToProjectReferencesAsync()
        {
            await RunTaskAsync(token => Task.Run(() =>
            {
                foreach (var project in Projects.ToArray())
                {
                    var assemblyReferenceSwitchesForProject = "";
                    foreach (var assemblyToProjectSwitch in Switches.Where(p => !p.IsDeactivated))
                    {
                        AddProjectToSolution(assemblyToProjectSwitch);

                        var reference = project.NuGetReferences
                            .FirstOrDefault(r => r.Path == assemblyToProjectSwitch.FromAssemblyPath);

                        if (reference != null)
                        {
                            reference.Reference.Remove();
                            var projectToAdd = assemblyToProjectSwitch.IsProjectPathSelected
                                ? assemblyToProjectSwitch.ToProjectFromPath.Project.Project
                                : assemblyToProjectSwitch.ToProject.Project.Project;
                            project.Project.References.AddProject(projectToAdd);

                            if (SaveProjects)
                                project.Project.Project.Save();

                            assemblyReferenceSwitchesForProject +=
                                assemblyToProjectSwitch.ToProject.Name + "\t" +
                                assemblyToProjectSwitch.FromAssemblyPath + "\n";
                        }
                    }

                    if (!string.IsNullOrEmpty(assemblyReferenceSwitchesForProject))
                        File.AppendAllText(project.ConfigurationPath, assemblyReferenceSwitchesForProject);
                }
            }, token));
        }

        private void AddProjectToSolution(AssemblyToProjectSwitch assemblyToProjectSwitch)
        {
            if (assemblyToProjectSwitch.IsProjectPathSelected)
            {
                var project = Application.Solution.AddFromFile(assemblyToProjectSwitch.ProjectPath);
                var myProject = new MyProject((VSProject)project.Object);
                assemblyToProjectSwitch.ToProjectFromPath = myProject;
            }
        }

        public async Task SwitchToNuGetReferencesAsync()
        {
            await RunTaskAsync(token => Task.Run(() =>
            {
                var projectsToDelete = Projects
                    .SelectMany(p => p.ProjectToAssemblySwitches.Select(s => s.FromProjectName))
                    .Select(name => Projects.SingleOrDefault(p => p.Name == name))
                    .ToArray();

                foreach (var project in Projects)
                {
                    foreach (var line in project.ProjectToAssemblySwitches)
                    {
                        var reference = project.References
                            .FirstOrDefault(r => r.ProjectName == line.FromProjectName);

                        if (reference != null)
                        {
                            reference.Reference.Remove();
                            project.Project.References.Add(line.ToAssemblyPath);

                            if (SaveProjects)
                                project.Project.Project.Save();
                        }
                    }

                    project.DeleteConfigurationFile();
                }

                if (RemoveProjects)
                {
                    foreach (var project in projectsToDelete)
                    {
                        if (project != null)
                            Application.Solution.Remove(project.Project.Project);
                    }
                }
            }, token));
        }
    }
}
