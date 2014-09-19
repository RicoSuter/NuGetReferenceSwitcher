//-----------------------------------------------------------------------
// <copyright file="MainDialog.xaml.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using EnvDTE;
using MyToolkit.Collections;
using MyToolkit.Mvvm;
using NuGetReferenceSwitcher.Presentation.Model;
using NuGetReferenceSwitcher.Presentation.ViewModels;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Window = System.Windows.Window;

namespace NuGetReferenceSwitcher.Presentation.Views
{
    /// <summary>Interaction logic for MainDialog.xaml </summary>
    public partial class MainDialog : Window
    {
        /// <summary>Initializes a new instance of the <see cref="MainDialog"/> class. </summary>
        /// <param name="application">The application object. </param>
        /// <param name="extensionAssembly">The assembly of the extension. </param>
        public MainDialog(DTE application, Assembly extensionAssembly)
        {
            InitializeComponent();

            Model.ExtensionAssembly = extensionAssembly; 
            Model.Application = application;
            Model.Dispatcher = Dispatcher;

            ViewModelHelper.RegisterViewModel(Model, this);

            Model.Projects.ExtendedCollectionChanged += OnProjectsChanged;
            KeyUp += OnKeyUp;
        }

        /// <summary>Gets the view model. </summary>
        public MainDialogModel Model
        {
            get { return (MainDialogModel)Resources["ViewModel"]; }
        }

        private void OnOpenHyperlink(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;
            System.Diagnostics.Process.Start(uri.ToString());
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
                Close();
        }

        private void OnProjectsChanged(object sender, ExtendedNotifyCollectionChangedEventArgs<ProjectModel> args)
        {
            if (Model.Projects.Any(p => p.CurrentToNuGetTransformations.Any()))
                Tabs.SelectedIndex = 1;
        }

        private async void OnSwitchToProjectReferences(object sender, RoutedEventArgs e)
        {
            await Model.SwitchToProjectReferencesAsync();
            Close();
        }

        private async void OnSwitchToNuGetReferences(object sender, RoutedEventArgs e)
        {
            await Model.SwitchToNuGetReferencesAsync();
            Close();
        }

        private void OnSelectProjectFile(object sender, RoutedEventArgs e)
        {
            var fntpSwitch = (FromNuGetToProjectTransformation)((Button)sender).Tag;
            var dlg = new OpenFileDialog();
            dlg.Filter = "Project Files (*.csproj)|*.csproj";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                fntpSwitch.ToProjectPath = dlg.FileName;
        }
    }
}
