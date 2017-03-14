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
using System.Threading;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using VSConstants = Microsoft.VisualStudio.VSConstants;

namespace Microsoft.VisualStudioTools.Navigation {

    /// <summary>
    /// Implements a simple library that tracks project symbols, objects etc.
    /// </summary>
    class Library : IVsSimpleLibrary2 {
        private Guid _guid;
        private _LIB_FLAGS2 _capabilities;
        private LibraryNode _root;
        private uint _updateCount;

        public Library(Guid libraryGuid) {
            _guid = libraryGuid;
            _root = new LibraryNode(null, String.Empty, String.Empty, LibraryNodeType.Package);
        }

        public _LIB_FLAGS2 LibraryCapabilities {
            get { return _capabilities; }
            set { _capabilities = value; }
        }

        internal void AddNode(LibraryNode node) {
            lock (this) {
                // re-create root node here because we may have handed out the node before and don't want to mutate it's list.
                _root = _root.Clone();
                _root.AddNode(node);
                _updateCount++;
            }
        }

        internal void RemoveNode(LibraryNode node) {
            lock (this) {
                _root = _root.Clone();
                _root.RemoveNode(node);
                _updateCount++;
            }
        }

        #region IVsSimpleLibrary2 Members

        public int AddBrowseContainer(VSCOMPONENTSELECTORDATA[] pcdComponent, ref uint pgrfOptions, out string pbstrComponentAdded) {
            pbstrComponentAdded = null;
            return VSConstants.E_NOTIMPL;
        }

        public int CreateNavInfo(SYMBOL_DESCRIPTION_NODE[] rgSymbolNodes, uint ulcNodes, out IVsNavInfo ppNavInfo) {
            ppNavInfo = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetBrowseContainersForHierarchy(IVsHierarchy pHierarchy, uint celt, VSBROWSECONTAINER[] rgBrowseContainers, uint[] pcActual) {
            return VSConstants.E_NOTIMPL;
        }

        public int GetGuid(out Guid pguidLib) {
            pguidLib = _guid;
            return VSConstants.S_OK;
        }

        public int GetLibFlags2(out uint pgrfFlags) {
            pgrfFlags = (uint)LibraryCapabilities;
            return VSConstants.S_OK;
        }

        public int GetList2(uint ListType, uint flags, VSOBSEARCHCRITERIA2[] pobSrch, out IVsSimpleObjectList2 ppIVsSimpleObjectList2) {
            if ((flags & (uint)_LIB_LISTFLAGS.LLF_RESOURCEVIEW) != 0) {
                ppIVsSimpleObjectList2 = null;
                return VSConstants.E_NOTIMPL;
            }

            ICustomSearchListProvider listProvider;
            if (pobSrch != null &&
                pobSrch.Length > 0) {
                if ((listProvider = pobSrch[0].pIVsNavInfo as ICustomSearchListProvider) != null) {
                    switch ((_LIB_LISTTYPE)ListType) {
                        case _LIB_LISTTYPE.LLT_NAMESPACES:
                            ppIVsSimpleObjectList2 = listProvider.GetSearchList();
                            break;
                        default:
                            ppIVsSimpleObjectList2 = null;
                            return VSConstants.E_FAIL;
                    }
                } else {
                    if (pobSrch[0].eSrchType == VSOBSEARCHTYPE.SO_ENTIREWORD && ListType == (uint)_LIB_LISTTYPE.LLT_MEMBERS) {
                        string srchText = pobSrch[0].szName;
                        int colonIndex;
                        if ((colonIndex = srchText.LastIndexOf(':')) != -1) {
                            string filename = srchText.Substring(0, srchText.LastIndexOf(':'));
                            foreach (ProjectLibraryNode project in _root.Children) {
                                foreach (var item in project.Children) {
                                    if (item.FullName == filename) {
                                        ppIVsSimpleObjectList2 = item.DoSearch(pobSrch[0]);
                                        if (ppIVsSimpleObjectList2 != null) {
                                            return VSConstants.S_OK;
                                        }
                                    }
                                }
                            }
                        }

                        ppIVsSimpleObjectList2 = null;
                        return VSConstants.E_FAIL;
                    } else if (pobSrch[0].eSrchType == VSOBSEARCHTYPE.SO_SUBSTRING && ListType == (uint)_LIB_LISTTYPE.LLT_NAMESPACES) {
                        var lib = new LibraryNode(null, "Search results " + pobSrch[0].szName, "Search results " + pobSrch[0].szName, LibraryNodeType.Package);
                        foreach (var item in SearchNodes(pobSrch[0], new SimpleObjectList<LibraryNode>(), _root).Children) {
                            lib.Children.Add(item);
                        }
                        ppIVsSimpleObjectList2 = lib;
                        return VSConstants.S_OK;
                    } else {
                        ppIVsSimpleObjectList2 = null;
                        return VSConstants.E_FAIL;
                    }
                }
            } else {
                ppIVsSimpleObjectList2 = _root as IVsSimpleObjectList2;
            }
            return VSConstants.S_OK;
        }

        private static SimpleObjectList<LibraryNode> SearchNodes(VSOBSEARCHCRITERIA2 srch, SimpleObjectList<LibraryNode> list, LibraryNode curNode) {
            foreach (var child in curNode.Children) {
                if (child.Name.IndexOf(srch.szName, StringComparison.OrdinalIgnoreCase) != -1) {
                    list.Children.Add(child.Clone(child.Name));
                }

                SearchNodes(srch, list, child);
            }
            return list;
        }

        public void VisitNodes(ILibraryNodeVisitor visitor, CancellationToken ct = default(CancellationToken)) {
            lock (this) {
                _root.Visit(visitor, ct);
            }
        }

        public int GetSeparatorStringWithOwnership(out string pbstrSeparator) {
            pbstrSeparator = ".";
            return VSConstants.S_OK;
        }

        public int GetSupportedCategoryFields2(int Category, out uint pgrfCatField) {
            pgrfCatField = (uint)_LIB_CATEGORY2.LC_HIERARCHYTYPE | (uint)_LIB_CATEGORY2.LC_PHYSICALCONTAINERTYPE;
            return VSConstants.S_OK;
        }

        public int LoadState(IStream pIStream, LIB_PERSISTTYPE lptType) {
            return VSConstants.S_OK;
        }

        public int RemoveBrowseContainer(uint dwReserved, string pszLibName) {
            return VSConstants.E_NOTIMPL;
        }

        public int SaveState(IStream pIStream, LIB_PERSISTTYPE lptType) {
            return VSConstants.S_OK;
        }

        public int UpdateCounter(out uint pCurUpdate) {
            pCurUpdate = _updateCount;
            return VSConstants.S_OK;
        }

        public void Update() {
            _updateCount++;
            _root.Update();
        }
        #endregion
    }
}
