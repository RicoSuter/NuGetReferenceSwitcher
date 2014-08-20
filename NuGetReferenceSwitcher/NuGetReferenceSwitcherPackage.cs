//-----------------------------------------------------------------------
// <copyright file="NuGetReferenceSwitcherPackage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://nugetreferenceswitcher.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
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
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
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
        #endregion

        private void OnShowDialog(object sender, EventArgs e)
        {
            var application = (DTE)GetService(typeof(SDTE));
            var window = new MainDialog(application);
            var helper = new WindowInteropHelper(window);
            helper.Owner = (IntPtr) application.MainWindow.HWnd;
            window.ShowDialog();
        }
    }
}
