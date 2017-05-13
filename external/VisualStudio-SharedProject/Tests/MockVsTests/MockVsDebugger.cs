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
    class MockVsDebugger : IVsDebugger {
        public int AdviseDebugEventCallback(object punkDebuggerEvents) {
            throw new NotImplementedException();
        }

        public int AdviseDebuggerEvents(IVsDebuggerEvents pSink, out uint pdwCookie) {
            throw new NotImplementedException();
        }

        public int AllowEditsWhileDebugging(ref Guid guidLanguageService) {
            throw new NotImplementedException();
        }

        public int ExecCmdForTextPos(VsTextPos[] pTextPos, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            throw new NotImplementedException();
        }

        public int GetDataTipValue(VisualStudio.TextManager.Interop.IVsTextLines pTextBuf, VisualStudio.TextManager.Interop.TextSpan[] pTS, string pszExpression, out string pbstrValue) {
            throw new NotImplementedException();
        }

        public int GetENCUpdate(out object ppUpdate) {
            throw new NotImplementedException();
        }

        public int GetMode(DBGMODE[] pdbgmode) {
            pdbgmode[0] = DBGMODE.DBGMODE_Design;
            return VSConstants.S_OK;
        }

        public int InsertBreakpointByName(ref Guid guidLanguage, string pszCodeLocationText) {
            throw new NotImplementedException();
        }

        public int IsBreakpointOnName(ref Guid guidLanguage, string pszCodeLocationText, out int pfIsBreakpoint) {
            throw new NotImplementedException();
        }

        public int LaunchDebugTargets(uint cTargets, IntPtr rgDebugTargetInfo) {
            throw new NotImplementedException();
        }

        public int ParseFileRedirection(string pszArgs, out string pbstrArgsProcessed, out IntPtr phStdInput, out IntPtr phStdOutput, out IntPtr phStdError) {
            throw new NotImplementedException();
        }

        public int QueryStatusForTextPos(VsTextPos[] pTextPos, ref Guid pguidCmdGroup, uint cCmds, VisualStudio.OLE.Interop.OLECMD[] prgCmds, IntPtr pCmdText) {
            throw new NotImplementedException();
        }

        public int RemoveBreakpointsByName(ref Guid guidLanguage, string pszCodeLocationText) {
            throw new NotImplementedException();
        }

        public int ToggleBreakpointByName(ref Guid guidLanguage, string pszCodeLocationText) {
            throw new NotImplementedException();
        }

        public int UnadviseDebugEventCallback(object punkDebuggerEvents) {
            throw new NotImplementedException();
        }

        public int UnadviseDebuggerEvents(uint dwCookie) {
            throw new NotImplementedException();
        }
    }
}
