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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsTrackProjectDocuments : IVsTrackProjectDocuments2 {
        private Dictionary<uint, IVsTrackProjectDocumentsEvents2> _events = new Dictionary<uint, IVsTrackProjectDocumentsEvents2>();
        private uint _curCookie;

        public int AdviseTrackProjectDocumentsEvents(IVsTrackProjectDocumentsEvents2 pEventSink, out uint pdwCookie) {
            _events[++_curCookie] = pEventSink;
            pdwCookie = _curCookie;
            return VSConstants.S_OK;
        }

        public int BeginBatch() {
            throw new NotImplementedException();
        }

        public int EndBatch() {
            throw new NotImplementedException();
        }

        public int Flush() {
            throw new NotImplementedException();
        }

        public int OnAfterAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments) {
            return VSConstants.S_OK;
        }

        public int OnAfterAddDirectoriesEx(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags) {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments) {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags) {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags) {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags) {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags) {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFile(IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags) {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags) {
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, uint[] rgdwSccStatus) {
            throw new NotImplementedException();
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults) {
            throw new NotImplementedException();
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults) {
            if (rgResults != null) {
                for (int i = 0; i < cFiles; i++) {
                    rgResults[i] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
                }
            }
            if (pSummaryResult != null) {
                pSummaryResult[0] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
            }
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults) {
            throw new NotImplementedException();
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults) {
            if (pSummaryResult != null) {
                pSummaryResult[0] = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK;
            }
            if (rgResults != null) {
                for (int i = 0; i < cFiles; i++) {
                    rgResults[i] = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK;
                }
            }
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults) {
            throw new NotImplementedException();
        }

        public int OnQueryRenameFile(IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags, out int pfRenameCanContinue) {
            pfRenameCanContinue = 1;
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults) {
            throw new NotImplementedException();
        }

        public int UnadviseTrackProjectDocumentsEvents(uint dwCookie) {
            return VSConstants.S_OK;
        }
    }
}
