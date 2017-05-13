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
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    [Export(typeof(IVsHierarchyItemManager))]
    public class MockVsHierarchyItemManager : IVsHierarchyItemManager {
        public event EventHandler<HierarchyItemEventArgs> AfterInvalidateItems { add { } remove { } }

        public IVsHierarchyItem GetHierarchyItem(IVsHierarchy hierarchy, uint itemid) {
            throw new NotImplementedException();
        }

        public bool IsChangingItems {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<HierarchyItemEventArgs> OnItemAdded { add { } remove { } }

        public bool TryGetHierarchyItem(IVsHierarchy hierarchy, uint itemid, out IVsHierarchyItem item) {
            item = null;
            return false;
        }

        public bool TryGetHierarchyItemIdentity(IVsHierarchy hierarchy, uint itemid, out IVsHierarchyItemIdentity identity) {
            throw new NotImplementedException();
        }
    }
}
