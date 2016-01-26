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
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace TestUtilities.Mocks {
    public class MockTextBuffer : ITextBuffer {
        private readonly IContentType _contentType;
        internal MockTextSnapshot _snapshot;
        private MockTextEdit _edit;

        /// <summary>
        /// Do not access this field. Use <see cref="Properties"/> instead.
        /// </summary>
        private PropertyCollection _properties;

        public MockTextBuffer(string content) {
        }

        public MockTextBuffer(string content, string contentType, string filename = null) {
            _snapshot = new MockTextSnapshot(this, content);
            _contentType = new MockContentType(contentType, new IContentType[0]);
            if (filename == null) {
                filename = Path.Combine(TestData.GetTempPath(), Path.GetRandomFileName(), "file.py");
            }
            Properties[typeof(ITextDocument)] = new MockTextDocument(this, filename);
        }

        public MockTextBuffer(string content, IContentType contentType, string filename = null) {
            _snapshot = new MockTextSnapshot(this, content);
            _contentType = contentType;
            if (filename == null) {
                filename = Path.Combine(TestData.GetTempPath(), Path.GetRandomFileName(), "file.py");
            }
            Properties[typeof(ITextDocument)] = new MockTextDocument(this, filename);
        }

        public void ChangeContentType(Microsoft.VisualStudio.Utilities.IContentType newContentType, object editTag) {
            throw new NotImplementedException();
        }

        public event EventHandler<TextContentChangedEventArgs> Changed;

        public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;

        public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;

#pragma warning disable 67
        public event EventHandler<TextContentChangingEventArgs> Changing;

        public event EventHandler PostChanged;

        public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;

        public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;

#pragma warning restore 67

        /// <summary>
        /// Raises a fake changed low priority event
        /// </summary>
        public void RaiseChangedLowPriority() {
            var changed = ChangedLowPriority;
            if (changed != null) {
                var oldSnapshot = _snapshot;
                var newSnapshot = new MockTextSnapshot(this, _snapshot.GetText(), _snapshot, 
                    new MockTextChange(
                        new SnapshotSpan(_snapshot, 0, _snapshot.Length),
                        0,
                        _snapshot.GetText()
                    )
                );
                _snapshot = newSnapshot;
                changed(this, new TextContentChangedEventArgs(oldSnapshot, newSnapshot, EditOptions.None, null));
            }
        }

        public bool CheckEditAccess() {
            throw new NotImplementedException();
        }

        public IContentType ContentType {
            get { return _contentType; }
        }

        public ITextEdit CreateEdit() {
            if (EditInProgress) {
                throw new InvalidOperationException();
            }
            _edit = new MockTextEdit((MockTextSnapshot)CurrentSnapshot);
            return _edit;
        }

        public ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag) {
            throw new NotImplementedException();
        }

        public IReadOnlyRegionEdit CreateReadOnlyRegionEdit() {
            throw new NotImplementedException();
        }

        public ITextSnapshot CurrentSnapshot {
            get { return _snapshot; }
        }

        public ITextSnapshot Delete(Span deleteSpan) {
            using (var edit = CreateEdit()) {
                edit.Delete(deleteSpan);
                return edit.Apply();
            }
        }

        public bool EditInProgress {
            get { return _edit != null; }
        }

        internal void EditApplied(ITextSnapshot previous) {
            _edit = null;
            var e = new TextContentChangedEventArgs(previous, _snapshot, new EditOptions(), null);
            foreach (var evt in new[] { ChangedHighPriority, Changed, ChangedLowPriority }) {
                if (evt != null) {
                    evt(this, e);
                }
            }
        }

        public NormalizedSpanCollection GetReadOnlyExtents(Span span) {
            throw new NotImplementedException();
        }

        public ITextSnapshot Insert(int position, string text) {
            using (var edit = CreateEdit()) {
                edit.Insert(position, text);
                return edit.Apply();
            }
        }

        public bool IsReadOnly(Span span, bool isEdit) {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(Span span) {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(int position, bool isEdit) {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(int position) {
            throw new NotImplementedException();
        }

        public ITextSnapshot Replace(Span replaceSpan, string replaceWith) {
            var oldText = _snapshot.GetText();
            string newText = oldText.Remove(replaceSpan.Start, replaceSpan.Length);
            newText  = newText.Insert(replaceSpan.Start, replaceWith);

            _snapshot = new MockTextSnapshot(
                this, 
                newText, 
                _snapshot, 
                new MockTextChange(
                    new SnapshotSpan(_snapshot, replaceSpan), 
                    replaceSpan.Start,
                    replaceWith
                )
            );
            return _snapshot;
        }

        public void TakeThreadOwnership() {
            throw new NotImplementedException();
        }

        public Microsoft.VisualStudio.Utilities.PropertyCollection Properties {
            get {
                if (_properties == null) {
                    _properties = new PropertyCollection();
                }
                return _properties;
            }
        }

        public void AddProperty(object key, object value) {
            Properties.AddProperty(key, value);
        }
    }
}
