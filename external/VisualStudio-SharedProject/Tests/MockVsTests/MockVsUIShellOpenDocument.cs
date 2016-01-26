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
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsUIShellOpenDocument : IVsUIShellOpenDocument {
        public int AddStandardPreviewer(string pszExePath, string pszDisplayName, int fUseDDE, string pszDDEService, string pszDDETopicOpenURL, string pszDDEItemOpenURL, string pszDDETopicActivate, string pszDDEItemActivate, uint aspAddPreviewerFlags) {
            throw new NotImplementedException();
        }

        public int GetFirstDefaultPreviewer(out string pbstrDefBrowserPath, out int pfIsInternalBrowser, out int pfIsSystemBrowser) {
            throw new NotImplementedException();
        }

        public int GetStandardEditorFactory(uint dwReserved, ref Guid pguidEditorType, string pszMkDocument, ref Guid rguidLogicalView, out string pbstrPhysicalView, out IVsEditorFactory ppEF) {
            throw new NotImplementedException();
        }

        public int InitializeEditorInstance(uint grfIEI, IntPtr punkDocView, IntPtr punkDocData, string pszMkDocument, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, string pszOwnerCaption, string pszEditorCaption, IVsUIHierarchy pHier, uint itemid, IntPtr punkDocDataExisting, VisualStudio.OLE.Interop.IServiceProvider pSPHierContext, ref Guid rguidCmdUI, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int IsDocumentInAProject(string pszMkDocument, out IVsUIHierarchy ppUIH, out uint pitemid, out VisualStudio.OLE.Interop.IServiceProvider ppSP, out int pDocInProj) {
            throw new NotImplementedException();
        }

        public int IsDocumentOpen(IVsUIHierarchy pHierCaller, uint itemidCaller, string pszMkDocument, ref Guid rguidLogicalView, uint grfIDO, out IVsUIHierarchy ppHierOpen, uint[] pitemidOpen, out IVsWindowFrame ppWindowFrame, out int pfOpen) {
            throw new NotImplementedException();
        }

        public int IsSpecificDocumentViewOpen(IVsUIHierarchy pHierCaller, uint itemidCaller, string pszMkDocument, ref Guid rguidEditorType, string pszPhysicalView, uint grfIDO, out IVsUIHierarchy ppHierOpen, out uint pitemidOpen, out IVsWindowFrame ppWindowFrame, out int pfOpen) {
            throw new NotImplementedException();
        }

        public int MapLogicalView(ref Guid rguidEditorType, ref Guid rguidLogicalView, out string pbstrPhysicalView) {
            throw new NotImplementedException();
        }

        public int OpenCopyOfStandardEditor(IVsWindowFrame pWindowFrame, ref Guid rguidLogicalView, out IVsWindowFrame ppNewWindowFrame) {
            throw new NotImplementedException();
        }

        public int OpenDocumentViaProject(string pszMkDocument, ref Guid rguidLogicalView, out VisualStudio.OLE.Interop.IServiceProvider ppSP, out IVsUIHierarchy ppHier, out uint pitemid, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int OpenDocumentViaProjectWithSpecific(string pszMkDocument, uint grfEditorFlags, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, out VisualStudio.OLE.Interop.IServiceProvider ppSP, out IVsUIHierarchy ppHier, out uint pitemid, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int OpenSpecificEditor(uint grfOpenSpecific, string pszMkDocument, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, string pszOwnerCaption, IVsUIHierarchy pHier, uint itemid, IntPtr punkDocDataExisting, VisualStudio.OLE.Interop.IServiceProvider pSPHierContext, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int OpenStandardEditor(uint grfOpenStandard, string pszMkDocument, ref Guid rguidLogicalView, string pszOwnerCaption, IVsUIHierarchy pHier, uint itemid, IntPtr punkDocDataExisting, VisualStudio.OLE.Interop.IServiceProvider psp, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int OpenStandardPreviewer(uint ospOpenDocPreviewer, string pszURL, VSPREVIEWRESOLUTION resolution, uint dwReserved) {
            throw new NotImplementedException();
        }

        public int SearchProjectsForRelativePath(uint grfRPS, string pszRelPath, string[] pbstrAbsPath) {
            throw new NotImplementedException();
        }
    }
}
