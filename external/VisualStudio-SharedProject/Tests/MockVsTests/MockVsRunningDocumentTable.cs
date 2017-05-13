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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsRunningDocumentTable : IVsRunningDocumentTable
#if DEV12_OR_LATER
        , IVsRunningDocumentTable4
#endif
        {
        private readonly MockVs _vs;
        private readonly Dictionary<uint, DocInfo> _table = new Dictionary<uint, DocInfo>();
        private readonly Dictionary<string, uint> _ids = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        private uint _curCookie;

        public MockVsRunningDocumentTable(MockVs vs) {
            _vs = vs;
        }

        class DocInfo {
            public _VSRDTFLAGS Flags;
            public string Document;
            public IVsHierarchy Hierarchy;
            public uint ItemId;
            public IntPtr DocData;
            public uint Cookie;
            public int ReadLockCount = 1;
            public int EditLockCount = 0;

            public DocInfo(_VSRDTFLAGS _VSRDTFLAGS, string pszMkDocument, IVsHierarchy pHier, uint itemid, IntPtr punkDocData, uint p) {
                Flags = _VSRDTFLAGS;
                Document = pszMkDocument;
                Hierarchy = pHier;
                ItemId = itemid;
                DocData = punkDocData;
                Cookie = p;
            }
        }

        public int AdviseRunningDocTableEvents(IVsRunningDocTableEvents pSink, out uint pdwCookie) {
            throw new NotImplementedException();
        }

        public int FindAndLockDocument(uint dwRDTLockType, string pszMkDocument, out IVsHierarchy ppHier, out uint pitemid, out IntPtr ppunkDocData, out uint pdwCookie) {
            IVsHierarchy pHier = null;
            uint itemid = 0;
            IntPtr punkDocData = IntPtr.Zero;
            uint dwCookie = 0;

            int res = _vs.Invoke(() => {
                uint id;
                if (_ids.TryGetValue(pszMkDocument, out id)) {
                    var docInfo = _table[id];
                    var lockType = (_VSRDTFLAGS)dwRDTLockType;

                    pHier = docInfo.Hierarchy;
                    itemid = docInfo.ItemId;
                    if (docInfo.DocData != IntPtr.Zero) {
                        Marshal.AddRef(docInfo.DocData);
                    }
                    punkDocData = docInfo.DocData;
                    dwCookie = id;
                    docInfo.Flags = (_VSRDTFLAGS)dwRDTLockType;
                    if (lockType.HasFlag(_VSRDTFLAGS.RDT_ReadLock)) {
                        docInfo.ReadLockCount++;
                    }
                    if (lockType.HasFlag(_VSRDTFLAGS.RDT_EditLock)) {
                        docInfo.EditLockCount++;
                    }
                    return VSConstants.S_OK;
                }
                return VSConstants.S_FALSE;
            });

            ppHier = pHier;
            pitemid = itemid;
            ppunkDocData = punkDocData;
            pdwCookie = dwCookie;
            return res;
        }

        public int GetDocumentInfo(uint docCookie, out uint pgrfRDTFlags, out uint pdwReadLocks, out uint pdwEditLocks, out string pbstrMkDocument, out IVsHierarchy ppHier, out uint pitemid, out IntPtr ppunkDocData) {
            uint grfRDTFlags = 0;
            uint dwReadLocks = 0;
            uint dwEditLocks = 0;
            string bstrMkDocument = null;
            IVsHierarchy pHier = null;
            uint itemid = 0;
            IntPtr punkDocData = IntPtr.Zero;

            int res = _vs.Invoke(() => {
                DocInfo docInfo;
                if (_table.TryGetValue(docCookie, out docInfo)) {
                    grfRDTFlags = (uint)docInfo.Flags;
                    dwReadLocks = (uint)docInfo.ReadLockCount;
                    dwEditLocks = (uint)docInfo.EditLockCount;
                    bstrMkDocument = docInfo.Document;
                    pHier = docInfo.Hierarchy;
                    itemid = docInfo.ItemId;
                    punkDocData = docInfo.DocData;
                    if (punkDocData != IntPtr.Zero) {
                        Marshal.AddRef(punkDocData);
                    }
                    return VSConstants.S_OK;
                }
                return VSConstants.E_FAIL;
            });

            pgrfRDTFlags = grfRDTFlags;
            pdwReadLocks = dwReadLocks;
            pdwEditLocks = dwEditLocks;
            pbstrMkDocument = bstrMkDocument;
            ppHier = pHier;
            pitemid = itemid;
            ppunkDocData = punkDocData;

            return res;
        }

        public int GetRunningDocumentsEnum(out IEnumRunningDocuments ppenum) {
            IEnumRunningDocuments penum = null;
            int res = _vs.Invoke(() => {
                penum = new RunningDocumentsEnum(this);
                return VSConstants.S_OK;
            });

            ppenum = penum;
            return res;
        }

        class RunningDocumentsEnum : IEnumRunningDocuments {
            private readonly MockVsRunningDocumentTable _docTable;
            private IEnumerator<DocInfo> _enum;

            public RunningDocumentsEnum(MockVsRunningDocumentTable docTable) {
                _docTable = docTable;
                _enum = _docTable._table.Values.GetEnumerator();
            }

            public int Clone(out IEnumRunningDocuments ppenum) {
                _docTable._vs.AssertUIThread();
                ppenum = new RunningDocumentsEnum(_docTable);
                return VSConstants.S_OK;
            }

            public int Next(uint celt, uint[] rgelt, out uint pceltFetched) {
                _docTable._vs.AssertUIThread();
                pceltFetched = 0;
                for (int i = 0; i < celt; i++) {
                    if (_enum.MoveNext()) {
                        rgelt[i] = _enum.Current.Cookie;
                        pceltFetched++;
                    }
                    if (i == celt - 1) {
                        return VSConstants.S_OK;
                    }
                }
                return VSConstants.S_FALSE;
            }

            public int Reset() {
                _docTable._vs.AssertUIThread();
                _enum = _docTable._table.Values.GetEnumerator();
                return VSConstants.S_OK;
            }

            public int Skip(uint celt) {
                for (int i = 0; i < celt && _enum.MoveNext(); i++) {
                }
                return VSConstants.S_OK;
            }
        }

        public int LockDocument(uint grfRDTLockType, uint dwCookie) {
            return _vs.Invoke(() => {
                DocInfo docInfo;
                if (_table.TryGetValue(dwCookie, out docInfo)) {
                    var lockType = (_VSRDTFLAGS)grfRDTLockType;
                    if (lockType.HasFlag(_VSRDTFLAGS.RDT_ReadLock)) {
                        docInfo.ReadLockCount++;
                    }
                    if (lockType.HasFlag(_VSRDTFLAGS.RDT_EditLock)) {
                        docInfo.EditLockCount++;
                    }
                    return VSConstants.S_OK;
                }
                return VSConstants.E_FAIL;
            });
        }

        public int ModifyDocumentFlags(uint docCookie, uint grfFlags, int fSet) {
            throw new NotImplementedException();
        }

        public int NotifyDocumentChanged(uint dwCookie, uint grfDocChanged) {
            throw new NotImplementedException();
        }

        public int NotifyOnAfterSave(uint dwCookie) {
            throw new NotImplementedException();
        }

        public int NotifyOnBeforeSave(uint dwCookie) {
            throw new NotImplementedException();
        }

        public int RegisterAndLockDocument(uint grfRDTLockType, string pszMkDocument, IVsHierarchy pHier, uint itemid, IntPtr punkDocData, out uint pdwCookie) {
            uint cookie = 0;
            int res = _vs.Invoke(() => {
                cookie = _ids[pszMkDocument] = ++_curCookie;
                _table[cookie] = new DocInfo(
                    (_VSRDTFLAGS)grfRDTLockType,
                    pszMkDocument,
                    pHier,
                    itemid,
                    punkDocData,
                    cookie
                );

                if (punkDocData != IntPtr.Zero) {
                    Marshal.AddRef(punkDocData);
                }
                var persist = (IVsPersistDocData)Marshal.GetObjectForIUnknown(punkDocData);
                return persist.OnRegisterDocData(cookie, pHier, itemid);
            });

            pdwCookie = cookie;
            return res;
        }

        public int RegisterDocumentLockHolder(uint grfRDLH, uint dwCookie, IVsDocumentLockHolder pLockHolder, out uint pdwLHCookie) {
            throw new NotImplementedException();
        }

        public int RenameDocument(string pszMkDocumentOld, string pszMkDocumentNew, IntPtr pHier, uint itemidNew) {
            return _vs.Invoke(() => {
                uint id;
                if (_ids.TryGetValue(pszMkDocumentOld, out id)) {
                    DocInfo docInfo = _table[id];

                    var docData = (IVsPersistDocData)Marshal.GetObjectForIUnknown(docInfo.DocData);
                    int hr = docData.RenameDocData(0, (IVsHierarchy)Marshal.GetObjectForIUnknown(pHier), itemidNew, pszMkDocumentNew);
                    if (ErrorHandler.Succeeded(hr)) {
                        docInfo.Document = pszMkDocumentNew;
                        docInfo.Hierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(pHier);
                        docInfo.ItemId = itemidNew;
                        return VSConstants.S_OK;
                    }
                    return hr;
                }
                return VSConstants.E_FAIL;
            });
        }

        public int SaveDocuments(uint grfSaveOpts, IVsHierarchy pHier, uint itemid, uint docCookie) {
            throw new NotImplementedException();
        }

        public int UnadviseRunningDocTableEvents(uint dwCookie) {
            throw new NotImplementedException();
        }

        public int UnlockDocument(uint grfRDTLockType, uint dwCookie) {
            return _vs.Invoke(() => {
                DocInfo docInfo;
                if (_table.TryGetValue(dwCookie, out docInfo)) {
                    var lockType = (_VSRDTFLAGS)grfRDTLockType;
                    if (lockType.HasFlag(_VSRDTFLAGS.RDT_ReadLock)) {
                        docInfo.ReadLockCount--;
                    }
                    if (lockType.HasFlag(_VSRDTFLAGS.RDT_EditLock)) {
                        docInfo.EditLockCount--;
                    }
                    if (docInfo.ReadLockCount + docInfo.EditLockCount == 0) {
                        _ids.Remove(docInfo.Document);
                        _table.Remove(dwCookie);
                        ErrorHandler.ThrowOnFailure(((IVsPersistDocData)Marshal.GetObjectForIUnknown(docInfo.DocData)).Close());
                        Marshal.Release(docInfo.DocData);
                    }
                    return VSConstants.S_OK;
                }
                return VSConstants.E_FAIL;
            });
        }

        public int UnregisterDocumentLockHolder(uint dwLHCookie) {
            throw new NotImplementedException();
        }

#if DEV12_OR_LATER
        public uint GetRelatedSaveTreeItems(uint cookie, uint grfSave, uint celt, VSSAVETREEITEM[] rgSaveTreeItems) {
            throw new NotImplementedException();
        }

        public void NotifyDocumentChangedEx(uint cookie, uint attributes) {
            throw new NotImplementedException();
        }

        public bool IsDocumentDirty(uint cookie) {
            throw new NotImplementedException();
        }

        public bool IsDocumentReadOnly(uint cookie) {
            throw new NotImplementedException();
        }

        public void UpdateDirtyState(uint cookie) {
            throw new NotImplementedException();
        }

        public void UpdateReadOnlyState(uint cookie) {
            throw new NotImplementedException();
        }

        public bool IsMonikerValid(string moniker) {
            return _ids.ContainsKey(moniker);
        }

        public bool IsCookieValid(uint cookie) {
            return _table.ContainsKey(cookie);
        }

        public uint GetDocumentCookie(string moniker) {
            return _ids[moniker];
        }

        public uint GetDocumentFlags(uint cookie) {
            throw new NotImplementedException();
        }

        public uint GetDocumentReadLockCount(uint cookie) {
            throw new NotImplementedException();
        }

        public uint GetDocumentEditLockCount(uint cookie) {
            throw new NotImplementedException();
        }

        public string GetDocumentMoniker(uint cookie) {
            return _table[cookie].Document;
        }

        public void GetDocumentHierarchyItem(uint cookie, out IVsHierarchy hierarchy, out uint itemID) {
            IVsHierarchy hierarchyRes = null;
            uint itemIDRes = (uint)VSConstants.VSITEMID.Nil;

            _vs.InvokeSync(() => {
                DocInfo docInfo;
                if (_table.TryGetValue(cookie, out docInfo)) {
                    hierarchyRes = docInfo.Hierarchy;
                    itemIDRes = docInfo.ItemId;
                }
            });

            hierarchy = hierarchyRes;
            itemID = itemIDRes;
        }

        public dynamic GetDocumentData(uint cookie) {
            throw new NotImplementedException();
        }

        public Guid GetDocumentProjectGuid(uint cookie) {
            throw new NotImplementedException();
        }
#endif
    }
}
