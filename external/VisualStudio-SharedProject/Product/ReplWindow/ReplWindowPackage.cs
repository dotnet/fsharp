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
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#if NTVS_FEATURE_INTERACTIVEWINDOW
using Microsoft.VisualStudio;
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
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
#if NTVS_FEATURE_INTERACTIVEWINDOW
    [Description("Node.js Tools - Interactive Window")]
#else
    [Description("Visual Studio Interactive Window")]
#endif
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideKeyBindingTable(ReplWindow.TypeGuid, 200)]        // Resource ID: "Interactive Console"
    [ProvideToolWindow(typeof(ReplWindow), Style = VsDockStyle.Linked, Orientation = ToolWindowOrientation.none, Window = ToolWindowGuids80.Outputwindow, MultiInstances = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.NoSolution)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionExists)]
    [Guid(Guids.guidReplWindowPkgString)]
    internal sealed class ReplWindowPackage : Package, IVsToolWindowFactory {
        int IVsToolWindowFactory.CreateToolWindow(ref Guid toolWindowType, uint id) {
            if (toolWindowType == typeof(ReplWindow).GUID) {
                var model = (IComponentModel)GetService(typeof(SComponentModel));
                var replProvider = (ReplWindowProvider)model.GetService<IReplWindowProvider>();

                return replProvider.CreateFromRegistry(model, (int)id) ? VSConstants.S_OK : VSConstants.E_FAIL;
            }

            return VSConstants.E_FAIL;
        }
    }
}
