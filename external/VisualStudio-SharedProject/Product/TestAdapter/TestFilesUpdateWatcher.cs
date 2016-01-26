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
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudioTools;

namespace Microsoft.VisualStudioTools.TestAdapter {
    class TestFilesUpdateWatcher : IDisposable {
        private readonly IDictionary<string, FileSystemWatcher> _fileWatchers;
        public event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        public TestFilesUpdateWatcher() {
            _fileWatchers = new Dictionary<string, FileSystemWatcher>(StringComparer.OrdinalIgnoreCase);
        }

        public bool AddWatch(string path) {
            ValidateArg.NotNull(path,"path");

            if (!String.IsNullOrEmpty(path)) {
                var directoryName = Path.GetDirectoryName(path);
                var filter = Path.GetFileName(path);

                if (!_fileWatchers.ContainsKey(path) && Directory.Exists(directoryName)) {
                    var watcher = new FileSystemWatcher(directoryName, filter);
                    _fileWatchers[path] = watcher;

                    watcher.Changed += OnChanged;
                    watcher.EnableRaisingEvents = true;
                    return true;
                }
            }
            return false;
        }

        public bool AddDirectoryWatch(string path) {
            ValidateArg.NotNull(path, "path");

            if (!String.IsNullOrEmpty(path)) {
                if (!_fileWatchers.ContainsKey(path) && Directory.Exists(path)) {
                    var watcher = new FileSystemWatcher(path);
                    _fileWatchers[path] = watcher;

                    watcher.IncludeSubdirectories = true;
                    watcher.Changed += OnChanged;
                    watcher.Renamed += OnRenamed;
                    watcher.EnableRaisingEvents = true;
                    return true;
                }
            }
            return false;
        }

        public void RemoveWatch(string path) {
            ValidateArg.NotNull(path, "path");

            if (!String.IsNullOrEmpty(path)) {
                FileSystemWatcher watcher;

                if (_fileWatchers.TryGetValue(path, out watcher)) {
                    watcher.EnableRaisingEvents = false;

                    _fileWatchers.Remove(path);

                    watcher.Changed -= OnChanged;
                    watcher.Dispose();
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e) {
            var evt = FileChangedEvent;
            if (evt != null) {
                evt(sender, new TestFileChangedEventArgs(null, e.FullPath, TestFileChangedReason.Changed));
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e) {
            var evt = FileChangedEvent;
            if (evt != null) {
                evt(sender, new TestFileChangedEventArgs(null, e.FullPath, TestFileChangedReason.Renamed));
            }
        }

        public void Dispose() {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing && _fileWatchers != null) {
                foreach (var watcher in _fileWatchers.Values) {
                    if (watcher != null) {
                        watcher.Changed -= OnChanged;
                        watcher.Dispose();
                    }
                }

                _fileWatchers.Clear();
            }
        }
    }
}
