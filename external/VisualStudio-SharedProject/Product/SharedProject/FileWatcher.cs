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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudioTools {
    /// <summary>
    ///     Listens to the file system change notifications and raises events when a directory,
    ///     or file in a directory, changes.  This replaces using FileSystemWatcher as this
    ///     implementation/wrapper uses a guard to make sure we don't try to operate during disposal.
    /// </summary>
    internal sealed class FileWatcher : IDisposable {
        private FileSystemWatcher _fsw;

        public FileWatcher(string path = "", string filter = "*.*") {
            _fsw = new FileSystemWatcher(path, filter);
        }

        public bool IsDisposing { get; private set; }

        public bool EnableRaisingEvents {
            get { return !IsDisposing ? _fsw.EnableRaisingEvents : false; }
            set { if (!IsDisposing) { _fsw.EnableRaisingEvents = value; } }
        }

        public bool IncludeSubdirectories {
            get { return !IsDisposing ? _fsw.IncludeSubdirectories : false; }
            set { if (!IsDisposing) { _fsw.IncludeSubdirectories = value; } }
        }

        /// <summary>
        /// The internal buffer size in bytes. The default is 8192 (8 KB).
        /// </summary>
        public int InternalBufferSize {
            get { return !IsDisposing ? _fsw.InternalBufferSize : 0; }
            set { if (!IsDisposing) { _fsw.InternalBufferSize = value; } }
        }

        public NotifyFilters NotifyFilter {
            get { return !IsDisposing ? _fsw.NotifyFilter : new NotifyFilters(); }
            set { if (!IsDisposing) { _fsw.NotifyFilter = value; } }
        }

        public event FileSystemEventHandler Changed {
            add {
                _fsw.Changed += value;
            }
            remove {
                _fsw.Changed -= value;
            }
        }

        public event FileSystemEventHandler Created {
            add {
                _fsw.Created += value;
            }
            remove {
                _fsw.Created -= value;
            }
        }

        public event FileSystemEventHandler Deleted {
            add {
                _fsw.Deleted += value;
            }
            remove {
                _fsw.Deleted -= value;
            }
        }

        public event ErrorEventHandler Error {
            add {
                _fsw.Error += value;
            }
            remove {
                _fsw.Error -= value;
            }
        }

        public event RenamedEventHandler Renamed {
            add {
                _fsw.Renamed += value;
            }
            remove {
                _fsw.Renamed -= value;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_fsw",
            Justification = "Will be disposed on a separate thread to avoid deadlocks")]
        public void Dispose() {
            if (!IsDisposing) {
                IsDisposing = true;

                // Call the _fsw dispose method from the background so it doesn't block anything else.
                var backgroundDispose = new Thread(BackgroundDispose);
                backgroundDispose.IsBackground = true;
                backgroundDispose.Start();
            }
        }

        private void BackgroundDispose()
        {
            try{
                _fsw.Dispose();
            }
            catch(Exception){}
        }
    }
}

