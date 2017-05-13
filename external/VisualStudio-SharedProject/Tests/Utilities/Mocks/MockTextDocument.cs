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
using System.Text;
using Microsoft.VisualStudio.Text;

namespace TestUtilities.Mocks {
    public class MockTextDocument : ITextDocument {
        private string _filePath;
        private readonly ITextBuffer _buffer;

        public MockTextDocument(ITextBuffer buffer, string filePath) {
            _buffer = buffer;
            _filePath = filePath;
        }


        #region ITextDocument Members

        public event EventHandler DirtyStateChanged {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public Encoding Encoding {
            get {
                return Encoding.UTF8;
            }
            set {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<EncodingChangedEventArgs> EncodingChanged {
            add {  }
            remove {  }
        }

        public event EventHandler<TextDocumentFileActionEventArgs> FileActionOccurred {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public string FilePath {
            get { return _filePath; }
        }

        public bool IsDirty {
            get { throw new NotImplementedException(); }
        }

        public bool IsReloading {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastContentModifiedTime {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastSavedTime {
            get { throw new NotImplementedException(); }
        }

        public ReloadResult Reload(EditOptions options) {
            throw new NotImplementedException();
        }

        public ReloadResult Reload() {
            throw new NotImplementedException();
        }

        public void Rename(string newFilePath) {
            _filePath = newFilePath;
        }

        public void Save() {
            File.WriteAllText(_filePath, TextBuffer.CurrentSnapshot.GetText());
        }

        public void SaveAs(string filePath, bool overwrite, bool createFolder, Microsoft.VisualStudio.Utilities.IContentType newContentType) {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite, Microsoft.VisualStudio.Utilities.IContentType newContentType) {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite, bool createFolder) {
            throw new NotImplementedException();
        }

        public void SaveAs(string filePath, bool overwrite) {
            throw new NotImplementedException();
        }

        public void SaveCopy(string filePath, bool overwrite, bool createFolder) {
            throw new NotImplementedException();
        }

        public void SaveCopy(string filePath, bool overwrite) {
            throw new NotImplementedException();
        }

        public void SetEncoderFallback(EncoderFallback fallback) {
            throw new NotImplementedException();
        }

        public ITextBuffer TextBuffer {
            get { return _buffer; }
        }

        public void UpdateDirtyState(bool isDirty, DateTime lastContentModifiedTime) {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }
}
