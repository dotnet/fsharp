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
    class MockVsSolutionBuildManager : IVsSolutionBuildManager {
        public int AdviseUpdateSolutionEvents(IVsUpdateSolutionEvents pIVsUpdateSolutionEvents, out uint pdwCookie) {
            throw new NotImplementedException();
        }

        public int CanCancelUpdateSolutionConfiguration(out int pfCanCancel) {
            throw new NotImplementedException();
        }

        public int CancelUpdateSolutionConfiguration() {
            throw new NotImplementedException();
        }

        public int DebugLaunch(uint grfLaunch) {
            throw new NotImplementedException();
        }

        public int FindActiveProjectCfg(IntPtr pvReserved1, IntPtr pvReserved2, IVsHierarchy pIVsHierarchy_RequestedProject, IVsProjectCfg[] ppIVsProjectCfg_Active = null) {
            throw new NotImplementedException();
        }

        public int GetProjectDependencies(IVsHierarchy pHier, uint celt, IVsHierarchy[] rgpHier, uint[] pcActual = null) {
            throw new NotImplementedException();
        }

        public int QueryBuildManagerBusy(out int pfBuildManagerBusy) {
            pfBuildManagerBusy = 0;
            return VSConstants.S_OK;
        }

        public int QueryDebugLaunch(uint grfLaunch, out int pfCanLaunch) {
            throw new NotImplementedException();
        }

        public int StartSimpleUpdateProjectConfiguration(IVsHierarchy pIVsHierarchyToBuild, IVsHierarchy pIVsHierarchyDependent, string pszDependentConfigurationCanonicalName, uint dwFlags, uint dwDefQueryResults, int fSuppressUI) {
            throw new NotImplementedException();
        }

        public int StartSimpleUpdateSolutionConfiguration(uint dwFlags, uint dwDefQueryResults, int fSuppressUI) {
            throw new NotImplementedException();
        }

        public int UnadviseUpdateSolutionEvents(uint dwCookie) {
            throw new NotImplementedException();
        }

        public int UpdateSolutionConfigurationIsActive(out int pfIsActive) {
            throw new NotImplementedException();
        }

        public int get_CodePage(out uint puiCodePage) {
            throw new NotImplementedException();
        }

        public int get_IsDebug(out int pfIsDebug) {
            throw new NotImplementedException();
        }

        public int get_StartupProject(out IVsHierarchy ppHierarchy) {
            throw new NotImplementedException();
        }

        public int put_CodePage(uint uiCodePage) {
            throw new NotImplementedException();
        }

        public int put_IsDebug(int fIsDebug) {
            throw new NotImplementedException();
        }

        public int set_StartupProject(IVsHierarchy pHierarchy) {
            throw new NotImplementedException();
        }
    }
}
