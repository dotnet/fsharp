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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    public static class HierarchyExtensions {
        /// <summary>
        /// Get the top-level items present in the project
        /// </summary>
        public static IEnumerable<HierarchyItem> GetHierarchyItems(this IVsHierarchy project) {
            if (project == null) {
                throw new ArgumentNullException("project");
            }

            // Each item in VS OM is IVSHierarchy. 
            IVsHierarchy hierarchy = (IVsHierarchy)project;

            return GetChildItems(hierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Get project items
        /// </summary>
        internal static IEnumerable<HierarchyItem> GetChildItems(this IVsHierarchy project, uint itemId) {
            for (var childId = GetItemId(GetPropertyValue(project, (int)__VSHPROPID.VSHPROPID_FirstChild, itemId));
                childId != VSConstants.VSITEMID_NIL;
                childId = GetItemId(GetPropertyValue(project, (int)__VSHPROPID.VSHPROPID_NextSibling, childId))) {

                yield return new HierarchyItem(project, childId);
            }
        }

        /// <summary>
        /// Convert parameter object to ItemId
        /// </summary>
        private static uint GetItemId(object pvar) {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        /// <summary>
        /// Get the parameter property value
        /// </summary>
        public static object GetPropertyValue(this IVsHierarchy vsHierarchy, int propid, uint itemId) {
            if (itemId == VSConstants.VSITEMID_NIL) {
                return null;
            }

            object o;
            if (ErrorHandler.Failed(vsHierarchy.GetProperty(itemId, propid, out o))) {
                return null;
            }
            return o;
        }
    }
}
