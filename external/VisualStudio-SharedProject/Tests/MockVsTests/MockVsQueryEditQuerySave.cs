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
    class MockVsQueryEditQuerySave : IVsQueryEditQuerySave2 {
        public int BeginQuerySaveBatch() {
            throw new NotImplementedException();
        }

        public int DeclareReloadableFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo) {
            throw new NotImplementedException();
        }

        public int DeclareUnreloadableFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo) {
            throw new NotImplementedException();
        }

        public int EndQuerySaveBatch() {
            throw new NotImplementedException();
        }

        public int IsReloadable(string pszMkDocument, out int pbResult) {
            throw new NotImplementedException();
        }

        public int OnAfterSaveUnreloadableFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo) {
            throw new NotImplementedException();
        }

        public int QueryEditFiles(uint rgfQueryEdit, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pfEditVerdict, out uint prgfMoreInfo) {
            pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
            prgfMoreInfo = 0;
            return VSConstants.S_OK;
        }

        public int QuerySaveFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo, out uint pdwQSResult) {
            throw new NotImplementedException();
        }

        public int QuerySaveFiles(uint rgfQuerySave, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pdwQSResult) {
            throw new NotImplementedException();
        }
    }
}
