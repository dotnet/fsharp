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
using System.Globalization;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Microsoft.VisualStudioTools.TestAdapter {
    internal static class ServiceProviderExtensions {
        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class {
            return serviceProvider.GetService<T>(typeof(T));
        }

        public static T GetService<T>(this IServiceProvider serviceProvider, Type serviceType)
            where T : class {
            ValidateArg.NotNull(serviceProvider, "serviceProvider");
            ValidateArg.NotNull(serviceType, "serviceType");

            var serviceInstance = serviceProvider.GetService(serviceType) as T;
            if (serviceInstance == null) {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, serviceType.Name));
            }

            return serviceInstance;
        }
    }
}
