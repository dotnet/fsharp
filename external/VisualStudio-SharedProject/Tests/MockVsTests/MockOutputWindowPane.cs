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
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockOutputWindowPane : IVsOutputWindowPane {
        private string _name;
        private readonly StringBuilder _content = new StringBuilder();

        public MockOutputWindowPane(string pszPaneName) {
            _name = pszPaneName;
        }

        public int Activate() {
            return VSConstants.S_OK;
        }

        public int Clear() {
            _content.Clear();
            return VSConstants.S_OK;
        }

        public int FlushToTaskList() {
            throw new NotImplementedException();
        }

        public int GetName(ref string pbstrPaneName) {
            pbstrPaneName = _name;
            return VSConstants.S_OK;
        }

        public int Hide() {
            return VSConstants.S_OK;
        }

        public int OutputString(string pszOutputString) {
            lock (this) {
                _content.Append(pszOutputString);
            }
            return VSConstants.S_OK;
        }

        public int OutputStringThreadSafe(string pszOutputString) {
            lock (this) {
                _content.Append(pszOutputString);
            }
            return VSConstants.S_OK;
        }

        public int OutputTaskItemString(string pszOutputString, VSTASKPRIORITY nPriority, VSTASKCATEGORY nCategory, string pszSubcategory, int nBitmap, string pszFilename, uint nLineNum, string pszTaskItemText) {
            throw new NotImplementedException();
        }

        public int OutputTaskItemStringEx(string pszOutputString, VSTASKPRIORITY nPriority, VSTASKCATEGORY nCategory, string pszSubcategory, int nBitmap, string pszFilename, uint nLineNum, string pszTaskItemText, string pszLookupKwd) {
            throw new NotImplementedException();
        }

        public int SetName(string pszPaneName) {
            _name = pszPaneName;
            return VSConstants.S_OK;
        }
    }
}
