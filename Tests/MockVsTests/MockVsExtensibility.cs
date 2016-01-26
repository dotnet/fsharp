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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsExtensibility : EnvDTE.IVsExtensibility, IVsExtensibility3 {
        [ThreadStatic]
        private static int _inAutomation;

        public void EnterAutomationFunction() {
            _inAutomation++;
        }

        public void ExitAutomationFunction() {
            _inAutomation--;
        }

        public void FireMacroReset() {
            throw new NotImplementedException();
        }

        public EnvDTE.ConfigurationManager GetConfigMgr(object pIVsProject, uint itemid) {
            throw new NotImplementedException();
        }

        public EnvDTE.Document GetDocumentFromDocCookie(int lDocCookie) {
            throw new NotImplementedException();
        }

        public EnvDTE.Globals GetGlobalsObject(object ExtractFrom) {
            throw new NotImplementedException();
        }

        public int GetLockCount() {
            throw new NotImplementedException();
        }

        public void GetSuppressUI(ref bool pOut) {
            throw new NotImplementedException();
        }

        public void GetUserControl(out bool fUserControl) {
            throw new NotImplementedException();
        }

        public EnvDTE.TextBuffer Get_TextBuffer(object pVsTextStream, EnvDTE.IExtensibleObjectSite pParent) {
            throw new NotImplementedException();
        }

        public int IsInAutomationFunction() {
            return _inAutomation;
        }

        public void IsMethodDisabled(ref Guid pGUID, int dispid) {
            throw new NotImplementedException();
        }

        public void LockServer(bool __MIDL_0010) {
            throw new NotImplementedException();
        }

        public EnvDTE.wizardResult RunWizardFile(string bstrWizFilename, int hwndOwner, ref object[] vContextParams) {
            throw new NotImplementedException();
        }

        public void SetSuppressUI(bool In) {
            throw new NotImplementedException();
        }

        public void SetUserControl(bool fUserControl) {
            throw new NotImplementedException();
        }

        public void SetUserControlUnlatched(bool fUserControl) {
            throw new NotImplementedException();
        }

        public bool TestForShutdown() {
            throw new NotImplementedException();
        }

        public void get_Properties(EnvDTE.ISupportVSProperties pParent, object pdispPropObj, out EnvDTE.Properties ppProperties) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.EnterAutomationFunction() {
            EnterAutomationFunction();
            return VSConstants.S_OK;
        }

        int IVsExtensibility3.ExitAutomationFunction() {
            ExitAutomationFunction();
            return VSConstants.S_OK;
        }

        public int FireCodeModelEvent3(int dispid, object pParent, object pElement, int changeKind) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.FireMacroReset() {
            throw new NotImplementedException();
        }

        public int FireProjectItemsEvent_ItemAdded(object ProjectItem) {
            throw new NotImplementedException();
        }

        public int FireProjectItemsEvent_ItemRemoved(object ProjectItem) {
            throw new NotImplementedException();
        }

        public int FireProjectItemsEvent_ItemRenamed(object ProjectItem, string OldName) {
            throw new NotImplementedException();
        }

        public int FireProjectsEvent_ItemAdded(object Project) {
            throw new NotImplementedException();
        }

        public int FireProjectsEvent_ItemRemoved(object Project) {
            throw new NotImplementedException();
        }

        public int FireProjectsEvent_ItemRenamed(object Project, string OldName) {
            throw new NotImplementedException();
        }

        public int GetConfigMgr(object pIVsProject, uint itemid, out object ppCfgMgr) {
            throw new NotImplementedException();
        }

        public int GetDocumentFromDocCookie(int lDocCookie, out object ppDoc) {
            throw new NotImplementedException();
        }

        public int GetGlobalsObject(object ExtractFrom, out object ppGlobals) {
            throw new NotImplementedException();
        }

        public int GetLockCount(out int pCount) {
            throw new NotImplementedException();
        }

        public int GetProperties(object pParent, object pdispPropObj, out object ppProperties) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.GetSuppressUI(ref bool pOut) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.GetUserControl(out bool fUserControl) {
            throw new NotImplementedException();
        }

        public int IsFireCodeModelEventNeeded(ref bool vbNeeded) {
            throw new NotImplementedException();
        }

        public int IsInAutomationFunction(out int pfInAutoFunc) {
            pfInAutoFunc = _inAutomation != 0 ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IVsExtensibility3.IsMethodDisabled(ref Guid pGuid, int dispid) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.LockServer(bool __MIDL_0010) {
            throw new NotImplementedException();
        }

        public int RunWizardFile(string bstrWizFilename, int hwndOwner, ref Array vContextParams, out int pResult) {
            throw new NotImplementedException();
        }

        public int RunWizardFileEx(string bstrWizFilename, int hwndOwner, ref Array vContextParams, ref Array vCustomParams, out int pResult) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.SetSuppressUI(bool In) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.SetUserControl(bool fUserControl) {
            throw new NotImplementedException();
        }

        int IVsExtensibility3.SetUserControlUnlatched(bool fUserControl) {
            throw new NotImplementedException();
        }

        public int TestForShutdown(out bool fShutdown) {
            throw new NotImplementedException();
        }
    }
}
