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
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudioTools.Project;

namespace Microsoft.VisualStudioTools.Navigation {

    /// <summary>
    /// Inplementation of the service that builds the information to expose to the symbols
    /// navigation tools (class view or object browser) from the source files inside a
    /// hierarchy.
    /// </summary>
    internal abstract partial class LibraryManager : IDisposable, IVsRunningDocTableEvents {
        private readonly CommonPackage/*!*/ _package;
        private readonly Dictionary<uint, TextLineEventListener> _documents;
        private readonly Dictionary<IVsHierarchy, HierarchyInfo> _hierarchies = new Dictionary<IVsHierarchy, HierarchyInfo>();
        private readonly Dictionary<ModuleId, LibraryNode> _files;
        private readonly Library _library;
        private readonly IVsEditorAdaptersFactoryService _adapterFactory;
        private uint _objectManagerCookie;
        private uint _runningDocTableCookie;

        public LibraryManager(CommonPackage/*!*/ package) {
            Contract.Assert(package != null);
            _package = package;
            _documents = new Dictionary<uint, TextLineEventListener>();
            _library = new Library(new Guid(CommonConstants.LibraryGuid));
            _library.LibraryCapabilities = (_LIB_FLAGS2)_LIB_FLAGS.LF_PROJECT;
            _files = new Dictionary<ModuleId, LibraryNode>();

            var model = ((IServiceContainer)package).GetService(typeof(SComponentModel)) as IComponentModel;
            _adapterFactory = model.GetService<IVsEditorAdaptersFactoryService>();

            // Register our library now so it'll be available for find all references
            RegisterLibrary();
        }

        public Library Library {
            get { return _library; }
        }

        protected abstract LibraryNode CreateLibraryNode(LibraryNode parent, IScopeNode subItem, string namePrefix, IVsHierarchy hierarchy, uint itemid);

        public virtual LibraryNode CreateFileLibraryNode(LibraryNode parent, HierarchyNode hierarchy, string name, string filename, LibraryNodeType libraryNodeType) {
            return new LibraryNode(null, name, filename, libraryNodeType);
        }

        private object GetPackageService(Type/*!*/ type) {
            return ((System.IServiceProvider)_package).GetService(type);
        }

        private void RegisterForRDTEvents() {
            if (0 != _runningDocTableCookie) {
                return;
            }
            IVsRunningDocumentTable rdt = GetPackageService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (null != rdt) {
                // Do not throw here in case of error, simply skip the registration.
                rdt.AdviseRunningDocTableEvents(this, out _runningDocTableCookie);
            }
        }

        private void UnregisterRDTEvents() {
            if (0 == _runningDocTableCookie) {
                return;
            }
            IVsRunningDocumentTable rdt = GetPackageService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (null != rdt) {
                // Do not throw in case of error.
                rdt.UnadviseRunningDocTableEvents(_runningDocTableCookie);
            }
            _runningDocTableCookie = 0;
        }

        #region ILibraryManager Members

        public void RegisterHierarchy(IVsHierarchy hierarchy) {
            if ((null == hierarchy) || _hierarchies.ContainsKey(hierarchy)) {
                return;
            }

            RegisterLibrary();
            var commonProject = hierarchy.GetProject().GetCommonProject();
            HierarchyListener listener = new HierarchyListener(hierarchy, this);
            var node = _hierarchies[hierarchy] = new HierarchyInfo(
                listener,
                new ProjectLibraryNode(commonProject)
            );
            _library.AddNode(node.ProjectLibraryNode);
            listener.StartListening(true);
            RegisterForRDTEvents();
        }

        private void RegisterLibrary() {
            if (0 == _objectManagerCookie) {
                IVsObjectManager2 objManager = GetPackageService(typeof(SVsObjectManager)) as IVsObjectManager2;
                if (null == objManager) {
                    return;
                }
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                    objManager.RegisterSimpleLibrary(_library, out _objectManagerCookie));
            }
        }

        public void UnregisterHierarchy(IVsHierarchy hierarchy) {
            if ((null == hierarchy) || !_hierarchies.ContainsKey(hierarchy)) {
                return;
            }
            HierarchyInfo info = _hierarchies[hierarchy];
            if (null != info) {
                info.Listener.Dispose();
            }
            _hierarchies.Remove(hierarchy);
            _library.RemoveNode(info.ProjectLibraryNode);
            if (0 == _hierarchies.Count) {
                UnregisterRDTEvents();
            }
            lock (_files) {
                ModuleId[] keys = new ModuleId[_files.Keys.Count];
                _files.Keys.CopyTo(keys, 0);
                foreach (ModuleId id in keys) {
                    if (hierarchy.Equals(id.Hierarchy)) {
                        _library.RemoveNode(_files[id]);
                        _files.Remove(id);
                    }
                }
            }
            // Remove the document listeners.
            uint[] docKeys = new uint[_documents.Keys.Count];
            _documents.Keys.CopyTo(docKeys, 0);
            foreach (uint id in docKeys) {
                TextLineEventListener docListener = _documents[id];
                if (hierarchy.Equals(docListener.FileID.Hierarchy)) {
                    _documents.Remove(id);
                    docListener.Dispose();
                }
            }
        }

        public void RegisterLineChangeHandler(uint document,
            TextLineChangeEvent lineChanged, Action<IVsTextLines> onIdle) {
            _documents[document].OnFileChangedImmediate += delegate(object sender, TextLineChange[] changes, int fLast) {
                lineChanged(sender, changes, fLast);
            };
            _documents[document].OnFileChanged += (sender, args) => onIdle(args.TextBuffer);
        }

        #endregion

        #region Library Member Production

        /// <summary>
        /// Overridden in the base class to receive notifications of when a file should
        /// be analyzed for inclusion in the library.  The derived class should queue
        /// the parsing of the file and when it's complete it should call FileParsed
        /// with the provided LibraryTask and an IScopeNode which provides information
        /// about the members of the file.
        /// </summary>
        protected virtual void OnNewFile(LibraryTask task) {
        }

        /// <summary>
        /// Called by derived class when a file has been parsed.  The caller should
        /// provide the LibraryTask received from the OnNewFile call and an IScopeNode
        /// which represents the contents of the library.
        /// 
        /// It is safe to call this method from any thread.
        /// </summary>
        protected void FileParsed(LibraryTask task, IScopeNode scope) {
            try {
                var project = task.ModuleID.Hierarchy.GetProject().GetCommonProject();

                HierarchyNode fileNode = fileNode = project.NodeFromItemId(task.ModuleID.ItemID);
                HierarchyInfo parent;
                if (fileNode == null || !_hierarchies.TryGetValue(task.ModuleID.Hierarchy, out parent)) {
                    return;
                }

                LibraryNode module = CreateFileLibraryNode(
                    parent.ProjectLibraryNode,
                    fileNode,
                    System.IO.Path.GetFileName(task.FileName),
                    task.FileName,
                    LibraryNodeType.Package | LibraryNodeType.Classes
                );

                // TODO: Creating the module tree should be done lazily as needed
                // Currently we replace the entire tree and rely upon the libraries
                // update count to invalidate the whole thing.  We could do this
                // finer grained and only update the changed nodes.  But then we
                // need to make sure we're not mutating lists which are handed out.

                CreateModuleTree(module, scope, task.FileName + ":", task.ModuleID);
                if (null != task.ModuleID) {
                    LibraryNode previousItem = null;
                    lock (_files) {
                        if (_files.TryGetValue(task.ModuleID, out previousItem)) {
                            _files.Remove(task.ModuleID);
                            parent.ProjectLibraryNode.RemoveNode(previousItem);
                        }
                    }
                }
                parent.ProjectLibraryNode.AddNode(module);
                _library.Update();
                if (null != task.ModuleID) {
                    lock (_files) {
                        _files.Add(task.ModuleID, module);
                    }
                }
            } catch (COMException) {
                // we're shutting down and can't get the project
            }
        }

        private void CreateModuleTree(LibraryNode current, IScopeNode scope, string namePrefix, ModuleId moduleId) {
            if ((null == scope) || (null == scope.NestedScopes)) {
                return;
            }

            foreach (IScopeNode subItem in scope.NestedScopes) {
                LibraryNode newNode = CreateLibraryNode(current, subItem, namePrefix, moduleId.Hierarchy, moduleId.ItemID);
                string newNamePrefix = namePrefix;

                current.AddNode(newNode);
                if ((newNode.NodeType & LibraryNodeType.Classes) != LibraryNodeType.None) {
                    newNamePrefix = namePrefix + newNode.Name + ".";
                }

                // Now use recursion to get the other types.
                CreateModuleTree(newNode, subItem, newNamePrefix, moduleId);
            }
        }
        #endregion

        #region Hierarchy Events

        private void OnNewFile(object sender, HierarchyEventArgs args) {
            IVsHierarchy hierarchy = sender as IVsHierarchy;
            if (null == hierarchy || IsNonMemberItem(hierarchy, args.ItemID)) {
                return;
            }

            ITextBuffer buffer = null;
            if (null != args.TextBuffer) {
                buffer = _adapterFactory.GetDocumentBuffer(args.TextBuffer);
            }

            var id = new ModuleId(hierarchy, args.ItemID);
            OnNewFile(new LibraryTask(args.CanonicalName, buffer, new ModuleId(hierarchy, args.ItemID)));
        }

        /// <summary>
        /// Handles the delete event, checking to see if this is a project item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDeleteFile(object sender, HierarchyEventArgs args) {
            IVsHierarchy hierarchy = sender as IVsHierarchy;
            if (null == hierarchy || IsNonMemberItem(hierarchy, args.ItemID)) {
                return;
            }

            OnDeleteFile(hierarchy, args);
        }

        /// <summary>
        /// Does a delete w/o checking if it's a non-meber item, for handling the
        /// transition from member item to non-member item.
        /// </summary>
        private void OnDeleteFile(IVsHierarchy hierarchy, HierarchyEventArgs args) {
            ModuleId id = new ModuleId(hierarchy, args.ItemID);
            LibraryNode node = null;
            lock (_files) {
                if (_files.TryGetValue(id, out node)) {
                    _files.Remove(id);
                    HierarchyInfo parent;
                    if (_hierarchies.TryGetValue(hierarchy, out parent)) {
                        parent.ProjectLibraryNode.RemoveNode(node);
                    }
                }
            }
            if (null != node) {
                _library.RemoveNode(node);
            }
        }

        private void IsNonMemberItemChanged(object sender, HierarchyEventArgs args) {
            IVsHierarchy hierarchy = sender as IVsHierarchy;
            if (null == hierarchy) {
                return;
            }

            if (!IsNonMemberItem(hierarchy, args.ItemID)) {
                OnNewFile(hierarchy, args);
            } else {
                OnDeleteFile(hierarchy, args);
            }
        }

        /// <summary>
        /// Checks whether this hierarchy item is a project member (on disk items from show all
        /// files aren't considered project members).
        /// </summary>
        protected bool IsNonMemberItem(IVsHierarchy hierarchy, uint itemId) {
            object val;
            int hr = hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out val);
            return ErrorHandler.Succeeded(hr) && (bool)val;
        }

        #endregion

        #region IVsRunningDocTableEvents Members

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs) {
            if ((grfAttribs & (uint)(__VSRDTATTRIB.RDTA_MkDocument)) == (uint)__VSRDTATTRIB.RDTA_MkDocument) {
                IVsRunningDocumentTable rdt = GetPackageService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
                if (rdt != null) {
                    uint flags, readLocks, editLocks, itemid;
                    IVsHierarchy hier;
                    IntPtr docData = IntPtr.Zero;
                    string moniker;
                    int hr;
                    try {
                        hr = rdt.GetDocumentInfo(docCookie, out flags, out readLocks, out editLocks, out moniker, out hier, out itemid, out docData);
                        TextLineEventListener listner;
                        if (_documents.TryGetValue(docCookie, out listner)) {
                            listner.FileName = moniker;
                        }
                    } finally {
                        if (IntPtr.Zero != docData) {
                            Marshal.Release(docData);
                        }
                    }
                }
            }
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame) {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie) {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) {
            // Check if this document is in the list of the documents.
            if (_documents.ContainsKey(docCookie)) {
                return VSConstants.S_OK;
            }
            // Get the information about this document from the RDT.
            IVsRunningDocumentTable rdt = GetPackageService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (null != rdt) {
                // Note that here we don't want to throw in case of error.
                uint flags;
                uint readLocks;
                uint writeLoks;
                string documentMoniker;
                IVsHierarchy hierarchy;
                uint itemId;
                IntPtr unkDocData;
                int hr = rdt.GetDocumentInfo(docCookie, out flags, out readLocks, out writeLoks,
                                             out documentMoniker, out hierarchy, out itemId, out unkDocData);
                try {
                    if (Microsoft.VisualStudio.ErrorHandler.Failed(hr) || (IntPtr.Zero == unkDocData)) {
                        return VSConstants.S_OK;
                    }
                    // Check if the herarchy is one of the hierarchies this service is monitoring.
                    if (!_hierarchies.ContainsKey(hierarchy)) {
                        // This hierarchy is not monitored, we can exit now.
                        return VSConstants.S_OK;
                    }

                    // Check the file to see if a listener is required.
                    if (_package.IsRecognizedFile(documentMoniker)) {
                        return VSConstants.S_OK;
                    }

                    // Create the module id for this document.
                    ModuleId docId = new ModuleId(hierarchy, itemId);

                    // Try to get the text buffer.
                    IVsTextLines buffer = Marshal.GetObjectForIUnknown(unkDocData) as IVsTextLines;

                    // Create the listener.
                    TextLineEventListener listener = new TextLineEventListener(buffer, documentMoniker, docId);
                    // Set the event handler for the change event. Note that there is no difference
                    // between the AddFile and FileChanged operation, so we can use the same handler.
                    listener.OnFileChanged += new EventHandler<HierarchyEventArgs>(OnNewFile);
                    // Add the listener to the dictionary, so we will not create it anymore.
                    _documents.Add(docCookie, listener);
                } finally {
                    if (IntPtr.Zero != unkDocData) {
                        Marshal.Release(unkDocData);
                    }
                }
            }
            // Always return success.
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
            if ((0 != dwEditLocksRemaining) || (0 != dwReadLocksRemaining)) {
                return VSConstants.S_OK;
            }
            TextLineEventListener listener;
            if (!_documents.TryGetValue(docCookie, out listener) || (null == listener)) {
                return VSConstants.S_OK;
            }
            using (listener) {
                _documents.Remove(docCookie);
                // Now make sure that the information about this file are up to date (e.g. it is
                // possible that Class View shows something strange if the file was closed without
                // saving the changes).
                HierarchyEventArgs args = new HierarchyEventArgs(listener.FileID.ItemID, listener.FileName);
                OnNewFile(listener.FileID.Hierarchy, args);
            }
            return VSConstants.S_OK;
        }

        #endregion

        public void OnIdle(IOleComponentManager compMgr) {
            foreach (TextLineEventListener listener in _documents.Values) {
                if (compMgr.FContinueIdle() == 0) {
                    break;
                }

                listener.OnIdle();
            }
        }

        #region IDisposable Members

        public void Dispose() {
            // Dispose all the listeners.
            foreach (var info in _hierarchies.Values) {
                info.Listener.Dispose();
            }
            _hierarchies.Clear();

            foreach (TextLineEventListener textListener in _documents.Values) {
                textListener.Dispose();
            }
            _documents.Clear();

            // Remove this library from the object manager.
            if (0 != _objectManagerCookie) {
                IVsObjectManager2 mgr = GetPackageService(typeof(SVsObjectManager)) as IVsObjectManager2;
                if (null != mgr) {
                    mgr.UnregisterLibrary(_objectManagerCookie);
                }
                _objectManagerCookie = 0;
            }

            // Unregister this object from the RDT events.
            UnregisterRDTEvents();
        }

        #endregion

        class HierarchyInfo {
            public readonly HierarchyListener Listener;
            public readonly ProjectLibraryNode ProjectLibraryNode;

            public HierarchyInfo(HierarchyListener listener, ProjectLibraryNode projectLibNode) {
                Listener = listener;
                ProjectLibraryNode = projectLibNode;
            }
        }
    }
}
