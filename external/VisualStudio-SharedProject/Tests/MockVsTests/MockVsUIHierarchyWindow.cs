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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using TestUtilities;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsUIHierarchyWindow : IVsUIHierarchyWindow, IOleCommandTarget, ISelectionContainer {
        private readonly HashSet<HierarchyItem> _selectedItems = new HashSet<HierarchyItem>();
        private readonly MockVs _mockVs;
        private MultiItemSelect _multiSelect;
        private string _editLabel;
        private int _selectionStart, _selectionLength;

        public MockVsUIHierarchyWindow(MockVs vs) {
            _mockVs = vs;
        }

        public int AddUIHierarchy(IVsUIHierarchy pUIH, uint grfAddOptions) {
            throw new NotImplementedException();
        }

        public int ExpandItem(IVsUIHierarchy pUIH, uint itemid, EXPANDFLAGS expf) {
            throw new NotImplementedException();
        }

        public int FindCommonSelectedHierarchy(uint grfOpt, out IVsUIHierarchy lppCommonUIH) {
            throw new NotImplementedException();
        }

        public int GetCurrentSelection(out IntPtr ppHier, out uint pitemid, out IVsMultiItemSelect ppMIS) {
            throw new NotImplementedException();
        }

        public int GetItemState(IVsUIHierarchy pHier, uint itemid, uint dwStateMask, out uint pdwState) {
            throw new NotImplementedException();
        }

        public int Init(IVsUIHierarchy pUIH, uint grfUIHWF, out object ppunkOut) {
            throw new NotImplementedException();
        }

        public int RemoveUIHierarchy(IVsUIHierarchy pUIH) {
            throw new NotImplementedException();
        }

        public int SetCursor(IntPtr hNewCursor, out IntPtr phOldCursor) {
            throw new NotImplementedException();
        }

        public int SetWindowHelpTopic(string lpszHelpFile, uint dwContext) {
            throw new NotImplementedException();
        }

        public void AddSelectedItem(HierarchyItem item) {
            _selectedItems.Add(item);
            if (_selectedItems.Count > 1) {
                _multiSelect = new MultiItemSelect(_selectedItems.ToArray());
            } else {
                _multiSelect = null;
            }
            _mockVs._monSel._emptyCtx.OnSelectChangeEx(
                item.Hierarchy,
                item.ItemId,
                _multiSelect,
                this
            );
        }

        public void ClearSelectedItems() {
            _selectedItems.Clear();
            _multiSelect = null;
        }

        class MultiItemSelect : IVsMultiItemSelect {
            private HierarchyItem[] _items;

            public MultiItemSelect(HierarchyItem[] hierarchyItem) {
                _items = hierarchyItem;
            }

            public int GetSelectedItems(uint grfGSI, uint cItems, VSITEMSELECTION[] rgItemSel) {
                var flags = (__VSGSIFLAGS)grfGSI;
                for (int i = 0; i < cItems && i < _items.Length; i++) {
                    rgItemSel[i].itemid = _items[i].ItemId;
                    if (!flags.HasFlag(__VSGSIFLAGS.GSI_fOmitHierPtrs)) {
                        rgItemSel[i].pHier = _items[i].Hierarchy;
                    }
                }
                return VSConstants.S_OK;
            }

            public int GetSelectionInfo(out uint pcItems, out int pfSingleHierarchy) {
                pcItems = (uint)_items.Length;
                var hier = _items[0].Hierarchy;
                pfSingleHierarchy = 1;
                for (int i = 1; i < _items.Length; i++) {
                    if (hier != _items[i].Hierarchy) {
                        pfSingleHierarchy = 0;
                        break;
                    }
                }

                return VSConstants.S_OK;
            }
        }

        public int CountObjects(uint dwFlags, out uint pc) {
            throw new NotImplementedException();
        }

        public int GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects) {
            throw new NotImplementedException();
        }

        public int SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags) {
            throw new NotImplementedException();
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            foreach (var item in _selectedItems) {
                if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97) {
                    switch ((VSConstants.VSStd97CmdID)nCmdID) {
                        case VSConstants.VSStd97CmdID.Rename:
                            if ((_editLabel = item.EditLabel) != null) {
                                _selectionLength = 0;
                                _selectionLength = _editLabel.Length - Path.GetExtension(_editLabel).Length;
                                return VSConstants.S_OK;
                            }
                            break;
                    }
                } else if (pguidCmdGroup == VSConstants.VSStd2K) {
                    switch ((VSConstants.VSStd2KCmdID)nCmdID) {
                        case VSConstants.VSStd2KCmdID.TYPECHAR:
                            if (_editLabel != null) {
                                if (_selectionLength != 0) {
                                    _editLabel = _editLabel.Remove(_selectionStart, _selectionLength);
                                    _selectionLength = 0;
                                }
                                var ch = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                                _editLabel = _editLabel.Insert(_selectionStart, ch.ToString());
                                _selectionStart++;
                            }
                            return VSConstants.S_OK;
                        case VSConstants.VSStd2KCmdID.RETURN:
                            if (_editLabel != null) {
                                var tmpItem = item;
                                tmpItem.EditLabel = _editLabel;
                                _editLabel = null;
                                return VSConstants.S_OK;
                            }
                            break;
                        case VSConstants.VSStd2KCmdID.DELETE:
                            DeleteItem(item);
                            break;
                    }
                }

                IVsUIHierarchy uiHier = item.Hierarchy as IVsUIHierarchy;
                if (uiHier != null) {
                    int hr = uiHier.ExecCommand(
                        item.ItemId,
                        ref pguidCmdGroup,
                        nCmdID,
                        nCmdexecopt,
                        pvaIn,
                        pvaOut
                    );
                    if (hr != (int)NativeMethods.OLECMDERR_E_NOTSUPPORTED) {
                        return hr;
                    }
                }
            }

            return NativeMethods.OLECMDERR_E_NOTSUPPORTED;
        }


        private void DeleteItem(HierarchyItem item) {
            var deleteHandler = item.Hierarchy as IVsHierarchyDeleteHandler;
            int canRemoveItem = 0, canDeleteItem = 0;
            if (deleteHandler != null &&
                ErrorHandler.Succeeded(deleteHandler.QueryDeleteItem((uint)__VSDELETEITEMOPERATION.DELITEMOP_RemoveFromProject, item.ItemId, out canRemoveItem))) {
                deleteHandler.QueryDeleteItem((uint)__VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage, item.ItemId, out canDeleteItem);
            }
            bool showSpecificMsg = ShouldShowSpecificMessage(item, canRemoveItem, canDeleteItem);
            if (canRemoveItem != 0) {
                if (canDeleteItem != 0) {
                    // show delete or remove dialog...
                } else {
                    // show remove dialog...
                    PromptAndDelete(
                        item,
                        deleteHandler,
                        __VSDELETEITEMOPERATION.DELITEMOP_RemoveFromProject,
                        ""
                    );
                }

            } else if (canDeleteItem != 0) {
                object name;
                ErrorHandler.ThrowOnFailure(item.Hierarchy.GetProperty(item.ItemId, (int)__VSHPROPID.VSHPROPID_Name, out name));
                string message = string.Format("'{0}' will be deleted permanently.", name);
                PromptAndDelete(
                    item,
                    deleteHandler,
                    __VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage,
                    message
                );
            }
        }

        private void PromptAndDelete(HierarchyItem item, IVsHierarchyDeleteHandler deleteHandler, __VSDELETEITEMOPERATION deleteType, string message) {
            Guid unused = Guid.Empty;
            int result;
            // show delete dialog...
            if (ErrorHandler.Succeeded(
                _mockVs.UIShell.ShowMessageBox(
                    0,
                    ref unused,
                    null,
                    message,
                    null,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    0,
                    out result
                )) && result == DialogResult.OK) {
                int hr = deleteHandler.DeleteItem(
                    (uint)deleteType,
                    item.ItemId
                );

                if (ErrorHandler.Failed(hr) && hr != VSConstants.OLE_E_PROMPTSAVECANCELLED) {
                    _mockVs.UIShell.ReportErrorInfo(hr);
                }
            }
        }

        private bool ShouldShowSpecificMessage(HierarchyItem item, int canRemoveItem, int canDeleteItem) {
            __VSDELETEITEMOPERATION op = 0;
            if (canRemoveItem != 0) {
                op |= __VSDELETEITEMOPERATION.DELITEMOP_RemoveFromProject;
            }
            if (canDeleteItem != 0) {
                op |= __VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage;
            }

            IVsHierarchyDeleteHandler2 deleteHandler = item.Hierarchy as IVsHierarchyDeleteHandler2;
            if (deleteHandler != null) {
                int dwShowStandardMessage;
                uint pdwDelItemOp;
                deleteHandler.ShowSpecificDeleteRemoveMessage(
                    (uint)op,
                    1,
                    new[] { item.ItemId },
                    out dwShowStandardMessage,
                    out pdwDelItemOp
                );
                return dwShowStandardMessage != 0;
            }
            return false;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            foreach (var item in _selectedItems) {
                IOleCommandTarget target = item.Hierarchy as IOleCommandTarget;
                if (target != null) {
                    int hr = target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
                    if (hr != (int)NativeMethods.OLECMDERR_E_NOTSUPPORTED) {
                        return hr;
                    }
                }

                IVsUIHierarchy uiHier = item.Hierarchy as IVsUIHierarchy;
                if (uiHier != null) {
                    return uiHier.QueryStatusCommand(
                        item.ItemId,
                        ref pguidCmdGroup,
                        cCmds,
                        prgCmds,
                        pCmdText
                    );
                }
            }

            return NativeMethods.OLECMDERR_E_NOTSUPPORTED;
        }
    }
}
