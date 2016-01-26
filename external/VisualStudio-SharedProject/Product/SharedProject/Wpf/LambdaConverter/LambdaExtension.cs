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
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Windows.Data;
using System.Globalization;
using Microsoft.VisualStudioTools.Wpf;

namespace Microsoft.VisualStudioTools.Wpf {
    [ContentProperty("Lambda")]
    public class LambdaExtension : MarkupExtension {
        public string Lambda { get; set; }

        public LambdaExtension() {
        }

        public LambdaExtension(string lambda) {
            Lambda = lambda;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (Lambda == null) {
                throw new InvalidOperationException("Lambda not specified");
            }

            var rootProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            var root = rootProvider.RootObject;
            if (root == null) {
                throw new InvalidOperationException("Cannot locate root object - service provider did not provide IRootObjectProvider");
            }

            var provider = root as ILambdaConverterProvider;
            if (provider == null) {
                throw new InvalidOperationException("Root object does not implement ILambdaConverterProvider - code generator not run");
            }

            return provider.GetConverterForLambda(Lambda);
        }
    }
}
