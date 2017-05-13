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
using System.Diagnostics;

namespace Microsoft.VisualStudioTools.Project {
    sealed class HierarchyIdMap {
        private readonly List<HierarchyNode> _ids = new List<HierarchyNode>();
        private readonly Stack<int> _freedIds = new Stack<int>();

        /// <summary>
        /// Must be called from the UI thread
        /// </summary>
        public uint Add(HierarchyNode node) {
#if DEBUG
            foreach (var item in _ids) {
                Debug.Assert(node != item);
            }
#endif
            if (_freedIds.Count > 0) {
                var i = _freedIds.Pop();
                _ids[i] = node;
                return (uint)i + 1;
            } else {
                _ids.Add(node);
                // ids are 1 based
                return (uint)_ids.Count;
            }
        }

        /// <summary>
        /// Must be called from the UI thread
        /// </summary>
        public void Remove(HierarchyNode node) {
            int i = (int)node.ID - 1;
            if(i < 0 ||
                i >= _ids.Count ||
                !object.ReferenceEquals(node, _ids[i])) {
                throw new InvalidOperationException("Removing node with invalid ID or map is corrupted");
            }

            _ids[i] = null;
            _freedIds.Push(i);
        }

        /// <summary>
        /// Must be called from the UI thread
        /// </summary>
        public HierarchyNode this[uint itemId] {
            get {
                int i = (int)itemId - 1;
                if (0 <= i && i < _ids.Count) {
                    return _ids[i];
                }
                return null;
            }
        }
    }
}
