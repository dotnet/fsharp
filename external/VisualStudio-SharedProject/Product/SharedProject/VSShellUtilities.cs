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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    ///This class provides some useful static shell based methods. 
    /// </summary>

    internal static class UIHierarchyUtilities {
        /// <summary>
        /// Get reference to IVsUIHierarchyWindow interface from guid persistence slot.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="persistenceSlot">Unique identifier for a tool window created using IVsUIShell::CreateToolWindow. 
        /// The caller of this method can use predefined identifiers that map to tool windows if those tool windows 
        /// are known to the caller. </param>
        /// <returns>A reference to an IVsUIHierarchyWindow interface.</returns>
        public static IVsUIHierarchyWindow GetUIHierarchyWindow(IServiceProvider serviceProvider, Guid persistenceSlot) {
            if (serviceProvider == null) {
                throw new ArgumentNullException("serviceProvider");
            }

            IVsUIShell shell = serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            if (shell == null) {
                throw new InvalidOperationException("Could not get the UI shell from the project");
            }

            object pvar;
            IVsWindowFrame frame;

            if (ErrorHandler.Succeeded(shell.FindToolWindow(0, ref persistenceSlot, out frame)) &&
                ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out pvar))) {
                return pvar as IVsUIHierarchyWindow;
            }

            return null;
        }
    }

    internal static class VsUtilities {
        internal static void NavigateTo(IServiceProvider serviceProvider, string filename, Guid docViewGuidType, int line, int col) {
            IVsTextView viewAdapter;
            IVsWindowFrame pWindowFrame;
            if (docViewGuidType != Guid.Empty) {
                OpenDocument(serviceProvider, filename, docViewGuidType, out viewAdapter, out pWindowFrame);
            } else {
                OpenDocument(serviceProvider, filename, out viewAdapter, out pWindowFrame);
            }

            ErrorHandler.ThrowOnFailure(pWindowFrame.Show());

            // Set the cursor at the beginning of the declaration.
            ErrorHandler.ThrowOnFailure(viewAdapter.SetCaretPos(line, col));
            // Make sure that the text is visible.
            viewAdapter.CenterLines(line, 1);
        }

        internal static void NavigateTo(IServiceProvider serviceProvider, string filename, Guid docViewGuidType, int pos) {
            IVsTextView viewAdapter;
            IVsWindowFrame pWindowFrame;
            if (docViewGuidType != Guid.Empty) {
                OpenDocument(serviceProvider, filename, docViewGuidType, out viewAdapter, out pWindowFrame);
            } else {
                OpenDocument(serviceProvider, filename, out viewAdapter, out pWindowFrame);
            }

            ErrorHandler.ThrowOnFailure(pWindowFrame.Show());
            int line, col;
            ErrorHandler.ThrowOnFailure(viewAdapter.GetLineAndColumn(pos, out line, out col));
            ErrorHandler.ThrowOnFailure(viewAdapter.SetCaretPos(line, col));
            // Make sure that the text is visible.
            viewAdapter.CenterLines(line, 1);
        }

        internal static void OpenDocument(IServiceProvider serviceProvider, string filename, out IVsTextView viewAdapter, out IVsWindowFrame pWindowFrame) {
            IVsTextManager textMgr = (IVsTextManager)serviceProvider.GetService(typeof(SVsTextManager));

            IVsUIHierarchy hierarchy;
            uint itemid;
            VsShellUtilities.OpenDocument(
                serviceProvider,
                filename,
                Guid.Empty,
                out hierarchy,
                out itemid,
                out pWindowFrame,
                out viewAdapter);
        }

        internal static void OpenDocument(IServiceProvider serviceProvider, string filename, Guid docViewGuid, out IVsTextView viewAdapter, out IVsWindowFrame pWindowFrame) {
            IVsUIHierarchy hierarchy;
            uint itemid;
            VsShellUtilities.OpenDocumentWithSpecificEditor(
                serviceProvider,
                filename,
                docViewGuid,
                Guid.Empty,
                out hierarchy,
                out itemid,
                out pWindowFrame
            );
            viewAdapter = VsShellUtilities.GetTextView(pWindowFrame);
        }
    }
}
