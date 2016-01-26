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
using System.ComponentModel.Design;

namespace TestUtilities.Mocks {
    public class MockServiceProvider : IServiceProvider, IServiceContainer {
        public readonly Dictionary<Guid, object> Services = new Dictionary<Guid, object>();
        
        public object GetService(Type serviceType) {
            object service;
            Console.WriteLine("MockServiceProvider.GetService({0})", serviceType.Name);
            Services.TryGetValue(serviceType.GUID, out service);
            return service;
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
            Services[serviceType.GUID] = callback(this, serviceType);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback) {
            Services[serviceType.GUID] = callback(this, serviceType);
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote) {
            Services[serviceType.GUID] = serviceInstance;
        }

        public void AddService(Type serviceType, object serviceInstance) {
            Services[serviceType.GUID] = serviceInstance;
        }

        public void RemoveService(Type serviceType, bool promote) {
            Services.Remove(serviceType.GUID);
        }

        public void RemoveService(Type serviceType) {
            Services.Remove(serviceType.GUID);
        }
    }
}
