using System.Windows;
using System.Windows.Forms;
using EnvDTE;
using MyToolkit.Mvvm;
using NuGetReferenceSwitcher.Presentation.Domain;
using NuGetReferenceSwitcher.Presentation.ViewModels;
using Button = System.Windows.Controls.Button;
using Window = System.Windows.Window;

namespace NuGetReferenceSwitcher.Presentation.Views
{
    /// <summary>
    /// Interaction logic for MainDialog.xaml
    /// </summary>
    public partial class MainDialog : Window
    {
        public MainDialog(DTE application)
        {
            InitializeComponent();
            Model.Application = application;
            Model.Dispatcher = Dispatcher;
            ViewModelHelper.RegisterViewModel(Model, this);
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var swi = (FromAssemblyToProjectSwitch)((Button)sender).Tag;

            var dlg = new OpenFileDialog();
            dlg.Filter = "Project Files (*.csproj)|*.csproj";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                swi.ProjectPath = dlg.FileName;
        }
    }
}
