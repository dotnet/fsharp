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
    class MockOutputWindow : IVsOutputWindow {
        private static Dictionary<Guid, MockOutputWindowPane> _panes = new Dictionary<Guid, MockOutputWindowPane>() {
            {VSConstants.OutputWindowPaneGuid.GeneralPane_guid, new MockOutputWindowPane("General") }
        };

        public int CreatePane(ref Guid rguidPane, string pszPaneName, int fInitVisible, int fClearWithSolution) {
            MockOutputWindowPane pane;
            if (_panes.TryGetValue(rguidPane, out pane)) {
                _panes[rguidPane] = new MockOutputWindowPane(pszPaneName);
            }
            return VSConstants.S_OK;
        }

        public int DeletePane(ref Guid rguidPane) {
            _panes.Remove(rguidPane);
            return VSConstants.S_OK;
        }

        public int GetPane(ref Guid rguidPane, out IVsOutputWindowPane ppPane) {
            MockOutputWindowPane pane;
            if (_panes.TryGetValue(rguidPane, out pane)) {
                ppPane = pane;
                return VSConstants.S_OK;
            }
            ppPane = null;
            return VSConstants.E_FAIL;
        }
    }
}
