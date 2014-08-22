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
using System.Windows;
using System.Windows.Interop;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using NuGetReferenceSwitcher.Presentation.Views;

namespace RicoSuter.NuGetReferenceSwitcher
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidNuGetReferenceSwitcherPkgString)]
    public sealed class NuGetReferenceSwitcherPackage : Package
    {
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidNuGetReferenceSwitcherCmdSet, (int)PkgCmdIDList.cmdidSwitchNuGetAndProjectReferences);
                MenuCommand menuItem = new MenuCommand(OnShowDialog, menuCommandID);
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
                    MessageBox.Show("Please save your solution first. \n" +
                                    "Select the solution in the Solution Explorer and press Ctrl-S. ", 
                                    "Solution not saved");
                else
                {
                    var window = new MainDialog(application, GetType().Assembly);
                    var helper = new WindowInteropHelper(window);
                    helper.Owner = (IntPtr)application.MainWindow.HWnd;
                    window.ShowDialog();
                }
            }
        }
    }
}
