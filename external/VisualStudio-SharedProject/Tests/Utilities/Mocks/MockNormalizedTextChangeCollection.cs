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
using Microsoft.VisualStudio.Text;

namespace TestUtilities.Mocks {
    class MockNormalizedTextChangeCollection : INormalizedTextChangeCollection {
        private readonly ITextChange[] _changes;

        public MockNormalizedTextChangeCollection(params ITextChange[] changes) {
            _changes = changes;
        }

        public bool IncludesLineChanges {
            get {
                foreach (var change in _changes) {
                    if (change.OldText.IndexOfAny(new[] { '\r', '\n' }) != -1 ||
                        change.NewText.IndexOfAny(new[] { '\r', '\n' }) != -1) {
                        return true;
                    }
                }
                return false;
            }
        }

        public int IndexOf(ITextChange item) {
            for (int i = 0; i < _changes.Length; i++) {
                if (_changes[i] == item) {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, ITextChange item) {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index) {
            throw new NotImplementedException();
        }

        public ITextChange this[int index] {
            get {
                return _changes[index];
            }
            set {
                throw new NotImplementedException();
            }
        }

        public void Add(ITextChange item) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(ITextChange item) {
            throw new NotImplementedException();
        }

        public void CopyTo(ITextChange[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public int Count {
            get { return _changes.Length; }
        }

        public bool IsReadOnly {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(ITextChange item) {
            throw new NotImplementedException();
        }

        public IEnumerator<ITextChange> GetEnumerator() {
            foreach (var change in _changes) {
                yield return change;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            foreach (var change in _changes) {
                yield return change;
            }
        }
    }
}
