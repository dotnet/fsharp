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
using Microsoft.VisualStudio.Text.IncrementalSearch;
using TestUtilities.Mocks;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockIncrementalSearch : IIncrementalSearch {
        private readonly MockTextView _view;
        
        public MockIncrementalSearch(MockTextView textView) {
            _view = textView;
        }

        public IncrementalSearchResult AppendCharAndSearch(char toAppend) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public IncrementalSearchResult DeleteCharAndSearch() {
            throw new NotImplementedException();
        }

        public void Dismiss() {
            throw new NotImplementedException();
        }

        public bool IsActive {
            get { return false; }
        }

        public IncrementalSearchDirection SearchDirection {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string SearchString {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public IncrementalSearchResult SelectNextResult() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public VisualStudio.Text.Editor.ITextView TextView {
            get { throw new NotImplementedException(); }
        }
    }
}
