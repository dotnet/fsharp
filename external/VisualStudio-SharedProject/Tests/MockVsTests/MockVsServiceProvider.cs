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
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudioTools.MockVsTests {
    [Export(typeof(SVsServiceProvider))]
    [Export(typeof(MockVsServiceProvider))]
    public class MockVsServiceProvider : SVsServiceProvider, IOleServiceProvider, IServiceContainer {
        private readonly MockVs _vs;
        private Dictionary<Type, object> _servicesByType = new Dictionary<Type, object>();
        private Dictionary<Guid, object> _servicesByGuid = new Dictionary<Guid, object>();

        [ImportingConstructor]
        public MockVsServiceProvider(MockVs mockVs) {
            _vs = mockVs;
            _servicesByType.Add(typeof(IOleServiceProvider), this);
        }

        public void AddService(Type type, object inst) {
            _servicesByType[type] = inst;
            _servicesByGuid[type.GUID] = inst;
        }

        public void AddService(Type type, object inst, bool promote) {
            AddService(type, inst);
        }

        public object GetService(Guid serviceType) {
            object res;
            if (_servicesByGuid.TryGetValue(serviceType, out res)) {
                return res;
            }
            Console.WriteLine("Unknown service: " + serviceType);
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType) {
            object res;
            if (_servicesByType.TryGetValue(serviceType, out res)) {
                return res;
            }

            if (_servicesByGuid.TryGetValue(serviceType.GUID, out res)) {
                return res;
            }

            Console.WriteLine("Unknown service: " + serviceType.FullName);
            return null;
        }

        public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject) {
            object res;
            if (_servicesByGuid.TryGetValue(guidService, out res)) {
                IntPtr punk = Marshal.GetIUnknownForObject(res);
                try {
                    return Marshal.QueryInterface(punk, ref riid, out ppvObject);
                } finally {
                    Marshal.Release(punk);
                }
            }

            Console.WriteLine("Unknown interface: {0}", guidService);
            ppvObject = IntPtr.Zero;
            return VSConstants.E_NOINTERFACE;
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
            AddService(serviceType, callback(this, serviceType), promote);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback) {
            AddService(serviceType, callback(this, serviceType));
        }

        public void RemoveService(Type serviceType, bool promote) {
            _servicesByType.Remove(serviceType);
            _servicesByGuid.Remove(serviceType.GUID);
        }

        public void RemoveService(Type serviceType) {
            _servicesByType.Remove(serviceType);
            _servicesByGuid.Remove(serviceType.GUID);
        }
    }
}
