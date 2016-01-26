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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockLocalRegistry : ILocalRegistry, ILocalRegistryCorrected {
        private static Guid AggregatorGuid = new Guid("{C402364C-5474-47e7-AE72-BF5418780221}");

        public int CreateInstance(Guid clsid, object punkOuter, ref Guid riid, uint dwFlags, out IntPtr ppvObj) {
            throw new NotImplementedException();
        }

        public int GetClassObjectOfClsid(ref Guid clsid, uint dwFlags, IntPtr lpReserved, ref Guid riid, out IntPtr ppvClassObject) {
            throw new NotImplementedException();
        }

        public int GetTypeLibOfClsid(Guid clsid, out VisualStudio.OLE.Interop.ITypeLib pptLib) {
            throw new NotImplementedException();
        }

        public int CreateInstance(Guid clsid, IntPtr punkOuterIUnknown, ref Guid riid, uint dwFlags, out IntPtr ppvObj) {
            if (clsid == typeof(Microsoft.VisualStudio.ProjectAggregator.CProjectAggregatorClass).GUID) {
                var res = new ProjectAggregator();
                ppvObj = Marshal.GetIUnknownForObject(res);
                return VSConstants.S_OK;
            }
            throw new NotImplementedException();
        }
    }

}
