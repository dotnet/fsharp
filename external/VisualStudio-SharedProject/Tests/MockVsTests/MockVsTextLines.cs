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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using TestUtilities.Mocks;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsTextLines : IVsTextLines, IVsPersistDocData, IVsExpansion {
        private readonly MockTextBuffer _buffer;
        private readonly IServiceProvider _serviceProvider;
        internal uint _docCookie;

        public MockVsTextLines(IServiceProvider serviceProvider, MockTextBuffer buffer) {
            _serviceProvider = serviceProvider;
            _buffer = buffer;
        }

        public int AdviseTextLinesEvents(IVsTextLinesEvents pSink, out uint pdwCookie) {
            throw new NotImplementedException();
        }

        public int CanReplaceLines(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, int iNewLen) {
            throw new NotImplementedException();
        }

        public int CopyLineText(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszBuf, ref int pcchBuf) {
            throw new NotImplementedException();
        }

        public int CreateEditPoint(int iLine, int iIndex, out object ppEditPoint) {
            throw new NotImplementedException();
        }

        public int CreateLineMarker(int iMarkerType, int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IVsTextMarkerClient pClient, IVsTextLineMarker[] ppMarker) {
            throw new NotImplementedException();
        }

        public int CreateTextPoint(int iLine, int iIndex, out object ppTextPoint) {
            throw new NotImplementedException();
        }

        public int EnumMarkers(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, int iMarkerType, uint dwFlags, out IVsEnumLineMarkers ppEnum) {
            throw new NotImplementedException();
        }

        public int FindMarkerByLineIndex(int iMarkerType, int iStartingLine, int iStartingIndex, uint dwFlags, out IVsTextLineMarker ppMarker) {
            throw new NotImplementedException();
        }

        public int GetLanguageServiceID(out Guid pguidLangService) {
            throw new NotImplementedException();
        }

        public int GetLastLineIndex(out int piLine, out int piIndex) {
            throw new NotImplementedException();
        }

        public int GetLengthOfLine(int iLine, out int piLength) {
            throw new NotImplementedException();
        }

        public int GetLineCount(out int piLineCount) {
            throw new NotImplementedException();
        }

        public int GetLineData(int iLine, LINEDATA[] pLineData, MARKERDATA[] pMarkerData) {
            throw new NotImplementedException();
        }

        public int GetLineDataEx(uint dwFlags, int iLine, int iStartIndex, int iEndIndex, LINEDATAEX[] pLineData, MARKERDATA[] pMarkerData) {
            throw new NotImplementedException();
        }

        public int GetLineIndexOfPosition(int iPosition, out int piLine, out int piColumn) {
            throw new NotImplementedException();
        }

        public int GetLineText(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, out string pbstrBuf) {
            throw new NotImplementedException();
        }

        public int GetMarkerData(int iTopLine, int iBottomLine, MARKERDATA[] pMarkerData) {
            throw new NotImplementedException();
        }

        public int GetPairExtents(TextSpan[] pSpanIn, TextSpan[] pSpanOut) {
            throw new NotImplementedException();
        }

        public int GetPositionOfLine(int iLine, out int piPosition) {
            throw new NotImplementedException();
        }

        public int GetPositionOfLineIndex(int iLine, int iIndex, out int piPosition) {
            throw new NotImplementedException();
        }

        public int GetSize(out int piLength) {
            throw new NotImplementedException();
        }

        public int GetStateFlags(out uint pdwReadOnlyFlags) {
            throw new NotImplementedException();
        }

        public int GetUndoManager(out VisualStudio.OLE.Interop.IOleUndoManager ppUndoManager) {
            throw new NotImplementedException();
        }

        public int IVsTextLinesReserved1(int iLine, LINEDATA[] pLineData, int fAttributes) {
            throw new NotImplementedException();
        }

        public int InitializeContent(string pszText, int iLength) {
            throw new NotImplementedException();
        }

        public int LockBuffer() {
            throw new NotImplementedException();
        }

        public int LockBufferEx(uint dwFlags) {
            throw new NotImplementedException();
        }

        public int ReleaseLineData(LINEDATA[] pLineData) {
            throw new NotImplementedException();
        }

        public int ReleaseLineDataEx(LINEDATAEX[] pLineData) {
            throw new NotImplementedException();
        }

        public int ReleaseMarkerData(MARKERDATA[] pMarkerData) {
            throw new NotImplementedException();
        }

        public int Reload(int fUndoable) {
            throw new NotImplementedException();
        }

        public int ReloadLines(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszText, int iNewLen, TextSpan[] pChangedSpan) {
            throw new NotImplementedException();
        }

        public int ReplaceLines(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszText, int iNewLen, TextSpan[] pChangedSpan) {
            throw new NotImplementedException();
        }

        public int ReplaceLinesEx(uint dwFlags, int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszText, int iNewLen, TextSpan[] pChangedSpan) {
            throw new NotImplementedException();
        }

        public int Reserved1() {
            throw new NotImplementedException();
        }

        public int Reserved10() {
            throw new NotImplementedException();
        }

        public int Reserved2() {
            throw new NotImplementedException();
        }

        public int Reserved3() {
            throw new NotImplementedException();
        }

        public int Reserved4() {
            throw new NotImplementedException();
        }

        public int Reserved5() {
            throw new NotImplementedException();
        }

        public int Reserved6() {
            throw new NotImplementedException();
        }

        public int Reserved7() {
            throw new NotImplementedException();
        }

        public int Reserved8() {
            throw new NotImplementedException();
        }

        public int Reserved9() {
            throw new NotImplementedException();
        }

        public int SetLanguageServiceID(ref Guid guidLangService) {
            throw new NotImplementedException();
        }

        public int SetStateFlags(uint dwReadOnlyFlags) {
            throw new NotImplementedException();
        }

        public int UnadviseTextLinesEvents(uint dwCookie) {
            throw new NotImplementedException();
        }

        public int UnlockBuffer() {
            throw new NotImplementedException();
        }

        public int UnlockBufferEx(uint dwFlags) {
            throw new NotImplementedException();
        }

        public int Close() {
            return VSConstants.S_OK;
        }

        public int GetGuidEditorType(out Guid pClassID) {
            throw new NotImplementedException();
        }

        public int IsDocDataDirty(out int pfDirty) {
            throw new NotImplementedException();
        }

        public int IsDocDataReloadable(out int pfReloadable) {
            throw new NotImplementedException();
        }

        public int LoadDocData(string pszMkDocument) {
            throw new NotImplementedException();
        }

        public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew) {
            _docCookie = docCookie;
            return VSConstants.S_OK;
        }

        public int ReloadDocData(uint grfFlags) {
            throw new NotImplementedException();
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) {
            var textDoc = _buffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));
            textDoc.Rename(pszMkDocumentNew);
            return VSConstants.S_OK;
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled) {

            throw new NotImplementedException();
        }

        public int SetUntitledDocPath(string pszDocDataPath) {
            throw new NotImplementedException();
        }

        public int InsertExpansion(TextSpan tsContext, TextSpan tsInsertPos, IVsExpansionClient pExpansionClient, Guid guidLang, out IVsExpansionSession pSession) {
            throw new NotImplementedException();
        }

        public int InsertNamedExpansion(string bstrTitle, string bstrPath, TextSpan tsInsertPos, IVsExpansionClient pExpansionClient, Guid guidLang, int fShowDisambiguationUI, out IVsExpansionSession pSession) {
            throw new NotImplementedException();
        }

        public int InsertSpecificExpansion(MSXML.IXMLDOMNode pSnippet, TextSpan tsInsertPos, IVsExpansionClient pExpansionClient, Guid guidLang, string pszRelativePath, out IVsExpansionSession pSession) {
            throw new NotImplementedException();
        }
    }
}
