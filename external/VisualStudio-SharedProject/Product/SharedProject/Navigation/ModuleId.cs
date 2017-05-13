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

using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Navigation {
    /// <summary>
    /// Class used to identify a module. The module is identified using the hierarchy that
    /// contains it and its item id inside the hierarchy.
    /// </summary>
    public sealed class ModuleId {
        private IVsHierarchy _ownerHierarchy;
        private uint _itemId;

        public ModuleId(IVsHierarchy owner, uint id) {
            _ownerHierarchy = owner;
            _itemId = id;
        }

        public IVsHierarchy Hierarchy {
            get { return _ownerHierarchy; }
        }

        public uint ItemID {
            get { return _itemId; }
        }

        public override int GetHashCode() {
            int hash = 0;
            if (null != _ownerHierarchy) {
                hash = _ownerHierarchy.GetHashCode();
            }
            hash = hash ^ (int)_itemId;
            return hash;
        }

        public override bool Equals(object obj) {
            ModuleId other = obj as ModuleId;
            if (null == obj) {
                return false;
            }
            if (!_ownerHierarchy.Equals(other._ownerHierarchy)) {
                return false;
            }
            return (_itemId == other._itemId);
        }
    }
}