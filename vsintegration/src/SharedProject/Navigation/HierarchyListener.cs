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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudioTools.Project;
using VSConstants = Microsoft.VisualStudio.VSConstants;

namespace Microsoft.VisualStudioTools.Navigation {
    class HierarchyEventArgs : EventArgs {
        private uint _itemId;
        private string _fileName;
        private IVsTextLines _buffer;

        public HierarchyEventArgs(uint itemId, string canonicalName) {
            _itemId = itemId;
            _fileName = canonicalName;
        }
        public string CanonicalName {
            get { return _fileName; }
        }
        public uint ItemID {
            get { return _itemId; }
        }
        public IVsTextLines TextBuffer {
            get { return _buffer; }
            set { _buffer = value; }
        }
    }

    internal abstract partial class LibraryManager : IDisposable, IVsRunningDocTableEvents {
        internal class HierarchyListener : IVsHierarchyEvents, IDisposable {
            private IVsHierarchy _hierarchy;
            private uint _cookie;
            private LibraryManager _manager;

            public HierarchyListener(IVsHierarchy hierarchy, LibraryManager manager) {
                Utilities.ArgumentNotNull("hierarchy", hierarchy);
                Utilities.ArgumentNotNull("manager", manager);

                _hierarchy = hierarchy;
                _manager = manager;
            }

            protected IVsHierarchy Hierarchy {
                get { return _hierarchy; }
            }

            #region Public Methods
            public bool IsListening {
                get { return (0 != _cookie); }
            }
            public void StartListening(bool doInitialScan) {
                if (0 != _cookie) {
                    return;
                }
                ErrorHandler.ThrowOnFailure(
                    _hierarchy.AdviseHierarchyEvents(this, out _cookie));
                if (doInitialScan) {
                    InternalScanHierarchy(VSConstants.VSITEMID_ROOT);
                }
            }
            public void StopListening() {
                InternalStopListening(true);
            }
            #endregion

            #region IDisposable Members

            public void Dispose() {
                InternalStopListening(false);
                _cookie = 0;
                _hierarchy = null;
            }

            #endregion
            #region IVsHierarchyEvents Members

            public int OnInvalidateIcon(IntPtr hicon) {
                // Do Nothing.
                return VSConstants.S_OK;
            }

            public int OnInvalidateItems(uint itemidParent) {
                // TODO: Find out if this event is needed.
                return VSConstants.S_OK;
            }

            public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded) {
                // Check if the item is my language file.
                string name;
                if (!IsAnalyzableSource(itemidAdded, out name)) {
                    return VSConstants.S_OK;
                }

                // This item is a my language file, so we can notify that it is added to the hierarchy.
                HierarchyEventArgs args = new HierarchyEventArgs(itemidAdded, name);
                _manager.OnNewFile(_hierarchy, args);
                return VSConstants.S_OK;
            }

            public int OnItemDeleted(uint itemid) {
                // Notify that the item is deleted only if it is my language file.
                string name;
                if (!IsAnalyzableSource(itemid, out name)) {
                    return VSConstants.S_OK;
                }
                HierarchyEventArgs args = new HierarchyEventArgs(itemid, name);
                _manager.OnDeleteFile(_hierarchy, args);
                return VSConstants.S_OK;
            }

            public int OnItemsAppended(uint itemidParent) {
                // TODO: Find out what this event is about.
                return VSConstants.S_OK;
            }

            public int OnPropertyChanged(uint itemid, int propid, uint flags) {
                if ((null == _hierarchy) || (0 == _cookie)) {
                    return VSConstants.S_OK;
                }
                string name;
                if (!IsAnalyzableSource(itemid, out name)) {
                    return VSConstants.S_OK;
                }
                if (propid == (int)__VSHPROPID.VSHPROPID_IsNonMemberItem) {
                    _manager.IsNonMemberItemChanged(_hierarchy, new HierarchyEventArgs(itemid, name));
                }
                return VSConstants.S_OK;
            }
            #endregion

            private bool InternalStopListening(bool throwOnError) {
                if ((null == _hierarchy) || (0 == _cookie)) {
                    return false;
                }
                int hr = _hierarchy.UnadviseHierarchyEvents(_cookie);
                if (throwOnError) {
                    ErrorHandler.ThrowOnFailure(hr);
                }
                _cookie = 0;
                return ErrorHandler.Succeeded(hr);
            }

            /// <summary>
            /// Do a recursive walk on the hierarchy to find all this language files in it.
            /// It will generate an event for every file found.
            /// </summary>
            private void InternalScanHierarchy(uint itemId) {
                uint currentItem = itemId;
                while (VSConstants.VSITEMID_NIL != currentItem) {
                    // If this item is a my language file, then send the add item event.
                    string itemName;
                    if (IsAnalyzableSource(currentItem, out itemName)) {
                        HierarchyEventArgs args = new HierarchyEventArgs(currentItem, itemName);
                        _manager.OnNewFile(_hierarchy, args);
                    }

                    // NOTE: At the moment we skip the nested hierarchies, so here  we look for the 
                    // children of this node.
                    // Before looking at the children we have to make sure that the enumeration has not
                    // side effects to avoid unexpected behavior.
                    object propertyValue;
                    bool canScanSubitems = true;
                    int hr = _hierarchy.GetProperty(currentItem, (int)__VSHPROPID.VSHPROPID_HasEnumerationSideEffects, out propertyValue);
                    if ((VSConstants.S_OK == hr) && (propertyValue is bool)) {
                        canScanSubitems = !(bool)propertyValue;
                    }
                    // If it is allow to look at the sub-items of the current one, lets do it.
                    if (canScanSubitems) {
                        object child;
                        hr = _hierarchy.GetProperty(currentItem, (int)__VSHPROPID.VSHPROPID_FirstChild, out child);
                        if (VSConstants.S_OK == hr) {
                            // There is a sub-item, call this same function on it.
                            InternalScanHierarchy(GetItemId(child));
                        }
                    }

                    // Move the current item to its first visible sibling.
                    object sibling;
                    hr = _hierarchy.GetProperty(currentItem, (int)__VSHPROPID.VSHPROPID_NextSibling, out sibling);
                    if (VSConstants.S_OK != hr) {
                        currentItem = VSConstants.VSITEMID_NIL;
                    } else {
                        currentItem = GetItemId(sibling);
                    }
                }
            }

            private bool IsAnalyzableSource(uint itemId, out string canonicalName) {
                // Find out if this item is a physical file.
                Guid typeGuid;
                canonicalName = null;
                int hr;
                try {
                    hr = Hierarchy.GetGuidProperty(itemId, (int)__VSHPROPID.VSHPROPID_TypeGuid, out typeGuid);
                } catch (System.Runtime.InteropServices.COMException) {
                    return false;
                }
                if (Microsoft.VisualStudio.ErrorHandler.Failed(hr) ||
                    VSConstants.GUID_ItemType_PhysicalFile != typeGuid) {
                    // It is not a file, we can exit now.
                    return false;
                }

                // This item is a file; find if current language can recognize it.
                hr = Hierarchy.GetCanonicalName(itemId, out canonicalName);
                if (ErrorHandler.Failed(hr)) {
                    return false;
                }
                return (System.IO.Path.GetExtension(canonicalName).Equals(".xaml", StringComparison.OrdinalIgnoreCase)) ||
                    _manager._package.IsRecognizedFile(canonicalName);
            }

            /// <summary>
            /// Gets the item id.
            /// </summary>
            /// <param name="variantValue">VARIANT holding an itemid.</param>
            /// <returns>Item Id of the concerned node</returns>
            private static uint GetItemId(object variantValue) {
                if (variantValue == null)
                    return VSConstants.VSITEMID_NIL;
                if (variantValue is int)
                    return (uint)(int)variantValue;
                if (variantValue is uint)
                    return (uint)variantValue;
                if (variantValue is short)
                    return (uint)(short)variantValue;
                if (variantValue is ushort)
                    return (uint)(ushort)variantValue;
                if (variantValue is long)
                    return (uint)(long)variantValue;
                return VSConstants.VSITEMID_NIL;
            }
        }
    }
}
