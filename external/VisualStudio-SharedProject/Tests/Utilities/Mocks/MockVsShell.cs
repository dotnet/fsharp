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

namespace TestUtilities.Mocks {
    public class MockVsShell : IVsShell {
        public readonly Dictionary<int, object> Properties = new Dictionary<int, object>();
        public readonly object ReadOnlyPropertyValue = new object();
        
        public int GetProperty(int propid, out object pvar) {
            if (Properties.TryGetValue(propid, out pvar)) {
                Console.WriteLine("MockVsShell.GetProperty(propid={0}) -> {1}", propid, pvar);
                return VSConstants.S_OK;
            }
            Console.WriteLine("MockVsShell.GetProperty(propid={0}) -> <unknown value>", propid);
            return VSConstants.E_INVALIDARG;
        }

        public int SetProperty(int propid, object var) {
            object value;
            if (Properties.TryGetValue(propid, out value)) {
                if (value == ReadOnlyPropertyValue) {
                    Console.WriteLine("MockVsShell.SetProperty(propid={0}, var={1}) -> E_INVALIDARG", propid, var);
                    return VSConstants.E_INVALIDARG;
                }
                Console.WriteLine("MockVsShell.SetProperty(propid={0}, var={1}) replacing {2}", propid, var, value);
            } else {
                Console.WriteLine("MockVsShell.SetProperty(propid={0}, var={1})", propid, var);
            }
            Properties[propid] = var;
            return VSConstants.S_OK;
        }


        public int AdviseBroadcastMessages(IVsBroadcastMessageEvents pSink, out uint pdwCookie) {
            throw new NotImplementedException();
        }

        public int AdviseShellPropertyChanges(IVsShellPropertyEvents pSink, out uint pdwCookie) {
            throw new NotImplementedException();
        }

        public int GetPackageEnum(out IEnumPackages ppenum) {
            throw new NotImplementedException();
        }

        public int IsPackageInstalled(ref Guid guidPackage, out int pfInstalled) {
            throw new NotImplementedException();
        }

        public int IsPackageLoaded(ref Guid guidPackage, out IVsPackage ppPackage) {
            throw new NotImplementedException();
        }

        public int LoadPackage(ref Guid guidPackage, out IVsPackage ppPackage) {
            throw new NotImplementedException();
        }

        public int LoadPackageString(ref Guid guidPackage, uint resid, out string pbstrOut) {
            throw new NotImplementedException();
        }

        public int LoadUILibrary(ref Guid guidPackage, uint dwExFlags, out uint phinstOut) {
            throw new NotImplementedException();
        }

        public int UnadviseBroadcastMessages(uint dwCookie) {
            throw new NotImplementedException();
        }

        public int UnadviseShellPropertyChanges(uint dwCookie) {
            throw new NotImplementedException();
        }
    }
}
