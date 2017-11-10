//-----------------------------------------------------------------------
// <copyright file="NuGetReferenceSwitcherPackage.cs" company="NuGet Reference Switcher">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using NuGetReferenceSwitcher.Presentation.Views;
using VSLangProj;

namespace RicoSuter.NuGetReferenceSwitcher
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidNuGetReferenceSwitcherPkgString)]
    public sealed class NuGetReferenceSwitcherPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                var menuCommandId = new CommandID(GuidList.guidNuGetReferenceSwitcherCmdSet, (int)PkgCmdIDList.cmdidSwitchNuGetAndProjectReferences);
                var menuItem = new MenuCommand(OnShowDialog, menuCommandId);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnShowDialog(object sender, EventArgs e)
        {
            var application = (DTE)GetService(typeof(SDTE));
            if (application.Solution == null || !application.Solution.IsOpen)
                MessageBox.Show("Please open a solution first. ", "No solution");
            else
            {
                if (application.Solution.IsDirty) // solution must be saved otherwise adding/removing projects will raise errors
                {
                    MessageBox.Show("Please save your solution first. \n" +
                                    "Select the solution in the Solution Explorer and press Ctrl-S. ",
                                    "Solution not saved");
                }
                else if (application.Solution.Projects.OfType<Project>().Any(p => p.IsDirty))
                {
                    MessageBox.Show("Please save your projects first. \n" +
                                    "Select the project in the Solution Explorer and press Ctrl-S. ",
                                    "Project not saved");
                }
                else
                {
                    var window = new MainDialog(application, GetType().Assembly);
                    var helper = new WindowInteropHelper(window);
                    helper.Owner = (IntPtr)application.MainWindow.HWnd;
                    window.ShowModal();
                }
            }
        }
    }
}
