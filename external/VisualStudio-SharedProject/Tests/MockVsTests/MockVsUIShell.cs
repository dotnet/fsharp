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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsUIShell : IVsUIShell {
        private readonly MockVs _instance;
        internal Stack<MockDialog> Dialogs = new Stack<MockDialog>();
        private Dictionary<Guid, MockToolWindow> _toolWindows = new Dictionary<Guid, MockToolWindow>();
        private const int _waitLoops = 500;
        private const int _waitTimeout = 10;

        public MockVsUIShell(MockVs instance) {
            _instance = instance;
        }

        public int AddNewBFNavigationItem(IVsWindowFrame pWindowFrame, string bstrData, object punk, int fReplaceCurrent) {
            throw new NotImplementedException();
        }

        public int CenterDialogOnWindow(IntPtr hwndDialog, IntPtr hwndParent) {
            throw new NotImplementedException();
        }

        public int CreateDocumentWindow(uint grfCDW, string pszMkDocument, IVsUIHierarchy pUIH, uint itemid, IntPtr punkDocView, IntPtr punkDocData, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidCmdUI, VisualStudio.OLE.Interop.IServiceProvider psp, string pszOwnerCaption, string pszEditorCaption, int[] pfDefaultPosition, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int CreateToolWindow(uint grfCTW, uint dwToolWindowId, object punkTool, ref Guid rclsidTool, ref Guid rguidPersistenceSlot, ref Guid rguidAutoActivate, VisualStudio.OLE.Interop.IServiceProvider psp, string pszCaption, int[] pfDefaultPosition, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int EnableModeless(int fEnable) {
            throw new NotImplementedException();
        }

        public int FindToolWindow(uint grfFTW, ref Guid rguidPersistenceSlot, out IVsWindowFrame ppWindowFrame) {
            MockToolWindow toolWindow;
            if (_toolWindows.TryGetValue(rguidPersistenceSlot, out toolWindow)) {
                ppWindowFrame = toolWindow;
                return VSConstants.S_OK;
            }
            ppWindowFrame = null;
            return VSConstants.E_FAIL;
        }

        public int FindToolWindowEx(uint grfFTW, ref Guid rguidPersistenceSlot, uint dwToolWinId, out IVsWindowFrame ppWindowFrame) {
            throw new NotImplementedException();
        }

        public int GetAppName(out string pbstrAppName) {
            throw new NotImplementedException();
        }

        public int GetCurrentBFNavigationItem(out IVsWindowFrame ppWindowFrame, out string pbstrData, out object ppunk) {
            throw new NotImplementedException();
        }

        public int GetDialogOwnerHwnd(out IntPtr phwnd) {
            throw new NotImplementedException();
        }

        public int GetDirectoryViaBrowseDlg(VSBROWSEINFOW[] pBrowse) {
            throw new NotImplementedException();
        }

        public int GetDocumentWindowEnum(out IEnumWindowFrames ppenum) {
            ppenum = new EnumWindowFrames(this);
            return VSConstants.S_OK;
        }

        class EnumWindowFrames : IEnumWindowFrames {
            private MockVsUIShell _uiShell;

            public EnumWindowFrames(MockVsUIShell mockVsUIShell) {
                _uiShell = mockVsUIShell;
            }

            public int Clone(out IEnumWindowFrames ppenum) {
                throw new NotImplementedException();
            }

            public int Next(uint celt, IVsWindowFrame[] rgelt, out uint pceltFetched) {
                pceltFetched = 0;
                return VSConstants.E_FAIL;
            }

            public int Reset() {
                throw new NotImplementedException();
            }

            public int Skip(uint celt) {
                throw new NotImplementedException();
            }
        }

        public int GetErrorInfo(out string pbstrErrText) {
            throw new NotImplementedException();
        }

        public int GetNextBFNavigationItem(out IVsWindowFrame ppWindowFrame, out string pbstrData, out object ppunk) {
            throw new NotImplementedException();
        }

        public int GetOpenFileNameViaDlg(VSOPENFILENAMEW[] pOpenFileName) {
            throw new NotImplementedException();
        }

        public int GetPreviousBFNavigationItem(out IVsWindowFrame ppWindowFrame, out string pbstrData, out object ppunk) {
            throw new NotImplementedException();
        }

        public int GetSaveFileNameViaDlg(VSSAVEFILENAMEW[] pSaveFileName) {
            throw new NotImplementedException();
        }

        public int GetToolWindowEnum(out IEnumWindowFrames ppenum) {
            throw new NotImplementedException();
        }

        public int GetURLViaDlg(string pszDlgTitle, string pszStaticLabel, string pszHelpTopic, out string pbstrURL) {
            throw new NotImplementedException();
        }

        public int GetVSSysColor(VSSYSCOLOR dwSysColIndex, out uint pdwRGBval) {
            throw new NotImplementedException();
        }

        public int OnModeChange(DBGMODE dbgmodeNew) {
            throw new NotImplementedException();
        }

        public int PostExecCommand(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, ref object pvaIn) {
            throw new NotImplementedException();
        }

        public int PostSetFocusMenuCommand(ref Guid pguidCmdGroup, uint nCmdID) {
            throw new NotImplementedException();
        }

        public int RefreshPropertyBrowser(int dispid) {
            return VSConstants.S_OK;
        }

        public int RemoveAdjacentBFNavigationItem(RemoveBFDirection rdDir) {
            throw new NotImplementedException();
        }

        public int RemoveCurrentNavigationDupes(RemoveBFDirection rdDir) {
            throw new NotImplementedException();
        }

        public int ReportErrorInfo(int hr) {
            throw new NotImplementedException();
        }

        public int SaveDocDataToFile(VSSAVEFLAGS grfSave, object pPersistFile, string pszUntitledPath, out string pbstrDocumentNew, out int pfCanceled) {
            throw new NotImplementedException();
        }

        public int SetErrorInfo(int hr, string pszDescription, uint dwReserved, string pszHelpKeyword, string pszSource) {
            throw new NotImplementedException();
        }

        public int SetForegroundWindow() {
            throw new NotImplementedException();
        }

        public int SetMRUComboText(ref Guid pguidCmdGroup, uint dwCmdID, string lpszText, int fAddToList) {
            throw new NotImplementedException();
        }

        public int SetMRUComboTextW(Guid[] pguidCmdGroup, uint dwCmdID, string pwszText, int fAddToList) {
            throw new NotImplementedException();
        }

        public int SetToolbarVisibleInFullScreen(Guid[] pguidCmdGroup, uint dwToolbarId, int fVisibleInFullScreen) {
            throw new NotImplementedException();
        }

        public int SetWaitCursor() {
            throw new NotImplementedException();
        }

        public int SetupToolbar(IntPtr hwnd, IVsToolWindowToolbar ptwt, out IVsToolWindowToolbarHost pptwth) {
            throw new NotImplementedException();
        }

        public int ShowContextMenu(uint dwCompRole, ref Guid rclsidActive, int nMenuId, POINTS[] pos, VisualStudio.OLE.Interop.IOleCommandTarget pCmdTrgtActive) {
            throw new NotImplementedException();
        }

        public int ShowMessageBox(uint dwCompRole, ref Guid rclsidComp, string pszTitle, string pszText, string pszHelpFile, uint dwHelpContextID, OLEMSGBUTTON msgbtn, OLEMSGDEFBUTTON msgdefbtn, OLEMSGICON msgicon, int fSysAlert, out int pnResult) {
            pnResult = (int)_instance.Invoke(
                () => {
                    MockDialog dialog = new MockMessageBox(_instance, pszTitle, pszText);
                    lock (Dialogs) {
                        Dialogs.Push(dialog);
                    }
                    dialog.Run();
                    lock (Dialogs) {
                        Dialogs.Pop();
                    }
                    return dialog.DialogResult;
                }
            );
            return VSConstants.S_OK;
        }

        public int TranslateAcceleratorAsACmd(VisualStudio.OLE.Interop.MSG[] pMsg) {
            throw new NotImplementedException();
        }

        public int UpdateCommandUI(int fImmediateUpdate) {
            throw new NotImplementedException();
        }

        public int UpdateDocDataIsDirtyFeedback(uint docCookie, int fDirty) {
            throw new NotImplementedException();
        }

        internal void CheckMessageBox(MessageBoxButton button, string[] text) {
            for (int i = 0; i < _waitLoops; i++) {
                MockMessageBox msgBox;
                if ((msgBox = LastDialog<MockMessageBox>()) != null) {
                    AssertUtil.Contains(msgBox.Text, text);
                    msgBox.Close((int)button);
                    return;
                }
                Thread.Sleep(_waitTimeout);
            }
            Assert.Fail("Failed to get message box");
        }

        private T LastDialog<T>() where T : MockDialog {
            lock (Dialogs) {
                if (Dialogs.Count == 0) {
                    return null;
                }
                return Dialogs.Last() as T;
            }
        }

        public void AddToolWindow(Guid id, MockToolWindow toolWindow) {
            _toolWindows[id] = toolWindow;
        }

        internal void WaitForDialogDismissed() {
            for (int i = 0; i < _waitLoops; i++) {
                if (Dialogs.Count == 0) {
                    return;
                }
                Thread.Sleep(_waitTimeout);
            }
            Assert.Fail("Dialog was not dismissed");
        }

        internal IntPtr WaitForDialog() {
            for (int i = 0; i < _waitLoops; i++) {
                if (Dialogs.Count != 0) {
                    return new IntPtr(IntPtr.Size * Dialogs.Count);
                }
                Thread.Sleep(_waitTimeout);
            }
            Assert.Fail("Dialog not created");
            throw new InvalidOperationException(); // not reachable
        }
    }
}
