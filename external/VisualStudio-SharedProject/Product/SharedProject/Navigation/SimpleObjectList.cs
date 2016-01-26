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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools.Project;

namespace Microsoft.VisualStudioTools.Navigation {
    /// <summary>
    /// Represents a simple list which the VS UI can query for items.
    /// 
    /// VS assumes that these lists do not change once VS has gotten ahold of them.  Therefore if the
    /// list is changing over time it should be thrown away and a new list should be placed in the parent.
    /// </summary>
    class SimpleObjectList<T> : IVsSimpleObjectList2 where T : ISimpleObject {
        private readonly List<T> _children;
        private uint _updateCount;

        public SimpleObjectList() {
            _children = new List<T>();
        }

        public List<T> Children {
            get {
                return _children;
            }
        }

        public virtual void Update() {
            _updateCount++;
        }

        public uint UpdateCount {
            get { return _updateCount; }
            set { _updateCount = value; }
        }

        public const uint NullIndex = (uint)0xFFFFFFFF;

        int IVsSimpleObjectList2.CanDelete(uint index, out int pfOK) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            pfOK = _children[(int)index].CanDelete ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.CanGoToSource(uint index, VSOBJGOTOSRCTYPE SrcType, out int pfOK) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            pfOK = _children[(int)index].CanGoToSource ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.CanRename(uint index, string pszNewName, out int pfOK) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            pfOK = _children[(int)index].CanRename ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.CountSourceItems(uint index, out IVsHierarchy ppHier, out uint pItemid, out uint pcItems) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            _children[(int)index].SourceItems(out ppHier, out pItemid, out pcItems);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.DoDelete(uint index, uint grfFlags) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            _children[(int)index].Delete();
            _children.RemoveAt((int)index);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.DoDragDrop(uint index, IDataObject pDataObject, uint grfKeyState, ref uint pdwEffect) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            OleDataObject dataObject = new OleDataObject(pDataObject);
            _children[(int)index].DoDragDrop(dataObject, grfKeyState, pdwEffect);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.DoRename(uint index, string pszNewName, uint grfFlags) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            _children[(int)index].Rename(pszNewName, grfFlags);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.EnumClipboardFormats(uint index, uint grfFlags, uint celt, VSOBJCLIPFORMAT[] rgcfFormats, uint[] pcActual) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            uint copied = _children[(int)index].EnumClipboardFormats((_VSOBJCFFLAGS)grfFlags, rgcfFormats);
            if ((null != pcActual) && (pcActual.Length > 0)) {
                pcActual[0] = copied;
            }
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.FillDescription2(uint index, uint grfOptions, IVsObjectBrowserDescription3 pobDesc) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            _children[(int)index].FillDescription((_VSOBJDESCOPTIONS)grfOptions, pobDesc);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetBrowseObject(uint index, out object ppdispBrowseObj) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            ppdispBrowseObj = _children[(int)index].BrowseObject;
            if (null == ppdispBrowseObj) {
                return VSConstants.E_NOTIMPL;
            }
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetCapabilities2(out uint pgrfCapabilities) {
            pgrfCapabilities = (uint)Capabilities;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetCategoryField2(uint index, int Category, out uint pfCatField) {
            if (NullIndex == index) {
                pfCatField = CategoryField((LIB_CATEGORY)Category);
            } else if (index < (uint)_children.Count) {
                pfCatField = _children[(int)index].CategoryField((LIB_CATEGORY)Category);
            } else {
                throw new ArgumentOutOfRangeException("index");
            }
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetClipboardFormat(uint index, uint grfFlags, FORMATETC[] pFormatetc, STGMEDIUM[] pMedium) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GetContextMenu(uint index, out Guid pclsidActive, out int pnMenuId, out IOleCommandTarget ppCmdTrgtActive) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            CommandID commandId = _children[(int)index].ContextMenuID;
            if (null == commandId) {
                pclsidActive = Guid.Empty;
                pnMenuId = 0;
                ppCmdTrgtActive = null;
                return VSConstants.E_NOTIMPL;
            }
            pclsidActive = commandId.Guid;
            pnMenuId = commandId.ID;
            ppCmdTrgtActive = _children[(int)index] as IOleCommandTarget;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetDisplayData(uint index, VSTREEDISPLAYDATA[] pData) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            pData[0] = _children[(int)index].DisplayData;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetExpandable3(uint index, uint ListTypeExcluded, out int pfExpandable) {
            // There is a not empty implementation of GetCategoryField2, so this method should
            // return E_NOTIMPL.
            pfExpandable = 0;
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GetExtendedClipboardVariant(uint index, uint grfFlags, VSOBJCLIPFORMAT[] pcfFormat, out object pvarFormat) {
            pvarFormat = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GetFlags(out uint pFlags) {
            pFlags = (uint)Flags;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetItemCount(out uint pCount) {
            pCount = (uint)_children.Count;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetList2(uint index, uint ListType, uint flags, VSOBSEARCHCRITERIA2[] pobSrch, out IVsSimpleObjectList2 ppIVsSimpleObjectList2) {
            // TODO: Use the flags and list type to actually filter the result.
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            ppIVsSimpleObjectList2 = _children[(int)index].FilterView(ListType);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetMultipleSourceItems(uint index, uint grfGSI, uint cItems, VSITEMSELECTION[] rgItemSel) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GetNavInfo(uint index, out IVsNavInfo ppNavInfo) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            ppNavInfo = _children[(int)index] as IVsNavInfo;
            return ppNavInfo == null ? VSConstants.E_NOTIMPL : VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetNavInfoNode(uint index, out IVsNavInfoNode ppNavInfoNode) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            ppNavInfoNode = _children[(int)index] as IVsNavInfoNode;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetProperty(uint index, int propid, out object pvar) {
            if (propid == (int)_VSOBJLISTELEMPROPID.VSOBJLISTELEMPROPID_FULLNAME) {
                pvar = _children[(int)index].FullName;
                return VSConstants.S_OK;
            }

            pvar = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GetSourceContextWithOwnership(uint index, out string pbstrFilename, out uint pulLineNum) {
            pbstrFilename = null;
            pulLineNum = (uint)0;
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GetTextWithOwnership(uint index, VSTREETEXTOPTIONS tto, out string pbstrText) {
            // TODO: make use of the text option.
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            pbstrText = _children[(int)index].GetTextRepresentation(tto);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetTipTextWithOwnership(uint index, VSTREETOOLTIPTYPE eTipType, out string pbstrText) {
            // TODO: Make use of the tooltip type.
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            pbstrText = _children[(int)index].TooltipText;
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.GetUserContext(uint index, out object ppunkUserCtx) {
            ppunkUserCtx = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.GoToSource(uint index, VSOBJGOTOSRCTYPE SrcType) {
            if (index >= (uint)_children.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            _children[(int)index].GotoSource(SrcType);
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.LocateNavInfoNode(IVsNavInfoNode pNavInfoNode, out uint pulIndex) {
            Utilities.ArgumentNotNull("pNavInfoNode", pNavInfoNode);

            pulIndex = NullIndex;
            string nodeName;
            ErrorHandler.ThrowOnFailure(pNavInfoNode.get_Name(out nodeName));
            for (int i = 0; i < _children.Count; i++) {
                if (0 == string.Compare(_children[i].UniqueName, nodeName, StringComparison.OrdinalIgnoreCase)) {
                    pulIndex = (uint)i;
                    return VSConstants.S_OK;
                }
            }
            return VSConstants.S_FALSE;
        }

        int IVsSimpleObjectList2.OnClose(VSTREECLOSEACTIONS[] ptca) {
            // Do Nothing.
            return VSConstants.S_OK;
        }

        int IVsSimpleObjectList2.QueryDragDrop(uint index, IDataObject pDataObject, uint grfKeyState, ref uint pdwEffect) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.ShowHelp(uint index) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsSimpleObjectList2.UpdateCounter(out uint pCurUpdate) {
            pCurUpdate = _updateCount;
            return VSConstants.S_OK;
        }

        public virtual uint Capabilities { get { return 0; } }

        public virtual _VSTREEFLAGS Flags { get { return 0; } }

        public virtual uint CategoryField(LIB_CATEGORY lIB_CATEGORY) {
            return 0;
        }
    }
}
