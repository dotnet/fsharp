/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace Microsoft.TestSccPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(Guids.guidSccPackagePkgString)]
    [ProvideService(typeof(TestSccProvider), ServiceName="Test Source Provider")]
    [ProvideSourceControlProvider("Test Source Provider", Guids.guidSccPackageCmdSetString, typeof(SccPackage), typeof(TestSccProvider))]
    [ProvideMenuResource(1000, 1)]                              // This attribute is needed to let the shell know that this package exposes some menus.
    public sealed class SccPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SccPackage()
        {            
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            CommandID replWindowCmdId = new CommandID(Guids.guidSccPackageCmdSet, PkgCmdId.cmdidShowDocEvents);
            MenuCommand showDocEventsCmdId = new MenuCommand(ShowDocEvents, replWindowCmdId);
            mcs.AddCommand(showDocEventsCmdId);

            CommandID clearDocEventsCmdId = new CommandID(Guids.guidSccPackageCmdSet, PkgCmdId.cmdidClearDocEvents);
            MenuCommand openRemoteDebugProxyFolderCmd = new MenuCommand(ClearDocEvents, clearDocEventsCmdId);
            mcs.AddCommand(openRemoteDebugProxyFolderCmd);

            var trackDocs = (IVsTrackProjectDocuments2)this.GetService(typeof(SVsTrackProjectDocuments));
            ((IServiceContainer)this).AddService(typeof(TestSccProvider), new TestSccProvider(trackDocs), true);
        }

        private void ClearDocEvents(object sender, EventArgs e) {
            TestSccProvider.DocumentEvents.Clear();
            TestSccProvider.CodeDocumentEvents.Clear();
        }

        private void ShowDocEvents(object sender, EventArgs e) {
            MessageBox.Show(
                String.Join(
                    Environment.NewLine,
                    TestSccProvider.DocumentEvents
                ) + CodeHeader +
                String.Join(
                    CodeLineSeperator + Environment.NewLine,
                    TestSccProvider.CodeDocumentEvents
                )
            );
        }

        const string CodeHeader = @"
-----------------------------------------------------------
Code version:
";

        const string CodeLineSeperator = ", ";

        #endregion
    }
}
