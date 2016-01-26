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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;

namespace Microsoft.VisualStudioTools.MockVsTests {

    class ProjectAggregator : IVsProjectAggregator2, ICustomQueryInterface {
        private IntPtr _inner;
        private IntPtr _project;

        public int SetInner(IntPtr innerIUnknown) {
            _inner = innerIUnknown;
            return VSConstants.S_OK;
        }

        public int SetMyProject(IntPtr projectIUnknown) {
            _project = projectIUnknown;
            return VSConstants.S_OK;
        }

        public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr ppv) {
            if (_project != IntPtr.Zero) {
                if (ErrorHandler.Succeeded(Marshal.QueryInterface(_project, ref iid, out ppv))) {
                    return CustomQueryInterfaceResult.Handled;
                }
            }
            if (_inner != IntPtr.Zero) {
                if (ErrorHandler.Succeeded(Marshal.QueryInterface(_inner, ref iid, out ppv))) {
                    return CustomQueryInterfaceResult.Handled;
                }
            }
            ppv = IntPtr.Zero;
            return CustomQueryInterfaceResult.Failed;
        }
    }
}
