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
using System.Collections;
using EnvDTE;
using Microsoft.VisualStudio;

namespace Microsoft.VisualStudioTools.MockVsTests {
    internal class MockDTESolution : Solution {
        private readonly MockDTE _dte;

        public MockDTESolution(MockDTE dte) {
            _dte = dte;
        }

        public AddIns AddIns {
            get {
                throw new NotImplementedException();
            }
        }

        public int Count {
            get {
                throw new NotImplementedException();
            }
        }

        public DTE DTE {
            get {
                return _dte;
            }
        }

        public string ExtenderCATID {
            get {
                throw new NotImplementedException();
            }
        }

        public object ExtenderNames {
            get {
                throw new NotImplementedException();
            }
        }

        public string FileName {
            get {
                throw new NotImplementedException();
            }
        }

        public string FullName {
            get {
                throw new NotImplementedException();
            }
        }

        public Globals Globals {
            get {
                throw new NotImplementedException();
            }
        }

        public bool IsDirty {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool IsOpen {
            get {
                throw new NotImplementedException();
            }
        }

        public DTE Parent {
            get {
                throw new NotImplementedException();
            }
        }

        public Projects Projects {
            get {
                throw new NotImplementedException();
            }
        }

        public Properties Properties {
            get {
                throw new NotImplementedException();
            }
        }

        public bool Saved {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public SolutionBuild SolutionBuild {
            get {
                throw new NotImplementedException();
            }
        }

        public Project AddFromFile(string FileName, bool Exclusive = false) {
            throw new NotImplementedException();
        }

        public Project AddFromTemplate(string FileName, string Destination, string ProjectName, bool Exclusive = false) {
            throw new NotImplementedException();
        }

        public void Close(bool SaveFirst = false) {
            ErrorHandler.ThrowOnFailure(_dte._vs.Solution.Close());
        }

        public void Create(string Destination, string Name) {
            throw new NotImplementedException();
        }

        public ProjectItem FindProjectItem(string FileName) {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator() {
            throw new NotImplementedException();
        }

        public object get_Extender(string ExtenderName) {
            throw new NotImplementedException();
        }

        public string get_TemplatePath(string ProjectType) {
            throw new NotImplementedException();
        }

        public Project Item(object index) {
            throw new NotImplementedException();
        }

        public void Open(string FileName) {
            throw new NotImplementedException();
        }

        public string ProjectItemsTemplatePath(string ProjectKind) {
            throw new NotImplementedException();
        }

        public void Remove(Project proj) {
            throw new NotImplementedException();
        }

        public void SaveAs(string FileName) {
            throw new NotImplementedException();
        }
    }
}