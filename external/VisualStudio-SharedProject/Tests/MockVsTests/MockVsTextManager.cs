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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsTextManager : IVsTextManager, IVsTextManager2, IVsExpansionManager, IConnectionPointContainer {
        private readonly MockVs _vs;
        private Dictionary<Guid, LANGPREFERENCES2> _langPrefs = new Dictionary<Guid, LANGPREFERENCES2>();
        private ConnectionPoint _ppCP;

        public MockVsTextManager(MockVs vs) {
            _vs = vs;
            foreach (var langService in MockVs.CachedInfo.LangServicesByGuid) {
                var type = langService.Value.Attribute;

                _langPrefs[langService.Key] = new LANGPREFERENCES2() {
                    guidLang = langService.Key,
                    fDropdownBar = (uint)(type.ShowDropDownOptions ? 1 : 0),
                    fInsertTabs = (uint)(type.DefaultToInsertSpaces ? 0 : 1),
                    fShowCompletion = (uint)(type.ShowCompletion ? 1 : 0),
                    fShowSmartIndent = (uint)(type.ShowSmartIndent ? 1 : 0),
                    fHideAdvancedAutoListMembers = (uint)(type.HideAdvancedMembersByDefault ? 1 : 0),
                    fAutoListMembers = 1,
                    fAutoListParams = 1,
                    IndentStyle = type.ShowSmartIndent ? vsIndentStyle.vsIndentStyleSmart : vsIndentStyle.vsIndentStyleDefault
                };
            }
        }

        private static LANGPREFERENCES2 FromLangPrefs(LANGPREFERENCES langPrefs) {
            return new LANGPREFERENCES2 {
                fAutoListMembers = langPrefs.fAutoListMembers,
                fAutoListParams = langPrefs.fAutoListParams,
                fDropdownBar = langPrefs.fDropdownBar,
                fHideAdvancedAutoListMembers = langPrefs.fHideAdvancedAutoListMembers,
                fHotURLs = langPrefs.fHotURLs,
                fInsertTabs = langPrefs.fInsertTabs,
                fLineNumbers = langPrefs.fLineNumbers,
                fShowCompletion = langPrefs.fShowCompletion,
                fShowSmartIndent = langPrefs.fShowSmartIndent,
                fTwoWayTreeview = langPrefs.fTwoWayTreeview,
                fVirtualSpace = langPrefs.fVirtualSpace,
                fWordWrap = langPrefs.fWordWrap,
                IndentStyle = langPrefs.IndentStyle,
                guidLang = langPrefs.guidLang,
                szFileType = langPrefs.szFileType,
                uIndentSize = langPrefs.uIndentSize,
                uTabSize = langPrefs.uTabSize
            };
        }

        private static LANGPREFERENCES FromLangPrefs2(LANGPREFERENCES2 langPrefs) {
            return new LANGPREFERENCES {
                fAutoListMembers = langPrefs.fAutoListMembers,
                fAutoListParams = langPrefs.fAutoListParams,
                fDropdownBar = langPrefs.fDropdownBar,
                fHideAdvancedAutoListMembers = langPrefs.fHideAdvancedAutoListMembers,
                fHotURLs = langPrefs.fHotURLs,
                fInsertTabs = langPrefs.fInsertTabs,
                fLineNumbers = langPrefs.fLineNumbers,
                fShowCompletion = langPrefs.fShowCompletion,
                fShowSmartIndent = langPrefs.fShowSmartIndent,
                fTwoWayTreeview = langPrefs.fTwoWayTreeview,
                fVirtualSpace = langPrefs.fVirtualSpace,
                fWordWrap = langPrefs.fWordWrap,
                IndentStyle = langPrefs.IndentStyle,
                guidLang = langPrefs.guidLang,
                szFileType = langPrefs.szFileType,
                uIndentSize = langPrefs.uIndentSize,
                uTabSize = langPrefs.uTabSize
            };
        }

        public int AttemptToCheckOutBufferFromScc3(IVsTextBuffer pBuffer, string pszFileName, uint dwQueryEditFlags, out int pbCheckoutSucceeded, out int piStatusFlags) {
            throw new NotImplementedException();
        }

        public int FireReplaceAllInFilesBegin() {
            throw new NotImplementedException();
        }

        public int FireReplaceAllInFilesEnd() {
            throw new NotImplementedException();
        }

        public int GetActiveView2(int fMustHaveFocus, IVsTextBuffer pBuffer, uint grfIncludeViewFrameType, out IVsTextView ppView) {
            throw new NotImplementedException();
        }

        public int GetBufferSccStatus3(IVsTextBuffer pBuffer, string pszFileName, out int pbCheckoutSucceeded, out int piStatusFlags) {
            throw new NotImplementedException();
        }

        public int GetExpansionManager(out IVsExpansionManager pExpansionManager) {
            pExpansionManager = this;
            return VSConstants.S_OK;
        }

        public int GetUserPreferences2(VIEWPREFERENCES2[] pViewPrefs, FRAMEPREFERENCES2[] pFramePrefs, LANGPREFERENCES2[] pLangPrefs, FONTCOLORPREFERENCES2[] pColorPrefs) {
            if (pViewPrefs != null || pFramePrefs != null || pColorPrefs != null) {
                throw new NotImplementedException();
            }
            LANGPREFERENCES2 langPrefs;
            if (_langPrefs.TryGetValue(pLangPrefs[0].guidLang, out langPrefs)) {
                pLangPrefs[0] = langPrefs;
                return VSConstants.S_OK;
            }
            return VSConstants.E_FAIL;
        }

        public int NavigateToLineAndColumn2(IVsTextBuffer pBuffer, ref Guid guidDocViewType, int iStartRow, int iStartIndex, int iEndRow, int iEndIndex, uint grfIncludeViewFrameType) {
            throw new NotImplementedException();
        }

        public int NavigateToPosition2(IVsTextBuffer pBuffer, ref Guid guidDocViewType, int iPos, int iLen, uint grfIncludeViewFrameType) {
            throw new NotImplementedException();
        }

        public int ResetColorableItems(Guid guidLang) {
            throw new NotImplementedException();
        }

        public int SetUserPreferences2(VIEWPREFERENCES2[] pViewPrefs, FRAMEPREFERENCES2[] pFramePrefs, LANGPREFERENCES2[] pLangPrefs, FONTCOLORPREFERENCES2[] pColorPrefs) {
            if (pViewPrefs != null || pFramePrefs != null || pColorPrefs != null) {
                throw new NotImplementedException();
            }

            foreach(var langPrefs in pLangPrefs) {
                _langPrefs[langPrefs.guidLang] = langPrefs;
            }

            if (_ppCP != null) {
                foreach (var sink in _ppCP.GetSinks<IVsTextManagerEvents>()) {
                    sink.OnUserPreferencesChanged(null, null, pLangPrefs.Select(FromLangPrefs2).ToArray(), null);
                }
                foreach (var sink in _ppCP.GetSinks<IVsTextManagerEvents2>()) {
                    sink.OnUserPreferencesChanged2(null, null, pLangPrefs, null);
                }
            }
            return VSConstants.S_OK;
        }

        public int EnumerateExpansions(Guid guidLang, int fShortCutOnly, string[] bstrTypes, int iCountTypes, int fIncludeNULLType, int fIncludeDuplicates, out IVsExpansionEnumeration pEnum) {
            throw new NotImplementedException();
        }

        public int GetExpansionByShortcut(IVsExpansionClient pClient, Guid guidLang, string szShortcut, IVsTextView pView, TextSpan[] pts, int fShowUI, out string pszExpansionPath, out string pszTitle) {
            throw new NotImplementedException();
        }

        public int GetSnippetShortCutKeybindingState(out int fBound) {
            throw new NotImplementedException();
        }

        public int GetTokenPath(uint token, out string pbstrPath) {
            throw new NotImplementedException();
        }

        public int InvokeInsertionUI(IVsTextView pView, IVsExpansionClient pClient, Guid guidLang, string[] bstrTypes, int iCountTypes, int fIncludeNULLType, string[] bstrKinds, int iCountKinds, int fIncludeNULLKind, string bstrPrefixText, string bstrCompletionChar) {
            throw new NotImplementedException();
        }

        public int AdjustFileChangeIgnoreCount(IVsTextBuffer pBuffer, int fIgnore) {
            throw new NotImplementedException();
        }

        public int AttemptToCheckOutBufferFromScc(IVsUserData pBufData, out int pfCheckoutSucceeded) {
            throw new NotImplementedException();
        }

        public int AttemptToCheckOutBufferFromScc2(string pszFileName, out int pfCheckoutSucceeded, out int piStatusFlags) {
            throw new NotImplementedException();
        }

        public int CreateSelectionAction(IVsTextBuffer pBuffer, out IVsTextSelectionAction ppAction) {
            throw new NotImplementedException();
        }

        public int EnumBuffers(out IVsEnumTextBuffers ppEnum) {
            throw new NotImplementedException();
        }

        public int EnumIndependentViews(IVsTextBuffer pBuffer, out IVsEnumIndependentViews ppEnum) {
            throw new NotImplementedException();
        }

        public int EnumLanguageServices(out IVsEnumGUID ppEnum) {
            throw new NotImplementedException();
        }

        public int EnumViews(IVsTextBuffer pBuffer, out IVsEnumTextViews ppEnum) {
            throw new NotImplementedException();
        }

        public int GetActiveView(int fMustHaveFocus, IVsTextBuffer pBuffer, out IVsTextView ppView) {
            throw new NotImplementedException();
        }

        public int GetBufferSccStatus(IVsUserData pBufData, out int pbNonEditable) {
            throw new NotImplementedException();
        }

        public int GetBufferSccStatus2(string pszFileName, out int pbNonEditable, out int piStatusFlags) {
            throw new NotImplementedException();
        }

        public int GetMarkerTypeCount(out int piMarkerTypeCount) {
            throw new NotImplementedException();
        }

        public int GetMarkerTypeInterface(int iMarkerTypeID, out IVsTextMarkerType ppMarkerType) {
            throw new NotImplementedException();
        }

        public int GetPerLanguagePreferences(LANGPREFERENCES[] pLangPrefs) {
            throw new NotImplementedException();
        }

        public int GetRegisteredMarkerTypeID(ref Guid pguidMarker, out int piMarkerTypeID) {
            throw new NotImplementedException();
        }

        public int GetShortcutManager(out IVsShortcutManager ppShortcutMgr) {
            throw new NotImplementedException();
        }

        public int GetUserPreferences(VIEWPREFERENCES[] pViewPrefs, FRAMEPREFERENCES[] pFramePrefs, LANGPREFERENCES[] pLangPrefs, FONTCOLORPREFERENCES[] pColorPrefs) {
            if (pViewPrefs != null || pFramePrefs != null || pColorPrefs != null) {
                throw new NotImplementedException();
            }
            LANGPREFERENCES2 langPrefs;
            if (_langPrefs.TryGetValue(pLangPrefs[0].guidLang, out langPrefs)) {
                pLangPrefs[0] = new LANGPREFERENCES() {
                    fAutoListMembers = langPrefs.fAutoListMembers,
                    fDropdownBar = langPrefs.fAutoListParams,
                    fAutoListParams = langPrefs.fAutoListParams,
                    fHideAdvancedAutoListMembers = langPrefs.fHideAdvancedAutoListMembers,
                    fHotURLs = langPrefs.fHotURLs,
                    fInsertTabs = langPrefs.fInsertTabs,
                    fLineNumbers = langPrefs.fLineNumbers,
                    fShowCompletion = langPrefs.fShowCompletion,
                    fShowSmartIndent = langPrefs.fShowSmartIndent,
                    fTwoWayTreeview = langPrefs.fTwoWayTreeview,
                    fVirtualSpace = langPrefs.fVirtualSpace,
                    fWordWrap = langPrefs.fWordWrap,
                    guidLang = langPrefs.guidLang,
                    IndentStyle = langPrefs.IndentStyle,
                    szFileType = langPrefs.szFileType,
                    uIndentSize = langPrefs.uIndentSize,
                    uTabSize = langPrefs.uTabSize
                };
                return VSConstants.S_OK;
            }
            return VSConstants.E_FAIL;

        }

        public int IgnoreNextFileChange(IVsTextBuffer pBuffer) {
            throw new NotImplementedException();
        }

        public int MapFilenameToLanguageSID(string pszFileName, out Guid pguidLangSID) {
            throw new NotImplementedException();
        }

        public int NavigateToLineAndColumn(IVsTextBuffer pBuffer, ref Guid guidDocViewType, int iStartRow, int iStartIndex, int iEndRow, int iEndIndex) {
            throw new NotImplementedException();
        }

        public int NavigateToPosition(IVsTextBuffer pBuffer, ref Guid guidDocViewType, int iPos, int iLen) {
            throw new NotImplementedException();
        }

        public int RegisterBuffer(IVsTextBuffer pBuffer) {
            throw new NotImplementedException();
        }

        public int RegisterIndependentView(object pUnk, IVsTextBuffer pBuffer) {
            throw new NotImplementedException();
        }

        public int RegisterView(IVsTextView pView, IVsTextBuffer pBuffer) {
            throw new NotImplementedException();
        }

        public int SetFileChangeAdvise(string pszFileName, int fStart) {
            throw new NotImplementedException();
        }

        public int SetPerLanguagePreferences(LANGPREFERENCES[] pLangPrefs) {
            throw new NotImplementedException();
        }

        public int SetUserPreferences(VIEWPREFERENCES[] pViewPrefs, FRAMEPREFERENCES[] pFramePrefs, LANGPREFERENCES[] pLangPrefs, FONTCOLORPREFERENCES[] pColorPrefs) {
            if (pViewPrefs != null || pFramePrefs != null || pColorPrefs != null) {
                throw new NotImplementedException();
            }
            foreach (var langPrefs in pLangPrefs) {
                _langPrefs[langPrefs.guidLang] = FromLangPrefs(langPrefs);
            }

            if (_ppCP != null) {
                foreach (var sink in _ppCP.GetSinks<IVsTextManagerEvents>()) {
                    sink.OnUserPreferencesChanged(null, null, pLangPrefs, null);
                }
                foreach (var sink in _ppCP.GetSinks<IVsTextManagerEvents2>()) {
                    sink.OnUserPreferencesChanged2(null, null, pLangPrefs.Select(FromLangPrefs).ToArray(), null);
                }
            }

            return VSConstants.S_OK;
        }

        public int SuspendFileChangeAdvise(string pszFileName, int fSuspend) {
            throw new NotImplementedException();
        }

        public int UnregisterBuffer(IVsTextBuffer pBuffer) {
            throw new NotImplementedException();
        }

        public int UnregisterIndependentView(object pUnk, IVsTextBuffer pBuffer) {
            throw new NotImplementedException();
        }

        public int UnregisterView(IVsTextView pView) {
            throw new NotImplementedException();
        }

        public void EnumConnectionPoints(out IEnumConnectionPoints ppEnum) {
            throw new NotImplementedException();
        }

        public void FindConnectionPoint(ref Guid riid, out IConnectionPoint ppCP) {
            if (_ppCP == null) {
                _ppCP = new ConnectionPoint();
            }
            ppCP = _ppCP;
        }

        class ConnectionPoint : IConnectionPoint {
            private readonly List<object> _sinks = new List<object>();

            public void Advise(object pUnkSink, out uint pdwCookie) {
                _sinks.Add(pUnkSink);
                pdwCookie = (uint)_sinks.Count;
            }

            public IEnumerable<T> GetSinks<T>() {
                return _sinks.OfType<T>();
            }

            public void EnumConnections(out IEnumConnections ppEnum) {
                throw new NotImplementedException();
            }

            public void GetConnectionInterface(out Guid pIID) {
                throw new NotImplementedException();
            }

            public void GetConnectionPointContainer(out IConnectionPointContainer ppCPC) {
                throw new NotImplementedException();
            }

            public void Unadvise(uint dwCookie) {
            }
        }
    }
}
