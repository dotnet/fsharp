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

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    public struct HierarchyItem {
        public readonly IVsHierarchy Hierarchy;
        public readonly uint ItemId;

        public HierarchyItem(IVsHierarchy hierarchy, uint itemId) {
            Hierarchy = hierarchy;
            ItemId = itemId;
        }

        public bool IsNull {
            get {
                return Hierarchy == null;
            }
        }

        public string CanonicalName {
            get {
                return GetCanonicalName(ItemId, Hierarchy);
            }
        }

        public bool IsNonMemberItem {
            get {
                return (GetProperty((int)__VSHPROPID.VSHPROPID_IsNonMemberItem) as bool?) ?? false;
            }
        }

        public string Caption {
            get {
                return GetProperty((int)__VSHPROPID.VSHPROPID_Caption) as string;
            }
        }

        public string EditLabel {
            get {
                return GetProperty((int)__VSHPROPID.VSHPROPID_EditLabel) as string;
            }
            set {
                Hierarchy.SetProperty(ItemId, (int)__VSHPROPID.VSHPROPID_EditLabel, value);
            }
        }

        public string Name {
            get {
                return GetProperty((int)__VSHPROPID.VSHPROPID_Name) as string;
            }
        }

        public bool IsHidden {
            get {
                return (GetProperty((int)__VSHPROPID.VSHPROPID_IsHiddenItem) as bool?) ?? false;
            }
        }

        public bool IsLinkFile {
            get {
                return (GetProperty((int)__VSHPROPID2.VSHPROPID_IsLinkFile) as bool?) ?? false;
            }
        }

        public object ExtensionObject {
            get {
                return GetProperty((int)__VSHPROPID.VSHPROPID_ExtObject);
            }
        }

        public IEnumerable<HierarchyItem> Children {
            get {
                return Hierarchy.GetChildItems(ItemId);
            }
        }

        private object GetProperty(int propid) {
            return Hierarchy.GetPropertyValue(propid, ItemId);
        }

        /// <summary>
        /// Get the canonical name
        /// </summary>
        private static string GetCanonicalName(uint itemId, IVsHierarchy hierarchy) {
            Debug.Assert(itemId != VSConstants.VSITEMID_NIL, "ItemId cannot be nill");

            string strRet = string.Empty;
            if (ErrorHandler.Failed(hierarchy.GetCanonicalName(itemId, out strRet))) {
                return string.Empty;
            }
            return strRet;
        }
    }
}
