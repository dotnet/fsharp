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
using EnvDTE;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsProfferCommands : IVsProfferCommands {
        public void AddCommandBar(string pszCmdBarName, vsCommandBarType dwType, object pCmdBarParent, uint dwIndex, out object ppCmdBar) {
            ppCmdBar = null;
        }

        public void AddCommandBarControl(string pszCmdNameCanonical, object pCmdBarParent, uint dwIndex, uint dwCmdType, out object ppCmdBarCtrl) {
            ppCmdBarCtrl = null;
        }

        public void AddNamedCommand(ref Guid pguidPackage, ref Guid pguidCmdGroup, string pszCmdNameCanonical, out uint pdwCmdId, string pszCmdNameLocalized, string pszBtnText, string pszCmdTooltip, string pszSatelliteDLL, uint dwBitmapResourceId, uint dwBitmapImageIndex, uint dwCmdFlagsDefault, uint cUIContexts, ref Guid rgguidUIContexts) {
            pdwCmdId = 0;
        }

        public object FindCommandBar(IntPtr pToolbarSet, ref Guid pguidCmdGroup, uint dwMenuId) {
            throw new NotImplementedException();
        }

        public void RemoveCommandBar(object pCmdBar) {
            throw new NotImplementedException();
        }

        public void RemoveCommandBarControl(object pCmdBarCtrl) {
            throw new NotImplementedException();
        }

        public void RemoveNamedCommand(string pszCmdNameCanonical) {
            throw new NotImplementedException();
        }

        public void RenameNamedCommand(string pszCmdNameCanonical, string pszCmdNameCanonicalNew, string pszCmdNameLocalizedNew) {
            throw new NotImplementedException();
        }
    }
}
