//-----------------------------------------------------------------------
// <copyright file="MainDialog.xaml.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using EnvDTE;
using MyToolkit.Collections;
using MyToolkit.Mvvm;
using NuGetReferenceSwitcher.Presentation.Domain;
using NuGetReferenceSwitcher.Presentation.ViewModels;
using Button = System.Windows.Controls.Button;
using Window = System.Windows.Window;

namespace NuGetReferenceSwitcher.Presentation.Views
{
    /// <summary>Interaction logic for MainDialog.xaml </summary>
    public partial class MainDialog : Window
    {
        public MainDialog(DTE application)
        {
            InitializeComponent();
            Model.Application = application;
            Model.Dispatcher = Dispatcher;
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Projects.ExtendedCollectionChanged += OnProjectsChanged;
        }

        private void OnProjectsChanged(object sender, ExtendedNotifyCollectionChangedEventArgs<ProjectModel> args)
        {
            if (Model.Projects.Any(p => p.CurrentSwitches.Any()))
                Tabs.SelectedIndex = 1;
        }

        public MainDialogModel Model
        {
            get { return (MainDialogModel)Resources["ViewModel"]; }
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
            var swi = (FromAssemblyToProjectSwitch)((Button)sender).Tag;

            var dlg = new OpenFileDialog();
            dlg.Filter = "Project Files (*.csproj)|*.csproj";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                swi.ProjectPath = dlg.FileName;
        }
    }
}
